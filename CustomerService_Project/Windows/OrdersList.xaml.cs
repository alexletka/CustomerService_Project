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
    /// Логика взаимодействия для OrdersList.xaml
    /// </summary>
    public partial class OrdersList : UserControl
    {
        public OrdersList()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            using (var db = new CustomerServiceEntities())
            {
                OrdersDataGrid.ItemsSource = db.Orders.ToList();
            }
        }

        private void OrderDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Orders selectedOrder)
            {
                OrderDetails detailsWindow = new OrderDetails(selectedOrder.Id);

                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(
                    "Выберите заказ из таблицы.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Orders selectedOrder)
            {
                var result = MessageBox.Show(
                    "Вы действительно хотите удалить заказ?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new CustomerServiceEntities())
                    {
                        var details = db.OrderDetails
                            .Where(od => od.OrderId == selectedOrder.Id)
                            .ToList();

                        db.OrderDetails.RemoveRange(details);

                        var order = db.Orders.FirstOrDefault(o => o.Id == selectedOrder.Id);

                        if (order != null)
                        {
                            db.Orders.Remove(order);
                            db.SaveChanges();

                            LoadOrders();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOrder window = new CreateOrder();
            window.Owner = Window.GetWindow(this);

            if (window.ShowDialog() == true)
            {
                LoadOrders();

                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    mainWindow.RefreshProducts();
                }
            }
        }
    }
}
