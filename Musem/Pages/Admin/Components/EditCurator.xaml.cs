using Musem.Database;
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
    /// Логика взаимодействия для EditCurator.xaml
    /// </summary>
    public partial class EditCurator : Page
    {
        private string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private Curator _curator;
        public class User
        {
            public int Id_User { get; set; }
            public string Username_User { get; set; }
        }
        public EditCurator(int curator)
        {
            InitializeComponent();
            var curatorEx = Database.DbConn.DbConnect.Curator.FirstOrDefault(cur => cur.Id_Curator == curator);
            _curator = curatorEx;

            LoadUsers(); // Загрузка пользователей в ComboBox

            // Установка выбранного пользователя в ComboBox по Id_User
            UserComboBox.SelectedValue = _curator.Id_User;
            ContactInfoTxt.Text = _curator.ContactInfo;
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
            // Проверка на пустые поля
            if (UserComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(ContactInfoTxt.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Обновление данных куратора в базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE [Curator] SET Id_User = @IdUser, ContactInfo = @ContactInfo WHERE Id_Curator = @IdCurator";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdUser", ((User)UserComboBox.SelectedItem).Id_User); // Получаем Id_User из выбранного элемента
                    command.Parameters.AddWithValue("@ContactInfo", ContactInfoTxt.Text);
                    command.Parameters.AddWithValue("@IdCurator", _curator.Id_Curator);
                    command.ExecuteNonQuery(); // Выполнение запроса
                }
            }

            NavigationService.GoBack(); // Возврат на предыдущую страницу
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возврат на предыдущую страницу
        }
    }
}
