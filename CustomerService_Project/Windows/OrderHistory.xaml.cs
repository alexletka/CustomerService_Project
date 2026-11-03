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
    /// Логика взаимодействия для OrderHistory.xaml
    /// </summary>
    public partial class OrderHistory : Window
    {
        public OrderHistory(Clients client)
        {
            InitializeComponent();

            ClientNameText.Text = client.FullName;

            LoadOrderHistory(client.Id);
        }

        private void LoadOrderHistory(int clientId)
        {
            using (var db = new CustomerServiceEntities())
            {
                OrdersHistoryDataGrid.ItemsSource = db.Orders
                    .Where(o => o.ClientId == clientId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersHistoryDataGrid.SelectedItem is Orders selectedOrder)
            {
                OrderDetails order_details = new OrderDetails(selectedOrder.Id);

                order_details.Owner = this;
                order_details.ShowDialog();

            }
            else
            {
                MessageBox.Show("Выберите заказ для просмотра.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
