using System;

namespace Inspector.POService.LicenseValidator
{
    public interface IPOLicenseValidator
    {
        void ProcessVerificationToken(string token, Action<string, string> callBack);
        void StoreLicenseInformationToRegistry(LicenseInfo maybeLicenseInfo = null);
        LicenseInfo GetStoredLicenseInfo { get; }
        string ActualDeviceKey { get; }
        bool IsNewUser { get; }
        string GetLicenseStatus { get; }
        string GetCustomerId { get; }
        DateTime GetLicenseExpiryDate { get; }
        string GetBaseUrl { get; }
        string GetDeviceId { get; }
    }
}
