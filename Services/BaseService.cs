using MoviePlatformApi.Models;
using MoviePlatformApi.Configuration;
using System.Text.Json;

namespace MoviePlatformApi.Services
{
    public class BaseService
    {
        public Setting Setting;

        public BaseService(Setting setting, IHttpContextAccessor Context)
        {
            Setting = setting;
        }

        public async Task<Movie> Get(string qry)
        {
            var result = await WebHttpClient(qry);
            return result;
        }
        public async Task<Movie> WebHttpClient(string query)
        {
            var client = new HttpClient();
            var url = $"{Setting.BaseUrl}&t={query}&apikey={Setting.OMDbApiKey}";
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Could not authenticate! Status code : {response.StatusCode}");
            }
            // response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Movie>(responseBody);
            return result;
        }

    }
}