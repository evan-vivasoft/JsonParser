using JSONParser.InspectionProcedure;
using JSONParser.RequestHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using JSONParser.LicenseHelper;

namespace JSONParser.Tests
{
    public class LicenseHelperTest
    {
        private readonly LicenseHelper.LicenseHelper _licenseHelper;

        public LicenseHelperTest()
        {
            _licenseHelper = new LicenseHelper.LicenseHelper();
        }

        [Fact]
        public void EncryptionAndDecryption()
        {
            LicenseInfo licenseInfo = new LicenseInfo()
            {
                InspectorPCApiToken = Guid.NewGuid().ToString(),
                InspectorPCBaseUrl = "",
                InspectorPCCustomerId = Guid.NewGuid().ToString(),
                InspectorPCDeviceId = Guid.NewGuid().ToString()
            };
            var x = _licenseHelper.EncryptLicensesInfo(licenseInfo);

            var y = _licenseHelper.DecryptObject<LicenseInfo>(x);

            Assert.True(AreEqual(licenseInfo, y));
        }

        private bool AreEqual(LicenseInfo x, LicenseInfo y)
        {
            return x != null &&
                   y != null &&
                   x.InspectorPCApiToken == y.InspectorPCApiToken &&
                   x.InspectorPCBaseUrl == y.InspectorPCBaseUrl &&
                   x.InspectorPCCustomerId == y.InspectorPCCustomerId &&
                   x.InspectorPCDeviceId == y.InspectorPCDeviceId;
        }
    }
}
