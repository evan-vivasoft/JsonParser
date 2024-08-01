using Inspector.POService.InspectionProcedure;
using Inspector.POService.RequestHandler;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace JSONParser.Tests
{
    public class RequestHandlerTest
    {
        private readonly IRequestHandler _requestHandler;
        private readonly HttpClient _httpClient;

        public RequestHandlerTest()
        {
            _httpClient = new HttpClient();
            _requestHandler = new RequestHandler(_httpClient);
        }

        [Fact]
        public async Task GetAsync_ValidUrl_ReturnsDeserializedObject()
        {
            string url = ConfigurationManager.AppSettings.Get("ApiBaseUrl") + "inspector-device/station-information";
            var result = await _requestHandler.GetAsync<List<InspectionProcedureEntityJsonParserProject>>(url);
            Assert.NotNull(result);
        }
    }
}
