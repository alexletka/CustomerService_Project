using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для ClientsList.xaml
    /// </summary>
    public partial class ClientsList : UserControl
    {
        public ClientsList()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadClients();
            }
        }

        private void LoadClients()
        {
            using (var db = new CustomerServiceEntities())
            {
                ClientsDataGrid.ItemsSource = db.Clients.ToList();
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ClientEditor editor = new ClientEditor();
            editor.Owner = Window.GetWindow(this);

            if (editor.ShowDialog() == true)
                LoadClients();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Clients selectedClient)
            {
                ClientEditor editor = new ClientEditor(selectedClient);
                editor.Owner = Window.GetWindow(this);

                if (editor.ShowDialog() == true)
                    LoadClients();
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.");
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Clients selectedClient)
            {
                OrderHistory order_history = new OrderHistory(selectedClient);
                order_history.Owner = Window.GetWindow(this);
                order_history.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите клиента для просмотра истории заказов.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Clients selectedClient)
            {
                using (var db = new CustomerServiceEntities())
                {
                    bool hasOrders = db.Orders.Any(o => o.ClientId == selectedClient.Id);

                    if (hasOrders)
                    {
                        MessageBox.Show(
                            "Нельзя удалить клиента, у которого есть заказы.",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    var result = MessageBox.Show(
                        "Вы действительно хотите удалить клиента?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var client = db.Clients.FirstOrDefault(c => c.Id == selectedClient.Id);

                        if (client != null)
                        {
                            db.Clients.Remove(client);
                            db.SaveChanges();

                            LoadClients();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.");
            }
        }
    } 
}
