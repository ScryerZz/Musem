using Musem.NavPanel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void buttLog_Click(object sender, RoutedEventArgs e)
        {
            string login = txtName.Text;
            var USER = Database.DbConn.DbConnect.User.FirstOrDefault(name => name.Username_User == login);

            if (USER == null)
            {
                MessageBox.Show("Пользователя с такой почтой не существует!");
            }
            else if (USER.Password != txtPass.Text)
            {
                MessageBox.Show("Неправильный пароль!");
            }
            else
            {
                if (USER.Role == "Админ")
                {
                    MainWindow.Instance.FrameNavBar_.Navigate(new NavPanelAdmin(USER.Id_User));
                    MainWindow.Instance.FrameMain.Navigate(new Admin.ExibitsAdminPage());
                    USER.LastLogin = DateTime.Now;
                    Database.DbConn.DbConnect.SaveChanges();
                }
                else if (USER.Role == "Клиент")
                {
                    MainWindow.Instance.FrameNavBar_.Navigate(new NavPanelUser(USER.Id_User));
                    MainWindow.Instance.FrameMain.Navigate(new Client.ExibitsPageClient(USER.Id_User));
                    USER.LastLogin = DateTime.Now;
                    Database.DbConn.DbConnect.SaveChanges();
                }
            }
        }

        private void buttLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new RegistrationPage());
        }
    }
}
