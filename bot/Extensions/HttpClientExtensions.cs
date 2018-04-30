using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Financial.Bot.Extensions
{
    public static class HttpClientExtensions
    {
        private const string JsonMimeType = "application/json";

        public static async Task<T> GetJsonObjectAsync<T>(this HttpClient httpClient, string requestUri)
        {
            using (var response = await httpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }
            }

            return default(T);
        }

        public static async Task<Uri> PostObjectAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            Uri locationUri = null;

            var json = JsonConvert.SerializeObject(value);
            var content = new StringContent(json, Encoding.UTF8, JsonMimeType);

            using (var response = await httpClient.PostAsync(requestUri, content))
            {
                response.EnsureSuccessStatusCode();
                locationUri = response.Headers.Location;
            }

            return locationUri;
        }
    }
}