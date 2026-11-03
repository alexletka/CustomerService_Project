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
    /// Логика взаимодействия для ProductsList.xaml
    /// </summary>
    public partial class ProductsList : UserControl
    {
        public ProductsList()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadProducts();
            }
        }

        public void RefreshProducts()
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new CustomerServiceEntities())
            {
                ProductsDataGrid.ItemsSource = db.Products.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ProductEditor editor = new ProductEditor();
            editor.Owner = Window.GetWindow(this);

            if (editor.ShowDialog() == true)
                LoadProducts();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Products selectedProduct)
            {
                ProductEditor editor = new ProductEditor(selectedProduct);
                editor.Owner = Window.GetWindow(this);

                if (editor.ShowDialog() == true)
                    LoadProducts();
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Products selectedProduct)
            {
                using (var db = new CustomerServiceEntities())
                {
                    bool hasOrders = db.OrderDetails.Any(o => o.ProductId == selectedProduct.Id);

                    if (hasOrders)
                    {
                        MessageBox.Show(
                            "Нельзя удалить товар, который присутствует в заказах.",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    var result = MessageBox.Show(
                        "Вы действительно хотите удалить товар?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var product = db.Products.FirstOrDefault(c => c.Id == selectedProduct.Id);

                        if (product != null)
                        {
                            db.Products.Remove(product);
                            db.SaveChanges();

                            LoadProducts();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchProducts();
        }

        private void SearchProducts()
        {
            string searchText = SearchBox.Text.Trim();

            using (var db = new CustomerServiceEntities())
            {
                var products = db.Products.AsQueryable();

                if (decimal.TryParse(searchText, out decimal price))
                {
                    products = products.Where(p =>
                        p.Name.Contains(searchText) ||
                        p.Category.Contains(searchText) ||
                        p.Price == price);
                }
                else
                {
                    products = products.Where(p =>
                        p.Name.Contains(searchText) ||
                        p.Category.Contains(searchText));
                }

                ProductsDataGrid.ItemsSource = products.ToList();
            }
        }
    }
}

