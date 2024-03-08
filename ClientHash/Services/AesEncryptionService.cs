using System.IO;
using System.Security.Cryptography;


namespace ClientHash.Services
{
    public class AesEncryptionService
    {

        public static byte[] Encrypt(byte[] dataToEncrypt, byte[] Key)
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

        public static byte[] Decrypt(byte[] dataToDecrypt, byte[] Key)
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
