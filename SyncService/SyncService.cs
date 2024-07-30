using JSONParser.InformationService;
using JSONParser.InspectionResults;
using JSONParser.LicenseHelper;
using JSONParser.StationInformation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace JSONParser.SyncService
{
    public class SyncService
    {
        #region Member Variables
        private LicenseHelper.LicenseHelper _licenseHelper;
        private string _apiToken;
        private HttpClient _httpClient;
        private LicenseInfo _licenseInfo;
        private Dictionary<string, string> _reqHeaderWithToken = new Dictionary<string, string>();
        private string _resultFileStringDir = ConfigurationManager.AppSettings.Get("ResultJsonFilePath");
        private RequestHandler.IRequestHandler _requestHandler;
        #endregion

        public SyncService() 
        {
            _licenseHelper = new LicenseHelper.LicenseHelper();
            _httpClient = new HttpClient();
            _requestHandler = new RequestHandler.RequestHandler(_httpClient);
        }

        public async Task ConnectWithPlexorOnline(Action<string> callBack)
        {
            _licenseInfo = _licenseHelper.GetStoredLicenseInfo();
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
            Exception maybeException = null;
            try
            {
                var storedLicenseInfo = _licenseHelper.GetStoredLicenseInfo();
                var maybeUpdatedLicenseInfo = await GetUpdatedLicenseInfo();
                var isLicenseValid = !IsLicenseActive(maybeUpdatedLicenseInfo);

                UpdateStoredLicenseInfo();
            }
            catch (Exception ex)
            {
                maybeException = ex;
            }
            callBack?.Invoke(maybeException?.Message);
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

        public async Task FetchUpdatedData(Action<string> callBack)
        {
            Exception maybeException = null;
            try
            {
                await StorePRSInformation();
                await StoreInspectionProcedure();
                // TODO: Update Manometer data
                await StorePlexorDeviceInfo();
            }
            catch (Exception ex)
            {
                maybeException = ex;
            }

            callBack?.Invoke(maybeException?.Message);
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

        private bool IsLicenseActive(DeviceStatus deviceStatus)
        {
            return !deviceStatus.is_active || deviceStatus.license_expiry_date.ToUniversalTime() < DateTime.UtcNow;
        }

        private void UpdateStoredLicenseInfo()
        {

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
                return ConfigurationManager.AppSettings.Get("StationInformationJsonPath");
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
                return ConfigurationManager.AppSettings.Get("InspectionProcedureJsonPath");
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
                return ConfigurationManager.AppSettings.Get("PlexorInformationJsonPath");
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
                var currentDir = Assembly.GetExecutingAssembly().Location;
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
        #endregion
    }
}
