using CustomerService_Project.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomerService_Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void RefreshProducts()
        {
            ProductsListControl.RefreshProducts();
        }

        public void CloseTab(UserControl content)
        {
            TabItem tab = MainTabControl.Items
                .OfType<TabItem>()
                .FirstOrDefault(item => ReferenceEquals(item.Content, content));

            if (tab == null)
                return;

            if (MainTabControl.Items.Count == 1)
            {
                MessageBox.Show(
                    "Нельзя закрыть последнюю вкладку.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            MainTabControl.Items.Remove(tab);
        }
    }
}
