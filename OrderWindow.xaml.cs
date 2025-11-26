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

namespace Shoe41
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string clientName)
        {
            InitializeComponent();
            var currentPickups = Chirkov41Entities.GetContext().PickUpPoint.ToList();
            PickUpPointCB.ItemsSource = currentPickups;

            ClientNameTB.Text = clientName;
            OrderIdTB.Text = selectedOrderProducts.First().OrderID.ToString();

            ShoeListView.ItemsSource = selectedProducts;
            foreach (var p in selectedProducts) {
                p.Quantity = 1;
                foreach (var q in selectedOrderProducts) {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.Quantity = q.OrderProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;

            bool allInStock = true;
            foreach (var product in selectedProducts) {
                if (product.ProductQuantityInStock <= 3) allInStock = false;
            }

            currentOrder.OrderID = Chirkov41Entities.GetContext().Order.ToList().Last().OrderID + 1;
            currentOrder.OrderCode = Chirkov41Entities.GetContext().Order.ToList().Last().OrderCode + 1;
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderDate = DateTime.Now;
            currentOrder.OrderDeliveryDate = currentOrder.OrderDate + (allInStock ? TimeSpan.FromDays(3) : TimeSpan.FromDays(6));

            OrderDP.Text = currentOrder.OrderDate.ToString();
            DeliveryDP.Text = currentOrder.OrderDeliveryDate.ToString();
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            selectedOP.OrderProductCount++;
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            if (prod.Quantity <= 0) return;
            prod.Quantity--;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            selectedOP.OrderProductCount++;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(PickUpPointCB.SelectedIndex == -1)
            {
                MessageBox.Show("укажите пункт выдачи");
                return;
            }

            currentOrder.OrderPickupPoint = (PickUpPointCB.SelectedItem as PickUpPoint).PickUpPointID;

            Chirkov41Entities.GetContext().Order.Add(currentOrder);
            foreach (var item in selectedOrderProducts)
                Chirkov41Entities.GetContext().OrderProduct.Add(item);
            try
            {
                Chirkov41Entities.GetContext().SaveChanges();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            MessageBox.Show("Заказ успешно создан");
        }
    }
}
