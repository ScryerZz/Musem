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

namespace Musem.Pages.Client
{
    /// <summary>
    /// Логика взаимодействия для NotflicationsPageClient.xaml
    /// </summary>
    public partial class NotflicationsPageClient : Page
    {
        public class Notification
        {
            public int Id_Notification { get; set; }
            public string Text { get; set; }
        }
        public NotflicationsPageClient()
        {
            InitializeComponent();
            LoadNotifications();
        }
        private void LoadNotifications()
        {
            List<Notification> notifications = new List<Notification>();

            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id_Notification, Text FROM Notifications"; // Запрос для получения данных
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        notifications.Add(new Notification
                        {
                            Id_Notification = reader.GetInt32(0), // Получаем Id_Notification
                            Text = reader.GetString(1) // Получаем Text
                        });
                    }
                }
            }

            Notifications.ItemsSource = notifications; // Установка источника данных для ListView
        }
    }
}
