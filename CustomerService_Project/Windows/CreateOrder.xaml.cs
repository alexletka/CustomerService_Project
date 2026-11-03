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
    /// Логика взаимодействия для CreateOrder.xaml
    /// </summary>

    public partial class CreateOrder : Window
    {
        private List<OrderItemRow> _orderItems = new List<OrderItemRow>();

        private List<Products> _products = new List<Products>();

        public CreateOrder()
        {
            InitializeComponent();

            LoadData();

            OrderItemsDataGrid.ItemsSource = _orderItems;

            UpdateTotal();
        }

        public class OrderItemRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal PricePerUnit { get; set; }

            public decimal LineTotal => Quantity * PricePerUnit;
        }

        private void LoadData()
        {
            using (var db = new CustomerServiceEntities())
            {
                ClientComboBox.ItemsSource = db.Clients.ToList();

                _products = db.Products.ToList();
                ProductComboBox.ItemsSource = _products;
            }
        }

        private void CheckProductStock()
        {
            if (StockInfoText == null || ProductComboBox == null)
                return;

            StockInfoText.Visibility = Visibility.Collapsed;

            if (!(ProductComboBox.SelectedItem is Products product))
                return;

            if (!int.TryParse(QuantityBox.Text, out int quantity) ||
                quantity <= 0)
                return;

            int alreadyAdded = _orderItems
                .Where(p => p.ProductId == product.Id)
                .Sum(p => p.Quantity);

            int available = product.StockQuantity - alreadyAdded;

            if (quantity > available)
            {
                StockInfoText.Text = $"В наличии {available} штук";
                StockInfoText.Visibility = Visibility.Visible;
            }
        }

        private void QuantityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckProductStock();
        }

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckProductStock();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProductComboBox.SelectedItem is Products product))
            {
                MessageBox.Show("Выберите товар.");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            var existingItem = _orderItems.FirstOrDefault(
                p => p.ProductId == product.Id);

            int alreadyAdded = existingItem == null
                ? 0
                : existingItem.Quantity;

            int available = product.StockQuantity - alreadyAdded;

            if (quantity > available)
            {
                StockInfoText.Text = $"В наличии {available} штук";
                StockInfoText.Visibility = Visibility.Visible;
                return;
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _orderItems.Add(new OrderItemRow
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = quantity,
                    PricePerUnit = product.Price
                });
            }

            RefreshOrderItems();

            QuantityBox.Text = "1";

            CheckProductStock();
        }

        private void RefreshOrderItems()
        {
            OrderItemsDataGrid.Items.Refresh();

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = _orderItems.Sum(p => p.LineTotal);

            TotalAmountText.Text = $"{total:N2} ₽";
        }

        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemsDataGrid.SelectedItem is OrderItemRow selectedItem)
            {
                _orderItems.Remove(selectedItem);

                RefreshOrderItems();

                CheckProductStock();
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ClientComboBox.SelectedItem is Clients client))
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }

            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте товары в заказ.");
                return;
            }

            using (var db = new CustomerServiceEntities())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    List<Products> products = new List<Products>();

                    foreach (var item in _orderItems)
                    {
                        var product = db.Products.FirstOrDefault(
                            p => p.Id == item.ProductId);

                        if (product == null || product.StockQuantity < item.Quantity)
                        {
                            int available = product == null
                                ? 0
                                : product.StockQuantity;

                            StockInfoText.Text = $"В наличии {available} штук";
                            StockInfoText.Visibility = Visibility.Visible;

                            MessageBox.Show(
                                $"Недостаточно товара «{item.ProductName}». " +
                                $"В наличии {available} штук.");

                            transaction.Rollback();
                            return;
                        }

                        products.Add(product);
                    }

                    Orders order = new Orders
                    {
                        ClientId = client.Id,
                        OrderDate = DateTime.Now,
                        TotalAmount = _orderItems.Sum(p => p.LineTotal)
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    for (int i = 0; i < _orderItems.Count; i++)
                    {
                        products[i].StockQuantity -= _orderItems[i].Quantity;

                        CustomerService_Project.OrderDetails detail =
                            new CustomerService_Project.OrderDetails
                            {
                                OrderId = order.Id,
                                ProductId = _orderItems[i].ProductId,
                                Quantity = _orderItems[i].Quantity,
                                PricePerUnit = _orderItems[i].PricePerUnit
                            };

                        db.OrderDetails.Add(detail);
                    }

                    db.SaveChanges();

                    transaction.Commit();

                    MessageBox.Show("Заказ успешно оформлен!");

                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    MessageBox.Show(
                        "Ошибка при оформлении заказа: " + ex.Message,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
