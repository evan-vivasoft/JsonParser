using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser.RequestHandler
{
    internal interface IRequestHandler
    {
        Task<T> GetAsync<T>(string url, Dictionary<string, string> maybeHeader = null);
        Task<T> PostAsync<T>(string url, object data, Dictionary<string, string> headers);
        Task<string> GetAsyncString(string url, Dictionary<string, string> maybeHeader = null);
    }
}
