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
    /// Логика взаимодействия для ProfilePageClient.xaml
    /// </summary>
    public partial class ProfilePageClient : Page
    {

        private string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private int userId; // Замените это значение на актуальный ID пользователя

        public ProfilePageClient(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Username_User, Email, CreatedDate, Balance, DiscountCode FROM [dbo].[User] WHERE Id_User = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        UsernameTextBlock.Text = reader["Username_User"].ToString();
                        EmailTextBlock.Text = reader["Email"].ToString();
                        CreatedDateTextBlock.Text = Convert.ToDateTime(reader["CreatedDate"]).ToString("yyyy-MM-dd");
                        if (reader["Balance"].ToString() == "")
                        {
                            BalanceTextBlock.Text = "0";
                        }
                        else
                        {
                            BalanceTextBlock.Text = reader["Balance"].ToString();
                        }
                        DiscountCodeTextBlock.Text = reader["DiscountCode"] != DBNull.Value ? reader["DiscountCode"].ToString() : "Нет";
                    }
                }
            }
        }

        private void TopUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                TopUpBalance(amount);
                LoadUserData();
            }
            else
            {
                StatusTextBlock.Text = "Введите корректную сумму для пополнения.";
            }
        }

        private void TopUpBalance(decimal amount)
        {
            try
            {
                // Получаем пользователя из базы данных
                var user = Database.DbConn.DbConnect.User.FirstOrDefault(us => us.Id_User == userId);

                // Проверяем, найден ли пользователь
                if (user != null)
                {
                    // Увеличиваем баланс
                    user.Balance += amount;
                    // Сохраняем изменения в базе данных
                    Database.DbConn.DbConnect.SaveChanges();

                    // Обновляем данные на интерфейсе
                    StatusTextBlock.Text = "Баланс успешно пополнен.";
                    LoadUserData(); // Обновляем данные пользователя
                }
                else
                {
                    StatusTextBlock.Text = "Ошибка: пользователь не найден.";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Произошла ошибка: " + ex.Message;
            }
        }
    }
}

