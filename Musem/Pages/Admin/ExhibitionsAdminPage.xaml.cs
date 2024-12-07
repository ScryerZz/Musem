using Musem.Database;
using Musem.Pages.Admin.Components;
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
using static Musem.Pages.Admin.Components.CreateExhibition;
using static Musem.Pages.Admin.ExibitsAdminPage;

namespace Musem.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для ExhibitionsAdminPage.xaml
    /// </summary>
    public partial class ExhibitionsAdminPage : Page
    {
        public ExhibitionsAdminPage()
        {
            InitializeComponent();
            LoadExhibitions();
            LoadCurators();
        }

        private void CreateAuthor_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new Components.CreateExhibition());
        }
        public class Exhibition
        {
            public int Id_Exhibition { get; set; }
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
        }
        private void LoadExhibitions()
        {
            List<Exhibition> exhibitions = GetExhibitionsFromDatabase();
            ListExibits.ItemsSource = exhibitions; // Устанавливаем источник данных для ListView
        }
        private List<Exhibition> GetExhibitionsFromDatabase()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
            var exhibitions = new List<Exhibition>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Id_Exhibition, Title, StartDate, EndDate, Description FROM Exhibitions";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var exhibition = new Exhibition
                            {
                                Id_Exhibition = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                StartDate = reader.GetDateTime(2),
                                EndDate = reader.GetDateTime(3),
                                Description = reader.GetString(4)
                            };
                            exhibitions.Add(exhibition);
                        }
                    }
                }
            }

            return exhibitions;
        }

        private void DeleteExhibitionBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли выставка
            if (ListExibits.SelectedItem is Exhibition selectedExhibition)
            {
                // Подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту выставку?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем выставку из базы данных
                    DeleteExhibitionFromDatabase(selectedExhibition.Id_Exhibition);

                    // Обновляем список выставок
                    LoadExhibitions();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите выставку для удаления.");
            }
        }
        private bool HasExhibits(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // SQL-запрос для проверки наличия связанных экспонатов
                string query = "SELECT COUNT(*) FROM [dbo].[Exhibit-Exhibition] WHERE Id_Exhibition = @Id_Exhibition";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);
                    int count = (int)command.ExecuteScalar();
                    return count > 0; // Возвращает true, если есть связанные экспонаты, иначе false
                }
            }
        }
        private void DeleteExhibitionFromDatabase(int exhibitionId)
        {
            // Проверяем, есть ли связанные экспонаты
            if (HasExhibits(exhibitionId))
            {
                // Удаляем все связанные записи из таблицы Exhibit-Exhibition
                DeleteExhibitExhibitionLinks(exhibitionId);
            }

            // Удаляем связанные записи из таблицы Reports
            DeleteReportsForExhibition(exhibitionId);

            // Удаляем связанные уведомления
            DeleteNotificationForExhibition(exhibitionId);

            // Удаляем связанные билеты
            DeleteTicketsForExhibition(exhibitionId);

            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Удаляем выставку
                string deleteExhibitionQuery = "DELETE FROM Exhibitions WHERE Id_Exhibition = @Id";
                using (SqlCommand command = new SqlCommand(deleteExhibitionQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", exhibitionId);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Выставка успешно удалена.");
            }
        }

        private void DeleteTicketsForExhibition(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Удаляем связанные билеты
                string deleteTicketsQuery = "DELETE FROM Tickets WHERE Id_Exhibition = @Id";
                using (SqlCommand command = new SqlCommand(deleteTicketsQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", exhibitionId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для удаления связанных записей из таблицы Reports
        private void DeleteReportsForExhibition(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Reports WHERE Id_Exhibition = @Id_Exhibition";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для удаления уведомления о выставке
        private void DeleteNotificationForExhibition(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteNotificationQuery = "DELETE FROM [dbo].[Notifications] WHERE Id_Exhibition = @Id_Exhibition";

                using (SqlCommand command = new SqlCommand(deleteNotificationQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для удаления связей между экспонатами и выставкой
        private void DeleteExhibitExhibitionLinks(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM [dbo].[Exhibit-Exhibition] WHERE Id_Exhibition = @Id_Exhibition";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);
                    command.ExecuteNonQuery();
                }
            }
        }
        private string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private void EditExibit_Click(object sender, RoutedEventArgs e)
        {
            if (ListExibits.SelectedItem is Exhibition selectedExhibition)
            {
                MainWindow.Instance.FrameMain.NavigationService.Navigate(new Components.EdixExhibition(selectedExhibition.Id_Exhibition));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите выставку для редактирования.");
            }
        }

        private void ListExibits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadCurators()
        {
            List<Curators> curators = new List<Curators>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT c.Id_Curator, u.Username_User, c.ContactInfo
            FROM [Curator] c
            JOIN [User] u ON c.Id_User = u.Id_User"; // Соединение таблиц Curators и Users
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            curators.Add(new Curators
                            {
                                Id_Curator = (int)reader["Id_Curator"],
                                Username = reader["Username_User"].ToString(),
                                ContactInfo = reader["ContactInfo"].ToString()
                            });
                        }
                    }
                }
            }
            CuratorsListView.ItemsSource = curators; // Заполнение ListView данными кураторов
        }

        private List<Curator> curators;
        private void AddCuratorBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateCurator());
        }

        private void EditCuratorBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = CuratorsListView.SelectedItem as Curators;
            var curator = Database.DbConn.DbConnect.Curator.FirstOrDefault(cur => cur.Id_Curator == selectedItem.Id_Curator);
            if (CuratorsListView.SelectedItem != null)
            {
                MainWindow.Instance.FrameMain.NavigationService.Navigate(new Components.EditCurator(curator.Id_Curator));
            }
            else
            {
                MessageBox.Show(selectedItem == null ? "Пожалуйста, выберите куратора для редактирования." : "Выбранный элемент не является куратором.");
            }

        }

        private void DeleteCuratorBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, выбран ли куратор для удаления
            if (CuratorsListView.SelectedItem is Curators selectedCurator)
            {
                // Запрос подтверждения удаления
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить куратора {selectedCurator.Username}?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM [Curator] WHERE Id_Curator = @IdCurator"; // SQL-запрос для удаления куратора
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@IdCurator", selectedCurator.Id_Curator); // Параметр для запроса
                            command.ExecuteNonQuery(); // Выполнение запроса
                        }
                    }

                    // Обновление списка кураторов
                    LoadCurators(); // Перезагрузка данных в ListView
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите куратора для удаления."); // Уведомление, если куратор не выбран
            }
        }
        public class Curators
        {
            public int Id_Curator { get; set; }
            public string Username { get; set; }
            public string ContactInfo { get; set; }
        }
        private void DeleteCurator(int idCurator)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Curators WHERE Id_Curator = @Id";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", idCurator);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
