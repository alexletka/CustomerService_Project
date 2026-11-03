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
using static System.Collections.Specialized.BitVector32;

namespace CustomerService_Project.Windows
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            try
            {
                using (var db = new CustomerServiceEntities())
                {
                    var client = db.Clients.FirstOrDefault(c => c.Email == email);

                    if (client == null ||
                        string.IsNullOrWhiteSpace(client.PasswordSalt) ||
                        string.IsNullOrWhiteSpace(client.Password) ||
                        !PasswordHelper.Verify(
                            password,
                            client.PasswordSalt,
                            client.Password))
                    {
                        MessageBox.Show("Неверная электронная почта или пароль.");
                        return;
                    }
                }

                MessageBox.Show("Вход выполнен успешно!");

                CustomerService_Project.MainWindow mainWindow = new CustomerService_Project.MainWindow();

                Application.Current.MainWindow = mainWindow;

                mainWindow.Show();

                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при входе в систему.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();

            registrationWindow.Owner = this;
            registrationWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

