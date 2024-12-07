using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для NavPanelUser.xaml
    /// </summary>
    public partial class NavPanelUser : Page
    {
        int id;
        public NavPanelUser(int UserId)
        {
            InitializeComponent();
            id = UserId;
            LoadUserBalance();
        }
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True"; // Укажите вашу строку подключения
        private void LoadUserBalance()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Balance FROM [dbo].[User] WHERE Id_User = @Id_User";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id_User", id);
                        var balance = command.ExecuteScalar(); // Получаем значение баланса

                        if (balance != null)
                        {
                            BalanceTextBlock.Text = $"Баланс: {balance.ToString()}";
                        }
                        else
                        {
                            BalanceTextBlock.Text = "Баланс: 0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки баланса: " + ex.Message);
            }
        }

        private void Exibits_ExibitionsPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.Client.ExibitsPageClient(id));
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.Client.ProfilePageClient(id));
        }

        private void NotificationsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.Client.NotflicationsPageClient());
        }

        private void Tickets_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.Client.TicketsPageClient(id));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.LoginPage());
            MainWindow.Instance.FrameNavBar_.Navigate(null);
        }

        private void NotificationsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.Navigate(new Pages.Client.NotflicationsPageClient());
        }
    }
}
