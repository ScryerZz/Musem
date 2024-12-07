using Musem.Pages.Client.components;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Musem.Pages.Client
{
    /// <summary>
    /// Логика взаимодействия для TicketsPageClient.xaml
    /// </summary>
    public partial class TicketsPageClient : Page
    {
        public class Ticket
        {
            public int Id { get; set; }
            public string ExhibitionTitle { get; set; }
            public DateTime Date { get; set; }
            public decimal Price { get; set; }
        }

        int _id;
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True"; // Укажите вашу строку подключения

        public TicketsPageClient(int id)
        {
            InitializeComponent();
            _id = id;
            var tickets = GetTicketsForUser(_id);
            ReportsListView.ItemsSource = tickets;
        }

        private List<Ticket> GetTicketsForUser(int userId)
        {
            var tickets = new List<Ticket>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT t.Id_Ticket, e.Title AS ExhibitionTitle, t.PurchaseDate, t.Price " +
                               "FROM Tickets t " +
                               "JOIN Exhibitions e ON t.Id_Exhibition = e.Id_Exhibition " +
                               "WHERE t.Id_User = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new Ticket
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id_Ticket")),
                                ExhibitionTitle = reader.GetString(reader.GetOrdinal("ExhibitionTitle")),
                                Date = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }

            return tickets;
        }
        private void ReportsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedTicket = ReportsListView.SelectedItem as Ticket; // Замените YourTicketClass на ваш реальный класс

            if (selectedTicket != null)
            {
                // Получаем ID выбранного билета
                int ticketId = selectedTicket.Id;

                // Создаем экземпляр окна TicketWindow с переданным ID
                TicketWindow ticketWindow = new TicketWindow(ticketId); // Предполагается, что у вас есть конструктор TicketWindow(int ticketId)
                ticketWindow.Show(); // Открываем окно
            }
        }


    }
}