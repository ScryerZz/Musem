using Musem.Pages;
using Musem.Pages.Admin;
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

namespace Musem.NavPanel
{
    /// <summary>
    /// Логика взаимодействия для NavPanelAdmin.xaml
    /// </summary>
    public partial class NavPanelAdmin : Page
    {
        public NavPanelAdmin(int Id_User)
        {
            InitializeComponent();
        }

        private void Exibits_Exibitions_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new ExibitsAdminPage());
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new ReportsAdminPage());
        }

        private void Exhibitions_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new ExhibitionsAdminPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new RegistrationPage());
            MainWindow.Instance.FrameNavBar_.Navigate(null);
        }

        private void Exibits_ExibitionsPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Exibits_ExibitionAdminPage());
        }

        private void Usersfsdfsd_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new UsersAdminPage());
        }
    }
}
