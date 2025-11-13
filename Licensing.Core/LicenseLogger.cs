using System;
using System.IO;

namespace Licensing.Core
{
    /// <summary>
    /// Статический класс для логгирования информации о выданных лицензиях в CSV файл
    /// </summary>
    public static class LicenseLogger
    {
        private const string LogFileName = "licenses_krp.csv";

        /// <summary>
        /// Логирует информацию о выданной лицензии в CSV файл
        /// </summary>
        /// <param name="hardwareId">Hardware ID устройства</param>
        /// <param name="clientName">Имя клиента</param>
        /// <param name="generatedKey">Сгенерированный ключ лицензии</param>
        /// <param name="licenseType">Тип лицензии (Вечная, Триальная, С датой)</param>
        /// <param name="expiryDate">Дата истечения срока действия лицензии</param>
        public static void LogLicense(string hardwareId, string clientName, string generatedKey, string licenseType, DateTime expiryDate)
        {
            try
            {
                // Получаем путь к папке с исполняемым файлом
                string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logFilePath = Path.Combine(executableDirectory, LogFileName);

                // Формируем CSV строку с символом ; как разделителем
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string csvLine = $"{timestamp};{hardwareId};{clientName};{generatedKey};{licenseType};{expiryDate:yyyy-MM-dd}";

                // Проверяем, существует ли файл. Если нет - создаем с заголовками
                bool fileExists = File.Exists(logFilePath);
                
                // Записываем данные в файл
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true, encoding: System.Text.Encoding.UTF8))
                {
                    // Если файл новый, добавляем заголовки
                    if (!fileExists)
                    {
                        writer.WriteLine("Timestamp;Hardware ID;Client Name;License Key;License Type;Expiry Date");
                    }
                    
                    // Записываем данные о лицензии
                    writer.WriteLine(csvLine);
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки можно добавить дополнительную обработку
                // Например, запись в системный лог или вывод в консоль
                System.Diagnostics.Debug.WriteLine($"Ошибка при записи в лог лицензий: {ex.Message}");
            }
        }
    }
}