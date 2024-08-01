using Inspector.POService.InspectionResults;
using Inspector.POService.LicenseValidator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;


namespace Inspector.POService.SyncService
{
    public class SyncService : IDisposable
    {
        #region Member Variables
        private POLicenseValidator _licenseValidator;
        private string _apiToken;
        private HttpClient _httpClient;
        private LicenseInfo _licenseInfo;
        private Dictionary<string, string> _reqHeaderWithToken = new Dictionary<string, string>();
        private string _resultFileStringDir = ConfigurationManager.AppSettings.Get("ResultJsonFilePath");
        private RequestHandler.IRequestHandler _requestHandler;
        private bool disposedValue;
        #endregion

        public SyncService() 
        {
            _licenseValidator = new POLicenseValidator();
            _httpClient = new HttpClient();
            _requestHandler = new RequestHandler.RequestHandler(_httpClient);
        }

        public async Task ConnectWithPlexorOnline(Action<string> callBack)
        {
            _licenseInfo = _licenseValidator.GetStoredLicenseInfo;
            var loginDto = new LoginDto()
            {
                customer_id = _licenseInfo.InspectorPCCustomerId,
                device_id = _licenseInfo.InspectorPCDeviceId,
            };

            var token = await CommonService.Login(loginDto, _licenseInfo.InspectorPCBaseUrl);
            this._apiToken = token;
            _reqHeaderWithToken.Add("token", this._apiToken);
            callBack?.Invoke(token);
        }

        public async Task CheckLicenseInformation(Action<string> callBack)
        {
            string maybeError = null;
            try
            {
                var storedLicenseInfo = _licenseValidator.GetStoredLicenseInfo;
                var maybeUpdatedLicenseInfo = await GetUpdatedLicenseInfo();
                var isLicenseValid = IsLicenseActive(maybeUpdatedLicenseInfo);

                if (!isLicenseValid)
                {
                    maybeError = "License is inactive or has exceeded expiry date";
                }

                UpdateStoredLicenseInfo(maybeUpdatedLicenseInfo);
            }
            catch (Exception ex)
            {
                maybeError = ex.Message;
            }
            callBack?.Invoke(maybeError);
        }

        public bool IsResultFilePresent
        {
            get
            {
                return File.Exists(_resultFileStringDir);
            }
        }

        public async Task<string> SendResultToPlexorOnline()
        {
            var responseStr = "";
            try
            {
                var jsonLine = "";
                using (StreamReader reader = new StreamReader(_resultFileStringDir))
                {
                    jsonLine = reader.ReadToEnd();
                }
                var data = JsonSerializer.Deserialize<List<InspectionResultJSON>>(jsonLine);
                using (var reqHandler = new RequestHandler.RequestHandler(_httpClient))
                {
                    responseStr = await reqHandler.PostAsync<string>(_licenseInfo.InspectorPCBaseUrl.TrimEnd('/') + "/" + ConfigurationManager.AppSettings.Get("InspectionResultUrl"), data, _reqHeaderWithToken);
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                return responseStr;
                throw new Exception(ex.Message);
            }
        }

        public async Task FetchUpdatedData(string applicationXmlPath, Action<string> callBack)
        {
            ErrorStringList maybeErrors = new ErrorStringList();
            try
            {
                CreateDirectoryIfNotExist();
                string maybeError;
                maybeError = await StorePRSInformation();
                maybeErrors.Add(maybeError);
                maybeError = await StoreInspectionProcedure();
                maybeErrors.Add(maybeError);
                maybeError = await StorePlexorDeviceInfo();
                maybeErrors.Add(maybeError);
                maybeError = await UpdateApplicationSettingsFile(applicationXmlPath);
                maybeErrors.Add(maybeError);
            }
            catch (Exception ex)
            {
                maybeErrors.Add(ex.Message);
            }

            callBack?.Invoke(maybeErrors.Pop());
        }

        #region Helper methods
        private async Task<DeviceStatus> GetUpdatedLicenseInfo()
        {
            try
            {
                using (var reqHandler = new RequestHandler.RequestHandler(_httpClient))
                {
                    DeviceStatus deviceStatus = await reqHandler.GetAsync<DeviceStatus>(GetBaseUrl + ConfigurationManager.AppSettings.Get("DeviceStatusUrl"), _reqHeaderWithToken);
                    return deviceStatus;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}; Source: GetUpdatedLicenseInfo()");
            }
        }

        private void CreateDirectoryIfNotExist()
        {
            if (!Directory.Exists(Assembly.GetExecutingAssembly().Location + "Data/JSON"))
            {
                Directory.CreateDirectory(Assembly.GetExecutingAssembly().Location + "Data/JSON");
            }
        }

        private bool IsLicenseActive(DeviceStatus deviceStatus)
        {
            return deviceStatus.is_active && deviceStatus.license_expiry_date.ToUniversalTime() >= DateTime.UtcNow;
        }

        private void UpdateStoredLicenseInfo(DeviceStatus updateLicenseInformation)
        {
            var storedLicenseInfo = _licenseValidator.GetStoredLicenseInfo;

            storedLicenseInfo.LicenseExpiryDate = updateLicenseInformation.license_expiry_date;
            storedLicenseInfo.LicenseStatus = updateLicenseInformation.license_status;

            _licenseValidator.StoreLicenseInformationToRegistry(storedLicenseInfo);
        }

        private string GetBaseUrl
        {
            get
            {
                return _licenseInfo.InspectorPCBaseUrl.TrimEnd('/') + ":8000/";
            }
        }

        private string StationInformationAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("StationInformationAPIUrl");
            }
        }

        private string StationInformationJsonPath
        {
            get
            {
                return "Data/JSON/" + ConfigurationManager.AppSettings.Get("StationInformationJsonPath");
            }
        }

        private string PlexorInformationAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("PlexorInformationAPIUrl");
            }
        }

        private string InspectionProcedureJsonPath
        {
            get
            {
                return "Data/JSON/" + ConfigurationManager.AppSettings.Get("InspectionProcedureJsonPath");
            }
        }

        private string InspectionProcedureAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("InspectionProcedureAPIUrl");
            }
        }

        private string PlexorInformationJsonPath
        {
            get
            {
                return "Data/JSON/" + ConfigurationManager.AppSettings.Get("PlexorInformationJsonPath");
            }
        }

        private void CreateFileIfNotExist(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
        }

        private async Task<string> StorePRSInformation()
        {
            try
            {
                CreateFileIfNotExist(StationInformationJsonPath);
                string prsData = await _requestHandler.GetAsyncString(GetBaseUrl + StationInformationAPIUrl, _reqHeaderWithToken);

                using (StreamWriter writer = new StreamWriter(StationInformationJsonPath))
                {
                    writer.WriteLine(prsData);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> StoreInspectionProcedure()
        {
            try
            {
                CreateFileIfNotExist(InspectionProcedureJsonPath);

                string ipData = await _requestHandler.GetAsyncString(GetBaseUrl + InspectionProcedureAPIUrl, _reqHeaderWithToken);

                using (StreamWriter writer = new StreamWriter(InspectionProcedureJsonPath))
                {
                    writer.WriteLine(ipData);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> StorePlexorDeviceInfo()
        {
            try
            {
                CreateFileIfNotExist(PlexorInformationJsonPath);

                string plexorData = await _requestHandler.GetAsyncString(GetBaseUrl + PlexorInformationAPIUrl, _reqHeaderWithToken);

                using (StreamWriter writer = new StreamWriter(PlexorInformationJsonPath))
                {
                    writer.WriteLine(plexorData);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateApplicationSettingsFile(string xmlFilePath)
        {
            string maybeError = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                UnitSection newUnitSection = await GetUnitSection();

                xmlDoc.Load(xmlFilePath);

                XmlNode unitSection = xmlDoc.SelectSingleNode("//Settings[@Section='UNITS']");

                if (unitSection != null)
                {
                    UpdateSetting(unitSection, "UnitLowPressure", newUnitSection.unit_low_pressure.ToString());
                    UpdateSetting(unitSection, "UnitHighPressure", newUnitSection.unit_high_pressure.ToString());
                    UpdateSetting(unitSection, "FactorLowHighPressure", newUnitSection.factor_low_high_pressure.ToString());
                    UpdateSetting(unitSection, "FactorMeasuredChangeRateToMbarMin", newUnitSection.factor_measured_change_rate_to_mbar_min.ToString());
                    UpdateSetting(unitSection, "FactorMbarMinToUnitChangeRate", newUnitSection.factor_mbar_min_to_unit_change_rate.ToString());
                    UpdateSetting(unitSection, "UnitChangeRate", newUnitSection.unit_change_rate.ToString());
                    UpdateSetting(unitSection, "UnitQVSLeakage", newUnitSection.unit_qvs_leakage.ToString());
                    UpdateSetting(unitSection, "FactorQVS", newUnitSection.factor_qvs.ToString());

                    xmlDoc.Save(xmlFilePath);
                }
                else
                {
                    maybeError = "UnitSection not found for the given file";
                }

            }
            catch (Exception ex)
            {
                maybeError = ex.Message;
            }
            return maybeError;
        }

        private async Task<UnitSection> GetUnitSection()
        {
            try
            {
                var unitSection = await _requestHandler.GetAsync<UnitSection>(GetBaseUrl + ConfigurationManager.AppSettings.Get("UnitSectionAPIUrl"), _reqHeaderWithToken);
                return unitSection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void UpdateSetting(XmlNode unitsSection, string settingName, string newValue)
        {
            XmlNode settingNode = unitsSection.SelectSingleNode($"Setting[@name='{settingName}']");
            if (settingNode != null)
            {
                settingNode.Attributes["Value"].Value = newValue;
            }
            else
            {
                // If the setting does not exist, create it
                XmlElement newSettingNode = unitsSection.OwnerDocument.CreateElement("Setting");
                newSettingNode.SetAttribute("name", settingName);
                newSettingNode.SetAttribute("Value", newValue);
                unitsSection.AppendChild(newSettingNode);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SyncService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    internal class UnitSection
    {
        public string unit_low_pressure { get; set; }
        public string unit_high_pressure { get; set; }
        public double factor_low_high_pressure { get; set; }
        public double factor_measured_change_rate_to_mbar_min { get; set; }
        public double factor_mbar_min_to_unit_change_rate { get; set; }
        public string unit_change_rate { get; set; }
        public string unit_qvs_leakage { get; set; }
        public double factor_qvs { get; set; }
    }

    internal class ErrorStringList : List<string>
    {
        public ErrorStringList() { }
        public new bool Add(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                base.Add(name);
                return true;
            }
            return false;
        }

        public string Pop()
        {
            if (base.Count == 0)
            {
                return null;
            }
            return base[0];
        }
    }
}
