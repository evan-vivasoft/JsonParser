using Autofac;
using JSONParser.InformationService;
using JSONParser.RequestHandler;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace JSONParser.LicenseHelper
{
    public class LicenseHelper
    {
        #region Member variables

        private JsonElement _jsonElement;
        private Dictionary<string, string> _reqHeader = new Dictionary<string, string>();
        private string _apiToken = "";
        private string _deviceId;
        private Dictionary<string, string> _reqHeaderWithToken = new Dictionary<string, string>();
        private HttpClient _httpClient = new HttpClient();
        #endregion
        
        #region Getters
        private string DeviceKey
        {
            get
            {
                return _jsonElement.GetProperty("device_key").GetString();
            }
        }

        private string CustomerId
        {
            get
            {
                return _jsonElement.GetProperty("customer_id").GetString();
            }
        }

        private string BaseUrl
        {
            get
            {
                return _jsonElement.GetProperty("base_url").GetString();
            }
        }

        private string VerificationUrl
        {
            get
            {
                return _jsonElement.GetProperty("verification_url").GetString();
            }
        }

        private string FullUrl
        {
            get
            {
                return BaseUrl.TrimEnd('/') + ":8000/" + VerificationUrl.TrimStart('/');
            }
        }
        #endregion
        
        #region Helper Methods
        private void CheckExpiryDate()
        {
            var expiry = _jsonElement.GetProperty("exp").GetInt64();
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiry).UtcDateTime;

            if (DateTime.UtcNow > expiryDate)
            {
                throw new InvalidOperationException("Token expired");
            }
        }

        private string GetPayLoadFromToken(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("The token should consist of three parts separated by dots.");
            }

            return DecodeBase64Url(parts[1]);
        }

        private string DecodeBase64Url(string input)
        {
            string output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            var byteArray = Convert.FromBase64String(output);
            return Encoding.UTF8.GetString(byteArray);
        }

        private void InitJsonElement(string token)
        {
            var jsonDocument = JsonDocument.Parse(token);
            _jsonElement = jsonDocument.RootElement;
        }

        private async Task<string> GetDeviceID(string token)
        {
            try
            {
                using (var reqHanlder = new RequestHandler.RequestHandler(_httpClient))
                {
                    VerificationData data = new VerificationData
                    {
                        customer_id = CustomerId,
                        device_key = DeviceKey,
                    };

                    _reqHeader.Add("verification-token", token);

                    var response = await reqHanlder.PostAsync<DeviceId>(FullUrl, data, _reqHeader);
                    return response.device_id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task MaybeFetchAndStoreInformationJson()
        {
            try
            {
                using (IInformationService informationService = new InformationService.InformationService())
                {
                    await informationService.StoreInspectionProcedure(BaseUrl, _reqHeaderWithToken);
                    await informationService.StoreStationInformation(BaseUrl, _reqHeaderWithToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> LoginUsingCredentials(string customerId, string deviceId)
        {
            try
            {
                LoginDto data = new LoginDto
                {
                    customer_id = customerId,
                    device_id = deviceId
                };

                using (var reqHandler = new RequestHandler.RequestHandler(_httpClient))
                {
                    var response = await reqHandler.PostAsync<ApiToken>(BaseUrl.TrimEnd('/') + ":8000/" + ConfigurationManager.AppSettings.Get("LoginUrl"), data);

                    return response.token;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private T DecryptObject<T>(string cipherText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(ConfigurationManager.AppSettings.Get("EncryptionKey"));
                    aes.IV = Convert.FromBase64String(ConfigurationManager.AppSettings.Get("EncryptionIv"));

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            return (T)formatter.Deserialize(cs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<DeviceStatus> GetDeviceStatus ()
        {
            try
            {
                using (var reqHandler = new RequestHandler.RequestHandler(_httpClient))
                {
                    DeviceStatus deviceStatus = await reqHandler.GetAsync<DeviceStatus>(BaseUrl.TrimEnd('/') + ":8000/" + ConfigurationManager.AppSettings.Get("DeviceStatusUrl"), _reqHeaderWithToken);
                    return deviceStatus;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}; Source: GetDeviceStatus()");
            }
        }

        private void StoreLicenseInformationToRegistry()
        {
            try
            {
                Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("LicenseInfo"), EncryptLicensesInfo());
                //Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("InspectorPCBaseUrl"), BaseUrl);
                //Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("InspectorPCCustomerId"), CustomerId);
                //Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("InspectorPCDeviceId"), this._deviceId);
                //Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("InspectorPCApiToken"), this._apiToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while saving license info in registry. {ex.Message}");
            }
        }

        private string EncryptLicensesInfo()
        {
            LicenseInfo licenseInfo = new LicenseInfo()
            {
                InspectorPCBaseUrl = BaseUrl,
                InspectorPCCustomerId = CustomerId.ToString(),
                InspectorPCDeviceId = this._deviceId.ToString(),
                InspectorPCApiToken = this._apiToken
            };

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes passwordBytes = new Rfc2898DeriveBytes(ConfigurationManager.AppSettings.Get("EncryptionKey"), 20);
                aes.Key = passwordBytes.GetBytes(32);
                aes.IV = passwordBytes.GetBytes(16);

                ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);

                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(cs, licenseInfo);
                    }

                    var cipher = Convert.ToBase64String(ms.ToArray());
                    return cipher;
                }
            }
        }
        #endregion
        
        #region Public Function
        public async void ProcessVerificationToken(string token, Action<string,string> callBack)
        {
            try
            {
                InitJsonElement(GetPayLoadFromToken(token));
                CheckExpiryDate();

                this._deviceId = await GetDeviceID(token);
                this._apiToken = await LoginUsingCredentials(CustomerId, this._deviceId);
                _reqHeaderWithToken.Add("token", this._apiToken);


                // Get device status
                DeviceStatus deviceStatus = await GetDeviceStatus();
                var licenseExpiredIn = (deviceStatus.license_expiry_date - DateTime.Now).Days;
                licenseExpiredIn = licenseExpiredIn < 0 ? 0 : licenseExpiredIn;

                // Store neccessary information to the registry
                this.StoreLicenseInformationToRegistry();
                // Call this api after customer clicks on `Sync` button
                //await MaybeFetchAndStoreInformationJson();
                callBack?.Invoke(deviceStatus.license_status, licenseExpiredIn.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error decoding token: " + ex.Message);
            }
        }

        public LicenseInfo GetLicenseInfo()
        {
            var licenseCipher = Registry.GetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("LicenseInfo"), null) as string;

            if (licenseCipher == null)
            {
                return new LicenseInfo();
            }

            return this.DecryptObject<LicenseInfo>(licenseCipher);
        }
        #endregion
    }

    internal class VerificationData
    {
        public string customer_id { get; set; }
        public string device_key { get; set; }
    }
    internal class DeviceId
    {
        public string device_id { get; set; }
    }

    internal class  ApiToken
    {
        public string token { get; set; }
    }

    internal class LoginDto
    {
        public string device_id { get; set; }
        public string customer_id { get; set; }
    }

    internal class DeviceStatus
    {
        public bool is_active { get; set; }
        public bool is_verified { get; set; }
        public string license_status { get; set; }
        public DateTime license_expiry_date { get; set; }
    }

    [Serializable]
    public class LicenseInfo
    {
        public string InspectorPCApiToken { get; set; }
        public string InspectorPCDeviceId { get; set; }
        public string InspectorPCCustomerId { get; set; }
        public string InspectorPCBaseUrl { get; set; }
        public string LicenseStatus { get; set; } = "";
    }
}
