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
    /// Логика взаимодействия для OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails : Window
    {
        private readonly int _orderId;

        public OrderDetails(int orderId)
        {
            InitializeComponent();

            _orderId = orderId;

            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            using (var db = new CustomerServiceEntities())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == _orderId);

                if (order == null)
                {
                    MessageBox.Show("Заказ не найден.");
                    Close();
                    return;
                }

                var client = db.Clients.FirstOrDefault(c => c.Id == order.ClientId);

                OrderIdText.Text = order.Id.ToString();
                OrderDateText.Text = order.OrderDate.ToString("dd.MM.yyyy HH:mm");
                ClientNameText.Text = client?.FullName ?? "Не указан";
                TotalAmountText.Text = order.TotalAmount.ToString("N2") + " ₽";

                var details = (from d in db.OrderDetails
                               join p in db.Products
                               on d.ProductId equals p.Id
                               where d.OrderId == _orderId
                               select new
                               {
                                   ProductName = p.Name,
                                   d.Quantity,
                                   d.PricePerUnit,
                                   LineTotal = d.Quantity * d.PricePerUnit
                               }).ToList();

                OrderDetailsDataGrid.ItemsSource = details;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
