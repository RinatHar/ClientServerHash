using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using ClientHash.Services;
using System.Configuration;

namespace ClientHash.Handlers
{
    public class EncryptionHandler : HttpClientHandler
    {
        private readonly string base64Key;
        private readonly byte[] fullKey;
        private readonly byte[] Key;

        public EncryptionHandler()
        {
            base64Key = ConfigurationManager.AppSettings["Base64Key"];
            fullKey = Convert.FromBase64String(base64Key);
            Key = fullKey.Take(32).ToArray();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsByteArrayAsync();
                var encryptedContent = AesEncryptionService.Encrypt(requestContent, Key);
                var encryptedByteArrayContent = new ByteArrayContent(encryptedContent);

                encryptedByteArrayContent.Headers.ContentEncoding.Add("aes256");
                encryptedByteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = encryptedByteArrayContent;
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.Content != null && response.Content.Headers.ContentEncoding.Contains("aes256"))
            {
                var encryptedResponseContent = await response.Content.ReadAsByteArrayAsync();

                var decryptedContent = AesEncryptionService.Decrypt(encryptedResponseContent, Key);

                response.Content = new StringContent(Encoding.UTF8.GetString(decryptedContent), Encoding.UTF8, "application/json");
                response.Content.Headers.ContentEncoding.Remove("aes256");
            }

            return response;
        }
    }
}
