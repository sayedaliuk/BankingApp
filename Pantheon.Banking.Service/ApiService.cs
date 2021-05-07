using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace Pantheon.Banking.Service
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> Get<T>(string url)
        {
            try
            {
                var _client = _httpClientFactory.CreateClient();
                _client.DefaultRequestHeaders.Accept.Clear();
                
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                _client.DefaultRequestHeaders.Accept.Add(contentType);
                
                var task = await _client.GetStringAsync(url);

                var opt = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<T>(task, opt);

                return data;
            }
            catch (Exception ex)
            {
                throw new OperationCanceledException("request failed", ex);
            }
        }
    }
}
