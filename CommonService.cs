using JSONParser.LicenseHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    public static class CommonService
    {
        public static async Task<string> Login(LoginDto loginData, string baseUrl)
        {
            try
            {
                var httpClient = new HttpClient();
                using (var reqHandler = new RequestHandler.RequestHandler(httpClient))
                {
                    var response = await reqHandler.PostAsync<ApiToken>(baseUrl + ConfigurationManager.AppSettings.Get("LoginUrl"), loginData);

                    return response.token;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    public class LoginDto
    {
        public string device_id { get; set; }
        public string customer_id { get; set; }
    }

    public class ApiToken
    {
        public string token { get; set; }
    }
}
