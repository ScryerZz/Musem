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

namespace Musem.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void buttLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new LoginPage());
        }

        private void buttReg_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "" || txtEmail.Text == "" || txtPass.Text == "")
            {
                MessageBox.Show("Заполните все поля!");
            }
            else
            {
                string LoginClient = txtName.Text;
                var Client = Database.DbConn.DbConnect.User.FirstOrDefault(name => name.Username_User == LoginClient);
                if (Client != null)
                {
                    MessageBox.Show("Клиент с таким логином уже есть!");
                }
                else
                {
                    Random random = new Random();
                    int discountCode;
                    do
                    {
                        discountCode = random.Next(10000, 100000);
                    } while (Database.DbConn.DbConnect.User.Any(u => u.DiscountCode == discountCode));

                    var tempClient = new Database.User()
                    {
                        Username_User = LoginClient,
                        Password = txtPass.Text,
                        Email = txtEmail.Text,
                        Role = "Клиент",
                        CreatedDate = DateTime.Now,
                        DiscountCode = discountCode, 
                        Balance = 0,
                    };

                    Database.DbConn.DbConnect.User.Add(tempClient);
                    Database.DbConn.DbConnect.SaveChanges();
                    MessageBox.Show("Пользоавтель зарегестрирван!");
                    return;
                    
                }
            }
        }
    }
}
