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
    /// Логика взаимодействия для ProductEditor.xaml
    /// </summary>
    public partial class ProductEditor : Window
    {
        private int? _productId;

        public ProductEditor()
        {
            InitializeComponent();
        }

        public ProductEditor(Products product) : this()
        {
            _productId = product.Id;

            NameBox.Text = product.Name;
            CategoryBox.Text = product.Category;
            PriceBox.Text = product.Price.ToString();
            StockQuantityBox.Text = product.StockQuantity.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            int quantity;

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(CategoryBox.Text))
            {
                MessageBox.Show("Заполните название и категорию товара.");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            if (!int.TryParse(StockQuantityBox.Text, out quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество товара.");
                return;
            }

            using (var db = new CustomerServiceEntities())
            {
                Products product;

                if (_productId.HasValue)
                {
                    product = db.Products.FirstOrDefault(p => p.Id == _productId.Value);

                    if (product == null)
                    {
                        MessageBox.Show("Товар не найден.");
                        return;
                    }
                }
                else
                {
                    product = new Products();
                    db.Products.Add(product);
                }

                product.Name = NameBox.Text;
                product.Category = CategoryBox.Text;
                product.Price = price;
                product.StockQuantity = quantity;

                db.SaveChanges();
            }

            MessageBox.Show("Товар успешно сохранён.");
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
