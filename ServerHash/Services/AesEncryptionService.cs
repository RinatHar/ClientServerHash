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
            // Создаем новый экземпляр AES
            using var aes = Aes.Create();
            // Устанавливаем ключ
            aes.Key = Key;
            // Генерируем вектор инициализации
            aes.GenerateIV();
            var iv = aes.IV;

            // Создаем шифратор
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var msEncrypt = new MemoryStream();

            // Записываем вектор инициализации в начало потока
            msEncrypt.Write(iv, 0, iv.Length);

            // Шифруем данные
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            // Возвращаем зашифрованные данные
            return msEncrypt.ToArray();
        }

        public byte[] Decrypt(byte[] dataToDecrypt)
        {
            // Создаем новый экземпляр AES
            using var aes = Aes.Create();
            // Устанавливаем ключ
            aes.Key = Key;

            // Извлекаем вектор инициализации из зашифрованных данных
            var iv = new byte[16];
            Array.Copy(dataToDecrypt, 0, iv, 0, iv.Length);

            // Создаем дешифратор
            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var msDecrypt = new MemoryStream(dataToDecrypt, iv.Length, dataToDecrypt.Length - iv.Length);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            // Дешифруем данные
            var decryptedData = new List<byte>();
            int readByte;
            while ((readByte = csDecrypt.ReadByte()) != -1)
            {
                decryptedData.Add((byte)readByte);
            }
            // Возвращаем дешифрованные данные
            return decryptedData.ToArray();
        }
    }

}
