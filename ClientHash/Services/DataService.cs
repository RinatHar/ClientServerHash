using ClientHash.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ClientHash.Services
{
    internal class DataService
    {
        private readonly HttpClient _client;

        public DataService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Data>> GetDataFromServerAsync(string url, string login, string password)
        {

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            // Создаем объект HttpRequestMessage с заголовком Authorization
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // Отправляем запрос без тела
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {url} failed with status code {response.StatusCode}.");
            }

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Data>>(data);
        }

        public async Task AddDataToServerAsync(string url, string value, string login, string password)
        {
            var data = new DataDto { Value = value };
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            // Создаем объект HttpRequestMessage с заголовком Authorization
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = content;

            // Отправляем запрос с телом
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                    throw new HttpRequestException($"Request to {url} failed with status code {response.StatusCode}.");
            }

        }
    }
}
