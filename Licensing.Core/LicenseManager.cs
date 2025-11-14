using System;
using System.Security.Cryptography;
using System.Text;

namespace Licensing.Core
{
    public static class LicenseManager
    {
        public static readonly string SecretKey = "MySuperSecretSauceForDokaStudioV2@#2025*";
        public static readonly string KrpPrefix = "KRP_V1_2025";
        public static readonly string QxPrefix = "QXV2_2025";

        public static string GenerateKey(string hardwareId, string licensePrefix, DateTime expiryDate)
        {
            // 1. Проверка: Если HardwareID или Prefix пустые, возвращаем пустую строку
            if (string.IsNullOrEmpty(hardwareId) || string.IsNullOrEmpty(licensePrefix))
            {
                return string.Empty;
            }

            // Формирование Payload: Hardware ID | Префикс | Дата_истечения_срока_действия
            string payload = $"{hardwareId}|{licensePrefix}|{expiryDate:yyyy-MM-dd}";

            // Вычисление HMAC-SHA256, используя SecretKey
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

                // 2. Длина ключа: Форматирование первых 16 байт (32 hex символа)
                string hex = BitConverter.ToString(hash, 0, 16).Replace("-", "").ToUpper();

                // 3. Форматирование: 6 блоков по 5 символов
                return $"{hex.Substring(0, 5)}-{hex.Substring(5, 5)}-{hex.Substring(10, 5)}-{hex.Substring(15, 5)}-{hex.Substring(20, 5)}-{hex.Substring(25, 5)}";
            }
        }
    }
}