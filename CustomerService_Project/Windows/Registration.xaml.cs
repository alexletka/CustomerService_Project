using CustomerService_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomerService_Project.Windows
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            string fullName = FullNameBox.Text.Trim();
            string email = MailBox.Text.Trim();
            string phone = PhoneNumberBox.Text.Trim();

            string password = PasswordInput.Password;

            try
            {
                using (var db = new CustomerServiceEntities())
                {
                    if (db.Clients.Any(c => c.Email == email))
                    {
                        MessageBox.Show("Такая электронная почта уже зарегистрирована.");
                        return;
                    }

                    string salt = PasswordHelper.getSaltPassword();
                    string hash = PasswordHelper.getHashPassword(password, salt);

                    Clients client = new Clients
                    {
                        FullName = fullName,
                        Email = email,
                        Password = hash,
                        PasswordSalt = salt,
                        PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone,
                        Role = 2
                    };

                    db.Clients.Add(client);
                    db.SaveChanges();
                }

                MessageBox.Show(
                    "Регистрация прошла успешно!",
                    "Регистрация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось выполнить регистрацию.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateFields()
        {
            ClearErrors();

            bool isValid = true;

            string fullName = FullNameBox.Text.Trim();
            string email = MailBox.Text.Trim();
            string phone = PhoneNumberBox.Text.Trim();

            string password = PasswordInput.Password;
            string repeatPassword = PasswordRepeatInput.Password;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowError(FullNameErrorText, "Введите ФИО.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError(EmailErrorText, "Введите электронную почту.");
                isValid = false;
            }
            else
            {
                try
                {
                    MailAddress address = new MailAddress(email);

                    if (address.Address != email || email.Contains(" "))
                        throw new FormatException();
                }
                catch
                {
                    ShowError(EmailErrorText, "Введите корректную электронную почту.");
                    isValid = false;
                }
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError(PasswordErrorText, "Введите пароль.");
                isValid = false;
            }
            else if (password.Length < 8)
            {
                ShowError(PasswordErrorText, "Пароль должен содержать минимум 8 символов.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(repeatPassword))
            {
                ShowError(PasswordRepeatErrorText, "Повторите пароль.");
                isValid = false;
            }
            else if (password != repeatPassword)
            {
                ShowError(PasswordRepeatErrorText, "Пароли не совпадают.");
                isValid = false;
            }

            return isValid;
        }

        private void ShowError(TextBlock errorText, string message)
        {
            errorText.Text = message;
            errorText.Visibility = Visibility.Visible;
        }

        private void ClearErrors()
        {
            FullNameErrorText.Visibility = Visibility.Collapsed;
            EmailErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
            PasswordRepeatErrorText.Visibility = Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
