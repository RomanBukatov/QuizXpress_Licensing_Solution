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
using System.Linq;
using Licensing.Core;

namespace KeyGenerator.QX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Setup mask for date TextBox
            ExpiryDate_TextBox.PreviewTextInput += DateTextBox_PreviewTextInput;
            ExpiryDate_TextBox.PreviewKeyDown += DateTextBox_PreviewKeyDown;
        }

        private void DateTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Allow only digits
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
                return;
            }

            string currentText = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            // Insert digit
            string newText = currentText.Insert(caretIndex, e.Text);

            // Remove non-digits for counting
            string digitsOnly = new string(newText.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 8)
            {
                e.Handled = true;
                return;
            }

            // Format as dd.mm.yyyy
            string formatted = "";
            if (digitsOnly.Length > 0) formatted += digitsOnly.Substring(0, Math.Min(2, digitsOnly.Length));
            if (digitsOnly.Length > 2) formatted += "." + digitsOnly.Substring(2, Math.Min(2, digitsOnly.Length - 2));
            if (digitsOnly.Length > 4) formatted += "." + digitsOnly.Substring(4, Math.Min(4, digitsOnly.Length - 4));

            textBox.Text = formatted;
            textBox.CaretIndex = formatted.Length;
            e.Handled = true;
        }

        private void DateTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Allow navigation keys
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Home || e.Key == Key.End || e.Key == Key.Tab)
            {
                return;
            }

            // Allow paste with Ctrl+V
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = false;
                return;
            }

            // Allow text input
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                return;
            }

            e.Handled = true;
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

        // Обработчик для изменения типа лицензии
        private void LicenseType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = LicenseType_ComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Content.ToString() == "С датой")
            {
                ExpiryDate_Label.Visibility = Visibility.Visible;
                ExpiryDate_TextBox.Visibility = Visibility.Visible;
            }
            else
            {
                ExpiryDate_Label.Visibility = Visibility.Collapsed;
                ExpiryDate_TextBox.Visibility = Visibility.Collapsed;
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
            string licensePrefix = LicenseManager.QxPrefix;

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
                    if (string.IsNullOrEmpty(ExpiryDate_TextBox.Text))
                    {
                        MessageBox.Show("Ошибка: Выберите дату истечения для лицензии 'С датой'!", "Ошибка валидации",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    if (!DateTime.TryParseExact(ExpiryDate_TextBox.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out expiryDate))
                    {
                        MessageBox.Show("Ошибка: Неверный формат даты. Используйте формат дд.мм.гггг", "Ошибка валидации",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
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
            LicenseLogger_QX.LogLicense(hardwareId, ClientName_TextBox.Text, generatedKey, selectedLicenseType, expiryDate);
        }
    }
}