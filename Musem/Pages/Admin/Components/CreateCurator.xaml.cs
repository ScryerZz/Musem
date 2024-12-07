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

namespace Musem.Pages.Admin.Components
{
    /// <summary>
    /// Логика взаимодействия для CreateCurator.xaml
    /// </summary>
    public partial class CreateCurator : Page
    {
        private string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        public class User
        {
            public int Id_User { get; set; }
            public string Username_User { get; set; }
        }
        public CreateCurator()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id_User, Username_User FROM [User]"; // SQL запрос для получения пользователей
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id_User = (int)reader["Id_User"],
                                Username_User = reader["Username_User"].ToString()
                            });
                        }
                    }
                }
            }
            UserComboBox.ItemsSource = users; // Заполнение ComboBox пользователями
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int selectedUserId = (int)UserComboBox.SelectedValue; // Получаем выбранный Id_User
            if (!string.IsNullOrEmpty(ContactInfoTxt.Text))
            {
                // Проверка, является ли пользователь уже куратором
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [Curator] WHERE Id_User = @IdUser"; // Проверка наличия пользователя в таблице Curators
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@IdUser", selectedUserId);
                        int count = (int)checkCommand.ExecuteScalar(); // Получаем количество кураторов с данным Id_User

                        if (count > 0)
                        {
                            MessageBox.Show("Этот пользователь уже является куратором."); // Сообщение об ошибке
                            return; // Завершаем выполнение метода
                        }
                    }

                    // Если пользователь не является куратором, добавляем его в таблицу Curators
                    string insertQuery = "INSERT INTO [Curator] (Id_User, ContactInfo) VALUES (@IdUser, @ContactInfo)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IdUser", selectedUserId);
                        command.Parameters.AddWithValue("@ContactInfo", ContactInfoTxt.Text);
                        command.ExecuteNonQuery(); // Выполнение запроса
                    }
                }
                MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExhibitionsAdminPage()); // Возврат на предыдущую страницу
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите контактную информацию."); // Сообщение об ошибке
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возврат на предыдущую страницу
        }
    }

    public class Users
    {
        public int Id_User { get; set; } // Идентификатор пользователя
        public string Username_User { get; set; } // Имя пользователя
    }

}


