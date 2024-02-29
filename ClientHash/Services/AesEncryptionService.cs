using System.Configuration;
using System.IO;
using System.Security.Cryptography;


namespace ClientHash.Services
{
    public class AesEncryptionService
    {
        private readonly string base64Key;
        private readonly byte[] fullKey;
        private readonly byte[] Key;

        public AesEncryptionService()
        {
            base64Key = ConfigurationManager.AppSettings["Base64Key"];
            fullKey = Convert.FromBase64String(base64Key);
            Key = fullKey.Take(32).ToArray();
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var iv = aes.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            var plaintext = string.Empty;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }
    }

}
