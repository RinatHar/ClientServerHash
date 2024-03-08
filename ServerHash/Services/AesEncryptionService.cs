using System.Security.Cryptography;

namespace ServerHash.Services
{
    public class AesEncryptionService
    {
        private readonly string base64Key;
        private readonly byte[] fullKey;
        private readonly byte[] Key;

        public AesEncryptionService(IConfiguration configuration)
        {
            base64Key = configuration["Base64Key"];
            fullKey = Convert.FromBase64String(base64Key);
            Key = fullKey.Take(32).ToArray();
        }

        public byte[] Encrypt(byte[] dataToEncrypt)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var msEncrypt = new MemoryStream();

            msEncrypt.Write(iv, 0, iv.Length);

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            return msEncrypt.ToArray();
        }

        public byte[] Decrypt(byte[] dataToDecrypt)
        {
            using var aes = Aes.Create();
            aes.Key = Key;

            var iv = new byte[16];
            Array.Copy(dataToDecrypt, 0, iv, 0, iv.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var msDecrypt = new MemoryStream(dataToDecrypt, iv.Length, dataToDecrypt.Length - iv.Length);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            var decryptedData = new List<byte>();
            int readByte;
            while ((readByte = csDecrypt.ReadByte()) != -1)
            {
                decryptedData.Add((byte)readByte);
            }
            return [.. decryptedData];
        }
    }

}
