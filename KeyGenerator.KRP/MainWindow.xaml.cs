using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Licensing.Core;

namespace KeyGenerator.KRP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик для кнопки "Копировать ключ"
        private void CopyKey_Button_Click(object sender, RoutedEventArgs e)
        {
            string licenseKey = LicenseKey_TextBox.Text;
            
            // Если поле пустое, ничего не делать
            if (!string.IsNullOrEmpty(licenseKey))
            {
                Clipboard.SetText(licenseKey);
            }
        }

        // Обработчик для кнопки "Сгенерировать"
        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что HardwareID_TextBox не пустой
            string hardwareId = HardwareID_TextBox.Text;
            if (string.IsNullOrEmpty(hardwareId))
            {
                MessageBox.Show("Ошибка: Поле Hardware ID не может быть пустым!", "Ошибка валидации", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Определяем LicensePrefix
            string licensePrefix = LicenseManager.KrpPrefix;

            // Определяем ExpiryDate в зависимости от выбранного типа лицензии
            DateTime expiryDate;
            string selectedLicenseType = LicenseType_ComboBox.Text;

            switch (selectedLicenseType)
            {
                case "Вечная":
                    expiryDate = DateTime.MaxValue;
                    break;
                case "Триальная":
                    expiryDate = DateTime.Now.AddDays(15);
                    break;
                case "С датой":
                    // Для типа "С датой" можно добавить дополнительное поле выбора даты
                    // Пока используем 30 дней как пример
                    expiryDate = DateTime.Now.AddDays(30);
                    break;
                default:
                    expiryDate = DateTime.Now.AddDays(30); // По умолчанию 30 дней
                    break;
            }

            // Генерируем ключ
            string generatedKey = LicenseManager.GenerateKey(hardwareId, licensePrefix, expiryDate);

            // Выводим ключ в текстовое поле
            LicenseKey_TextBox.Text = generatedKey;

            // Логируем информацию о выданной лицензии
            LicenseLogger.LogLicense(hardwareId, ClientName_TextBox.Text, generatedKey, selectedLicenseType, expiryDate);
        }
    }
}