using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inspector.POService.LicenseValidator
{
    public class POLicenseValidator : IPOLicenseValidator
    {
        #region Member variables

        private JsonElement _jsonElement;
        private Dictionary<string, string> _reqHeader = new Dictionary<string, string>();
        private string _apiToken = "";
        private string _deviceId;
        private Dictionary<string, string> _reqHeaderWithToken = new Dictionary<string, string>();
        private HttpClient _httpClient = new HttpClient();
        private string _licenseStatus;
        private DateTime _licenseExpiryDate;
        private LicenseInfo _maybeLicenseInfo;
        #endregion

        #region Public Getters
        // <summary> It gets the actual device key based on machineName and macAddress. We might have to think of it as machine name can be changed anytime</summary>
        public string ActualDeviceKey
        {
            get
            {
                string macAddress =
                    NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => 
                        nic.OperationalStatus == OperationalStatus.Up 
                        && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
                        && nic.GetIPProperties().UnicastAddresses.Count > 0
                        && !nic.Description.ToUpper().Contains("BLUETOOTH")
                    )
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return Environment.MachineName + "-" + macAddress;
            }
        }
        public LicenseInfo GetStoredLicenseInfo
        {
            get
            {
                if (_maybeLicenseInfo is null)
                {
                    var licenseCipher =
                        Environment.Is64BitOperatingSystem
                            ? Registry.GetValue(ConfigurationManager.AppSettings.Get("RegistryPath64Bit"), ConfigurationManager.AppSettings.Get("LicenseInfo"), null) as string
                            : Registry.GetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("LicenseInfo"), null) as string;

                    _maybeLicenseInfo =
                        licenseCipher == null
                            ? new LicenseInfo()
                            : this.DecryptObject<LicenseInfo>(licenseCipher);
                }
                return _maybeLicenseInfo;
            }
        }

        public bool IsNewUser
        {
            get
            {
                var maybeLicense =
                    Environment.Is64BitOperatingSystem
                        ? Registry.GetValue(ConfigurationManager.AppSettings.Get("RegistryPath64Bit"), ConfigurationManager.AppSettings.Get("LicenseInfo"), null)
                        : Registry.GetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("LicenseInfo"), null);
                return maybeLicense == null;
            }
        }

        public string GetLicenseStatus => GetStoredLicenseInfo.LicenseStatus;

        public string GetCustomerId => GetStoredLicenseInfo.InspectorPCCustomerId;

        public DateTime GetLicenseExpiryDate => GetStoredLicenseInfo.LicenseExpiryDate;

        public string GetDeviceId => GetStoredLicenseInfo.InspectorPCDeviceId;

        public string GetBaseUrl => GetStoredLicenseInfo.InspectorPCBaseUrl;
        #endregion

        #region Private getters
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
                return BaseUrl.TrimEnd('/') + "/" + VerificationUrl.TrimStart('/');
            }
        }

        private string EncryptionKey
        {
            get
            {
                return "YWJjZGVmZ2hpa3p7eW91cnN0c3RyaW5nLm9wZW5jb21wbGV4IFtdeQ==";
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

        private T DecryptObject<T>(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Extract the salt from the beginning of the cipher bytes
                byte[] salt = new byte[16];
                Array.Copy(cipherBytes, 0, salt, 0, salt.Length);

                // Derive the key and IV from the password and salt
                var key = new Rfc2898DeriveBytes(EncryptionKey, salt, 10000);
                byte[] keyBytes = key.GetBytes(32);
                byte[] ivBytes = key.GetBytes(16);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(cipherBytes, salt.Length, cipherBytes.Length - salt.Length))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            return (T)formatter.Deserialize(cs);
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new Exception($"Cryptographic error occurred during decryption: {ex.Message}", ex);
            }
            catch (FormatException ex)
            {
                throw new Exception($"Format error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during decryption: {ex.Message}", ex);
            }
        }


        private async Task<DeviceStatus> GetDeviceStatus ()
        {
            try
            {
                using (var reqHandler = new RequestHandler.RequestHandler(_httpClient))
                {
                    DeviceStatus deviceStatus = await reqHandler.GetAsync<DeviceStatus>(BaseUrl.TrimEnd('/') + "/" + ConfigurationManager.AppSettings.Get("DeviceStatusUrl"), _reqHeaderWithToken);
                    return deviceStatus;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}; Source: GetDeviceStatus()");
            }
        }

        private string EncryptLicensesInfo(LicenseInfo maybeLicenseInfo = null)
        {
            LicenseInfo licenseInfo = 
                maybeLicenseInfo ??
                new LicenseInfo()
                {
                    InspectorPCBaseUrl = BaseUrl,
                    InspectorPCCustomerId = CustomerId.ToString(),
                    InspectorPCDeviceId = this._deviceId.ToString(),
                    LicenseStatus = _licenseStatus,
                    LicenseExpiryDate = _licenseExpiryDate
                };

            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Derive a key and IV from the password and salt
            var key = new Rfc2898DeriveBytes(EncryptionKey, salt, 10000);
            byte[] keyBytes = key.GetBytes(32);
            byte[] ivBytes = key.GetBytes(16);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Write the salt to the beginning of the memory stream
                    ms.Write(salt, 0, salt.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(cs, licenseInfo);
                    }

                    return Convert.ToBase64String(ms.ToArray());
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
                if (ActualDeviceKey != DeviceKey)
                {
                    throw new Exception($"Device key didn't match. {ActualDeviceKey}, {DeviceKey}");
                }

                CheckExpiryDate();
                this._deviceId = await GetDeviceID(token);

                var loginDto = new LoginDto()
                {
                    customer_id = CustomerId,
                    device_id = this._deviceId
                };
                this._apiToken = await CommonService.Login(loginDto, BaseUrl.TrimEnd('/') + "/");
                _reqHeaderWithToken.Add("token", this._apiToken);


                // Get device status
                DeviceStatus deviceStatus = await GetDeviceStatus();
                var licenseExpiredIn = (deviceStatus.license_expiry_date - DateTime.Now).Days;
                licenseExpiredIn = licenseExpiredIn < 0 ? 0 : licenseExpiredIn;
                _licenseExpiryDate = deviceStatus.license_expiry_date;
                _licenseStatus = deviceStatus.license_status;


                // Store neccessary information to the registry
                this.StoreLicenseInformationToRegistry();
                callBack?.Invoke(deviceStatus.license_status, licenseExpiredIn.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error decoding token: " + ex.Message);
            }
        }

        public void StoreLicenseInformationToRegistry(LicenseInfo maybeLicenseInfo = null)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath64Bit"), ConfigurationManager.AppSettings.Get("LicenseInfo"), EncryptLicensesInfo(maybeLicenseInfo));
                }
                else
                {
                    Registry.SetValue(ConfigurationManager.AppSettings.Get("RegistryPath"), ConfigurationManager.AppSettings.Get("LicenseInfo"), EncryptLicensesInfo(maybeLicenseInfo));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while saving license info in registry. {ex.Message}");
            }
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
        public string InspectorPCDeviceId { get; set; }
        public string InspectorPCCustomerId { get; set; }
        public string InspectorPCBaseUrl { get; set; }
        public string LicenseStatus { get; set; } = "";
        public DateTime LicenseExpiryDate { get; set; } = DateTime.MinValue;
    }

    public static class LicenseStatuses
    {
        public static string NOT_ACTIVATED = "not_activated";
        public static string VERIFIED = "verified";
        public static string ACTIVE = "active";
        public static string INACTIVE = "inactive";
        public static string LICENSE_ACTIVATED = "license_activated";
        public static string LICENSE_DEACTIVATED = "license_deactivated";
        public static string TO_BE_RENEWED = "to_be_renewed";
        public static string LICENSE_RENEWED = "license_renewed";
    }
}
