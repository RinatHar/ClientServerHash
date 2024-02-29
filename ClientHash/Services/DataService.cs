using ClientHash.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ClientHash.Services
{
    public class DataService
    {
        private readonly HttpClient _client;
        private readonly AesEncryptionService _aesService;

        public DataService(HttpClient client, AesEncryptionService aesService)
        {
            _client = client;
            _aesService = aesService;
        }

        public async Task<List<DataDto>> GetDataFromServerAsync(string url, string login, string password)
        {
            var response = await SendRequestAsync(url, login, password, HttpMethod.Get);

            var data = await response.Content.ReadAsStringAsync();
            var dataList = JsonConvert.DeserializeObject<List<DataDto>>(data);

            foreach (var item in dataList)
            {
                item.Value = _aesService.Decrypt(item.Value);
            }

            return dataList;
        }

        public async Task AddDataToServerAsync(string url, string value, string login, string password)
        {
            var data = new DataDto
            {
                Value = _aesService.Encrypt(value)
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await SendRequestAsync(url, login, password, HttpMethod.Post, content);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string url, string login, string password, HttpMethod method, HttpContent content = null)
        {

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            // Создаем объект HttpRequestMessage с заголовком Authorization
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = content;

            // Отправляем запрос
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {url} failed with status code {response.StatusCode}.");
            }

            return response;
        }
    }
}
