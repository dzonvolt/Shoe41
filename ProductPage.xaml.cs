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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shoe41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public static List<Product> CurrentProducts = new List<Product>();

        List<string> discount_filters = new List<string>{ "0-9%", "10-14%", "15-100%"};

        public ProductPage(User user)
        {
            InitializeComponent();
            CurrentProducts = Chirkov41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = CurrentProducts;
            DiscountCB.ItemsSource = discount_filters;
            if (user != null) UserInfoTB.Text = "Вошлши как " + user.UserSurname + " " + user.UserName + " " + user.UserPatronymic + "\n" + "Роль: " + user.Role.RoleName;
            else UserInfoTB.Text = "Вошли как гость";
                update_products();
        }

        public void update_products() {

            var products = Chirkov41Entities.GetContext().Product.ToList();
            var raw_products_count = products.Count;

            if(SearchTB.Text.Length > 0)
            products = products.Where(p => p.ProductName.ToLower().Contains(SearchTB.Text.ToLower())).ToList();

            if (AscRB.IsChecked.Value)
                products = products.OrderBy(p => p.ProductCost).ToList();
            else if (DescRB.IsChecked.Value)
                products = products.OrderByDescending(p => p.ProductCost).ToList();
            
            if(DiscountCB.SelectedIndex == 0)
                products = products.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount <= 9).ToList();
            else if (DiscountCB.SelectedIndex == 1)
                products = products.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount <= 14).ToList();
            else if (DiscountCB.SelectedIndex == 2)
                products = products.Where(p => p.ProductDiscountAmount >= 15 && p.ProductDiscountAmount <= 100).ToList();

            SearchResultTB.Text = "кол-во " + Convert.ToString(products.Count) + " из " + Convert.ToString(raw_products_count);
            CurrentProducts = products;
            ProductListView.ItemsSource = CurrentProducts;
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void DiscountCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            update_products();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            update_products();
        }

        private void AscRB_Checked(object sender, RoutedEventArgs e)
        {
            update_products();
        }

        private void DescRB_Checked(object sender, RoutedEventArgs e)
        {
            update_products();
        }
    }
}
