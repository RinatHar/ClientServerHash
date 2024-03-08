using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Net.Http.Headers;
using ClientHash.Handlers;

namespace ClientHash.Services
{
    internal class AuthService
    {
        private readonly HttpClient _client;

        public AuthService()
        {
            _client = new HttpClient(new EncryptionHandler());
        }

        public async Task<string[]> LoginUser(string url, string login, string password)
        {
            string hashedPass = GetSHA1Hash(password);

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{hashedPass}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            else
            {
                throw new Exception("Ошибка при входе. Пожалуйста, попробуйте еще раз.");
            }
        }

        public async Task AddUser(string url, string login, string password)
        {
            string hashedPass = GetSHA1Hash(password);

            var data = new Dictionary<string, string>
            {
                { "login", login },
                { "password", hashedPass }
    };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Регистрация прошла успешно.");
            }
            else
            {
                throw new Exception("Ошибка при регистрации. Пожалуйста, попробуйте еще раз.");
            }
        }

        public static string GetSHA1Hash(string input)
        {
            byte[] hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}
