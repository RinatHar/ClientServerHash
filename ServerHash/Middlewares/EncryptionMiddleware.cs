using ServerHash.Services;
using System.Text;

namespace ServerHash.Middlewares
{
    public class EncryptionMiddleware(RequestDelegate next, AesEncryptionService aesService)
    {
        private readonly RequestDelegate _next = next;
        private readonly AesEncryptionService _aesService = aesService;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Content-Encoding") && context.Request.Headers.ContentEncoding.Contains("aes256"))
            {
                var encryptedContent = new MemoryStream();
                await context.Request.Body.CopyToAsync(encryptedContent);
                encryptedContent.Seek(0, SeekOrigin.Begin);

                var decryptedData = _aesService.Decrypt(encryptedContent.ToArray());

                context.Request.Body = new MemoryStream(decryptedData);
                context.Request.Headers.ContentLength = decryptedData.Length;
                context.Request.Headers.Remove("Content-Encoding");
            }

            var originalBodyStream = context.Response.Body;
            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var plainText = await new StreamReader(responseBodyStream).ReadToEndAsync();

                var encryptedData = _aesService.Encrypt(Encoding.UTF8.GetBytes(plainText));

                context.Response.Headers.ContentEncoding = "aes256";
                context.Response.ContentLength = encryptedData.Length;
                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(encryptedData);
            }
        }
    }
}
