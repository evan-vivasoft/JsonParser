using Inspector.POService.LicenseValidator;
using Xunit;

namespace JSONParser.Tests
{
    public class LicenseHelperTest
    {
        private readonly POLicenseValidator _licenseHelper;

        public LicenseHelperTest()
        {
            _licenseHelper = new POLicenseValidator();
        }

        [Fact]
        public void EncryptionAndDecryption()
        {
            //LicenseInfo licenseInfo = new LicenseInfo()
            //{
            //    InspectorPCApiToken = Guid.NewGuid().ToString(),
            //    InspectorPCBaseUrl = "",
            //    InspectorPCCustomerId = Guid.NewGuid().ToString(),
            //    InspectorPCDeviceId = Guid.NewGuid().ToString()
            //};
            //var x = _licenseHelper.EncryptLicensesInfo(licenseInfo);

            //var y = _licenseHelper.DecryptObject<LicenseInfo>(x);

            //Assert.True(AreEqual(licenseInfo, y));
        }

        private bool AreEqual(LicenseInfo x, LicenseInfo y)
        {
            return x != null &&
                   y != null &&
                   x.InspectorPCBaseUrl == y.InspectorPCBaseUrl &&
                   x.InspectorPCCustomerId == y.InspectorPCCustomerId &&
                   x.InspectorPCDeviceId == y.InspectorPCDeviceId;
        }
    }
}
