using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JSONParser.RequestHandler
{
    internal class RequestHandler: IRequestHandler, IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool disposedValue;

        public RequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> maybeHeader = null)
        {
            try
            {
                var token = ConfigurationManager.AppSettings.Get("AuthToken");
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (maybeHeader != null)
                {
                    foreach (var header in maybeHeader)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex) 
            {
                throw new Exception($"Message: {ex.Message}; Source: {ex.Source}; URL: {url}");
            }
        }

        public async Task<T> PostAsync<T>(string url, object data, Dictionary<string, string> headers = null)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        content.Headers.Add(header.Key, header.Value);
                    }
                }
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                throw new Exception($"Message: {ex.Message}; Source: {ex.Source}; URL: {url}");
            }
        }

        #region Dispose
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
        // ~RequestHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
