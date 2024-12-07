using Musem.NavPanel;
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
    /// Логика взаимодействия для ExibitsPageClient.xaml
    /// </summary>
    public partial class ExibitsPageClient : Page
    {
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True"; // Укажите вашу строку подключения
        private int userId; // Замените на актуальный ID пользователя

        public ExibitsPageClient(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadExhibitions();
        }

        private void LoadExhibitions()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id_Exhibition, Title, StartDate, EndDate FROM [dbo].[Exhibitions]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        List<Exhibition> exhibitions = new List<Exhibition>();

                        while (reader.Read())
                        {
                            exhibitions.Add(new Exhibition
                            {
                                Id_Exhibition = (int)reader["Id_Exhibition"],
                                Title = reader["Title"].ToString(),
                                StartDate = (DateTime)reader["StartDate"],
                                EndDate = (DateTime)reader["EndDate"],
                                TicketPrice = CalculateTicketPrice((DateTime)reader["StartDate"], (DateTime)reader["EndDate"])
                            });
                        }

                        ExhibitionListView.ItemsSource = exhibitions;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки выставок: " + ex.Message);
            }
        }

        private decimal CalculateTicketPrice(DateTime startDate, DateTime endDate)
        {
            TimeSpan duration = endDate - startDate;
            decimal basePrice = 1000; // Базовая цена билета
            decimal price = basePrice - (decimal)duration.TotalDays * 10; // Уменьшаем цену на 10 за каждый день

            return price < 0 ? 0 : price; // Цена не должна быть отрицательной
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExhibitionListView.SelectedItem is Exhibition selectedExhibition)
            {
                PurchaseTicket(selectedExhibition);
            }
            else
            {
                StatusTextBlock.Text = "Пожалуйста, выберите выставку.";
            }
        }

        private void PurchaseTicket(Exhibition exhibition)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Проверяем, есть ли у пользователя уже купленные билеты
                    string checkTicketQuery = "SELECT COUNT(*) FROM [dbo].[Tickets] WHERE Id_User = @Id_User";
                    using (SqlCommand checkCommand = new SqlCommand(checkTicketQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id_User", userId);
                        int ticketCount = (int)checkCommand.ExecuteScalar();

                        // Устанавливаем цену
                        decimal finalPrice = exhibition.TicketPrice;

                        // Если это первая покупка, применяем скидку 30%
                        if (ticketCount == 0)
                        {
                            finalPrice *= 0.7m; // 30% скидка
                        }

                        // Проверяем достаточно ли у пользователя средств
                        string balanceQuery = "SELECT Balance FROM [dbo].[User] WHERE Id_User = @Id_User";
                        using (SqlCommand balanceCommand = new SqlCommand(balanceQuery, connection))
                        {
                            balanceCommand.Parameters.AddWithValue("@Id_User", userId);
                            decimal balance = (decimal)balanceCommand.ExecuteScalar();

                            if (balance < finalPrice)
                            {
                                StatusTextBlock.Text = "Недостаточно средств для покупки билета.";
                                return; // Выход из метода, если средств недостаточно
                            }

                            // Обновляем баланс пользователя
                            string updateBalanceQuery = "UPDATE [dbo].[User] SET Balance = Balance - @NewBalance WHERE Id_User = @Id_User";
                            using (SqlCommand updateCommand = new SqlCommand(updateBalanceQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@NewBalance", finalPrice);
                                updateCommand.Parameters.AddWithValue("@Id_User", userId);
                                updateCommand.ExecuteNonQuery();
                            }
                        }

                        // Вставляем данные о покупке билета
                        string purchaseQuery = "INSERT INTO [dbo].[Tickets] (Id_Exhibition, PurchaseDate, Price, Id_User) VALUES (@Id_Exhibition, @PurchaseDate, @Price, @Id_User)";
                        using (SqlCommand purchaseCommand = new SqlCommand(purchaseQuery, connection))
                        {
                            purchaseCommand.Parameters.AddWithValue("@Id_Exhibition", exhibition.Id_Exhibition);
                            purchaseCommand.Parameters.AddWithValue("@PurchaseDate", DateTime.Now);
                            purchaseCommand.Parameters.AddWithValue("@Price", finalPrice);
                            purchaseCommand.Parameters.AddWithValue("@Id_User", userId);

                            purchaseCommand.ExecuteNonQuery();
                            StatusTextBlock.Text = "Билет успешно куплен!" + (ticketCount == 0 ? " Вы получили скидку 30%!" : "");
                            LoadExhibitions();
                        }

                        // Обновляем отчет о выставке
                        UpdateReportForExhibition(exhibition.Id_Exhibition, finalPrice);
                        MainWindow.Instance.FrameNavBar_.Navigate(new NavPanelUser(userId));
                    }
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка при покупке билета: " + ex.Message;
            }
        }
        private void UpdateReportForExhibition(int exhibitionId, decimal ticketPrice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Обновляем количество посетителей и выручку в отчете
                    string updateReportQuery = "UPDATE [dbo].[Reports] " +
                                                "SET VisitorCount = VisitorCount + 1, " +
                                                "Revenue = Revenue + @Price " +
                                                "WHERE Id_Exhibition = @Id_Exhibition";

                    using (SqlCommand updateCommand = new SqlCommand(updateReportQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Price", ticketPrice);
                        updateCommand.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        // Если отчет не был найден, можно добавить новый отчет
                        if (rowsAffected == 0)
                        {
                            string insertReportQuery = "INSERT INTO [dbo].[Reports] (ReportDate, VisitorCount, Revenue, Id_Exhibition) " +
                                                        "VALUES (@ReportDate, 1, @Price, @Id_Exhibition)";
                            using (SqlCommand insertCommand = new SqlCommand(insertReportQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@ReportDate", DateTime.Now);
                                insertCommand.Parameters.AddWithValue("@Price", ticketPrice);
                                insertCommand.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка при обновлении отчета: " + ex.Message;
            }
        }


        public class Exhibition
        {
            public int Id_Exhibition { get; set; }
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TicketPrice { get; set; }
        }
    }
}

