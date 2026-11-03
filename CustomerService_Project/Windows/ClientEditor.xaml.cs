using CustomerService_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для ClientEditor.xaml
    /// </summary>
    public partial class ClientEditor : Window
    {
        private int? _clientId;

        public ClientEditor()
        {
            InitializeComponent();
        }

        public ClientEditor(Clients client) : this()
        {
            _clientId = client.Id;

            FullNameBox.Text = client.FullName;
            EmailBox.Text = client.Email;
            PhoneNumberBox.Text = client.PhoneNumber;
            RoleBox.SelectedIndex = client.Role == 1 ? 1 : 0;

            foreach (ComboBoxItem item in RoleBox.Items)
            {
                if (Convert.ToInt32(item.Tag) == client.Role)
                {
                    RoleBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string phone = PhoneNumberBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            try
            {
                var mail = new System.Net.Mail.MailAddress(email);

                if (mail.Address != email)
                {
                    MessageBox.Show("Введите корректный Email.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Введите корректный Email.");
                return;
            }

            var selectedRole = RoleBox.SelectedItem as ComboBoxItem;

            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль клиента.");
                return;
            }

            byte role = Convert.ToByte(selectedRole.Tag);

            bool passwordEntered =
                !string.IsNullOrWhiteSpace(PasswordInput.Password) ||
                !string.IsNullOrWhiteSpace(PasswordRepeatInput.Password);

            if (!_clientId.HasValue || passwordEntered)
            {
                if (PasswordInput.Password.Length < 8)
                {
                    MessageBox.Show("Пароль должен содержать минимум 8 символов.");
                    return;
                }

                if (PasswordInput.Password != PasswordRepeatInput.Password)
                {
                    MessageBox.Show("Пароли не совпадают.");
                    return;
                }
            }

            using (var db = new CustomerServiceEntities())
            {
                bool emailExists;

                if (_clientId.HasValue)
                {
                    emailExists = db.Clients.Any(c =>
                        c.Email == email && c.Id != _clientId.Value);
                }
                else
                {
                    emailExists = db.Clients.Any(c => c.Email == email);
                }

                if (emailExists)
                {
                    MessageBox.Show("Клиент с таким Email уже существует.");
                    return;
                }

                Clients client;

                if (_clientId.HasValue)
                {
                    client = db.Clients.FirstOrDefault(c => c.Id == _clientId.Value);

                    if (client == null)
                    {
                        MessageBox.Show("Клиент не найден.");
                        return;
                    }
                }
                else
                {
                    client = new Clients();
                    db.Clients.Add(client);
                }

                client.FullName = fullName;
                client.Email = email;
                client.PhoneNumber = phone;
                client.Role = role;

                if (!_clientId.HasValue || passwordEntered)
                {
                    string salt = PasswordHelper.getSaltPassword();

                    client.PasswordSalt = salt;
                    client.Password = PasswordHelper.getHashPassword(
                        PasswordInput.Password, salt);
                }

                db.SaveChanges();
            }

            MessageBox.Show("Данные клиента успешно сохранены.");
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
