using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Net.Http.Headers;
using System.Configuration;

namespace ClientHash.Services
{
    internal class AuthService
    {
        private readonly HttpClient _client;
        private readonly AesEncryptionService _aesService;

        public AuthService(HttpClient client, AesEncryptionService aesService)
        {
            _client = client;
            _aesService = aesService;
        }

        public async Task<List<string>> LoginUser(string url, string login, string password)
        {
            using HttpClient client = new();

            string hashedPass = GetSHA1Hash(password);

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{hashedPass}"));

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string encryptedResponseBody = await response.Content.ReadAsStringAsync();
                string decryptedResponseBody = _aesService.Decrypt(encryptedResponseBody);
                List<string> permissions = JsonConvert.DeserializeObject<List<string>>(decryptedResponseBody);
                return permissions;
            }
            else
            {
                throw new Exception("Ошибка при входе. Пожалуйста, попробуйте еще раз.");
            }
        }

        public async Task AddUser(string url, string login, string password)
        {
            using HttpClient client = new();

            string hashedPass = GetSHA1Hash(password);

            string encryptedLogin = _aesService.Encrypt(login);
            string encryptedPassword = _aesService.Encrypt(hashedPass);

            var data = new Dictionary<string, string>
            {
                { "login", encryptedLogin },
                { "password", encryptedPassword }
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
