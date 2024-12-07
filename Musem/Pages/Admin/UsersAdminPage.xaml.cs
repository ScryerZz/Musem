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

namespace Musem.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для UsersAdminPage.xaml
    /// </summary>
    public partial class UsersAdminPage : Page
    {
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True"; // Замените на вашу строку подключения

        public UsersAdminPage()
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
                string query = "SELECT Id_User, Username_User, Email, CreatedDate, LastLogin, Role, DiscountCode, Balance FROM [User]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id_User = reader.GetInt32(reader.GetOrdinal("Id_User")),
                                Username_User = reader["Username_User"] != DBNull.Value ? reader["Username_User"].ToString() : string.Empty,
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                                Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : string.Empty,
                                DiscountCode = reader["DiscountCode"] != DBNull.Value ? reader["DiscountCode"].ToString() : string.Empty,
                                Balance = reader.IsDBNull(reader.GetOrdinal("Balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Balance"))
                            });
                        }
                    }
                }
            }
            UsersDataGrid.ItemsSource = users; // Заполнение DataGrid пользователями
        }
        private void DeleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                // Проверка, является ли пользователь куратором
                if (selectedUser.Role.Equals("Куратор", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Невозможно удалить пользователя с ролью 'Куратор'.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверка наличия записей в таблице Curator
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkCuratorQuery = "SELECT COUNT(*) FROM Curator WHERE Id_User = @Id_User";
                    using (SqlCommand command = new SqlCommand(checkCuratorQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id_User", selectedUser.Id_User);
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Невозможно удалить пользователя, так как он связан с записями в таблице 'Curator'.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.Username_User}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [User] WHERE Id_User = @Id_User";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id_User", selectedUser.Id_User);
                            command.ExecuteNonQuery();
                        }
                    }

                    LoadUsers(); // Обновляем список пользователей
                    MessageBox.Show("Пользователь удален.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.");
            }
        }
    }

    public class User
    {
        public int Id_User { get; set; }
        public string Username_User { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Role { get; set; }
        public string DiscountCode { get; set; }
        public decimal Balance { get; set; }
    }
}

