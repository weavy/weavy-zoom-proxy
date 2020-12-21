using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Weavy.Zoom.Proxy.Services
{
    public class ProxyRestService : IRestService
    {
        
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public ProxyRestService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["weavy.zoom-client-id"];
            _clientSecret = _configuration["weavy.zoom-client-secret"];
    }

        public async Task<HttpResponseMessage> Post(string uri, string body)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(120);
                string content = JsonConvert.SerializeObject(body);
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Base64Encode($"{_clientId}:{_clientSecret}"));
                request.RequestUri = new Uri(uri);

                using (var response = await httpClient.SendAsync(request))
                {                    
                    return response;
                }
            }
        }

        /// <summary>
        /// Encodes a string to base64
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Base64Encode(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
