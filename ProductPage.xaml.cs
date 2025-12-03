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
        User CurrentUser = null;
        public static List<Product> CurrentProducts = new List<Product>();
        int NewOrderID = 0;
        List<Product> selectedProducts = new List<Product>();
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<string> discount_filters = new List<string>{ "0-9%", "10-14%", "15-100%"};
        

        public ProductPage(User user)
        {
            InitializeComponent();
            CurrentProducts = Chirkov41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = CurrentProducts;
            DiscountCB.ItemsSource = discount_filters;
            if (user != null) UserInfoTB.Text = "Вошлши как " + user.FullName + "\n" + "Роль: " + user.Role.RoleName;
            else UserInfoTB.Text = "Вошли как гость";
            this.CurrentUser = user;
                UpdateProducts();
            NewOrderID = Chirkov41Entities.GetContext().Order.ToList().LastOrDefault().OrderID + 1;
        }

        public void UpdateProducts() {

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
            UpdateProducts();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void AscRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void DescRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex < 0) return;
            var prod = ProductListView.SelectedItem as Product;
            selectedProducts.Add(prod);

            var newOrderProd = new OrderProduct();
            newOrderProd.OrderID = NewOrderID;
            newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
            newOrderProd.OrderProductCount = 1;

            //var selOP = selectedProducts.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));
            if (selectedOrderProducts.Find(p => p.ProductArticleNumber == prod.ProductArticleNumber) == null)
            {
                selectedOrderProducts.Add(newOrderProd);
            }
            else
            {
                foreach(var p in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == prod.ProductArticleNumber)
                        p.OrderProductCount++;
                }
            }

            OrderBtn.Visibility = Visibility.Visible;
            ProductListView.SelectedIndex = -1;
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();
            var orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, CurrentUser);
            orderWindow.ShowDialog();
        }
    }
}
