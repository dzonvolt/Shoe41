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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
            //ShowCaptcha();
        }

        private void LoginGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            MessageBox.Show("Вы вошли как гость");
        }

        bool NeedCaptcha = false;
        DateTime LastLoginTry = DateTime.MinValue;

        static string GetRandomLetter()
        {
            Random random = new Random();
            int uppercaseIndex = random.Next(0, 26);
            char uppercaseLetter = (char)('A' + uppercaseIndex);
            int lowercaseIndex = random.Next(0, 26);
            char lowercaseLetter = (char)('a' + lowercaseIndex);
            return random.Next(0, 2) == 0 ? uppercaseLetter.ToString() : lowercaseLetter.ToString();
        }

        void ShowCaptcha() {
            captchaOneWord.Text = GetRandomLetter();
            captchaTwoWord.Text = GetRandomLetter();
            captchaThreeWord.Text = GetRandomLetter();
            captchaFourWord.Text = GetRandomLetter();
            CaptchaSP.Visibility = Visibility.Visible;
            CaptchaTB.Visibility = Visibility.Visible;
            NeedCaptcha = true;
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            DateTime dt = DateTime.Now;
            if (dt - LastLoginTry < TimeSpan.FromSeconds(10))
            {
                MessageBox.Show("Подождите 10 секунд");
                return;
            }

            if (NeedCaptcha) {
                string a = CaptchaTB.Text;
                string b = captchaOneWord.Text + captchaTwoWord.Text + captchaThreeWord.Text + captchaFourWord.Text;
                if (a != b) {
                    LastLoginTry = dt;
                    MessageBox.Show("Неверная капча");
                    return; 
                }

                CaptchaSP.Visibility = Visibility.Hidden;
                CaptchaTB.Visibility = Visibility.Hidden;
                NeedCaptcha = false;
            }

           
            string login = LoginTB.Text;
            string password = PasswordTB.Text;
            if (login == "" || password == "") {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Chirkov41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login);
            
            if (user == null) {
                ShowCaptcha();
                MessageBox.Show("Пользователь не найден");
                return;
            }

            if (user.UserPassword != password) {
                ShowCaptcha();
                MessageBox.Show("Неверный пароль");
                return;
            }


            Manager.MainFrame.Navigate(new ProductPage(user));
            MessageBox.Show("Вы успешно вошли в аккаунт");

        }
    }
}
