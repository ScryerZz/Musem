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
    /// Логика взаимодействия для CreateExhibition.xaml
    /// </summary>
    public partial class CreateExhibition : Page
    {
        public CreateExhibition()
        {
            InitializeComponent();
        }
        public class Exhibition
        {
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
        }

        private void CreateExibitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Сбор данных из текстовых полей и календарей
            string exhibitionName = nameTxt.Text;
            DateTime? startDate = startDateCal.SelectedDate; // Получаем выбранную дату начала
            DateTime? endDate = endDateCal.SelectedDate; // Получаем выбранную дату окончания
            string description = descriptionTxt.Text;

            // Проверка на валидность данных
            if (string.IsNullOrWhiteSpace(exhibitionName))
            {
                MessageBox.Show("Пожалуйста, введите название выставки.");
                return;
            }

            if (!startDate.HasValue || !endDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите даты начала и окончания выставки.");
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала.");
                return;
            }

            // Проверка на уникальность названия
            if (!IsTitleUnique(exhibitionName))
            {
                MessageBox.Show("Выставка с таким названием уже существует. Пожалуйста, выберите другое название.");
                return;
            }

            // Создание объекта выставки
            var newExhibition = new Exhibition
            {
                Name = exhibitionName,
                StartDate = startDate.Value, // Используем Value, чтобы получить DateTime
                EndDate = endDate.Value, // Используем Value, чтобы получить DateTime
                Description = description
            };

            // Сохранение выставки в базе данных
            int exhibitionId = SaveExhibitionToDatabase(newExhibition);

            // Создание отчета с нулевыми данными
            CreateReportForExhibition(exhibitionId);

            // Создание уведомления
            CreateNotificationForExhibition(exhibitionId, newExhibition.StartDate, newExhibition.EndDate, exhibitionName);

            // Вывод сообщения об успешном создании выставки
            MessageBox.Show("Выставка успешно создана!");
        }
        private void CreateNotificationForExhibition(int exhibitionId, DateTime startDate, DateTime endDate, string exhibitionName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Формируем текст уведомления
                    string notificationText = $"С {startDate.ToShortDateString()} по {endDate.ToShortDateString()} пройдет выставка \"{exhibitionName}\". Вас ждут незабываемые эмоции!";

                    string query = "INSERT INTO [dbo].[Notifications] (Text, Id_Exhibition) VALUES (@Text, @Id_Exhibition)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Text", notificationText);
                        command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании уведомления: " + ex.Message);
            }
        }

        private bool IsTitleUnique(string title)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Exhibitions WHERE Title = @Name";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", title);
                    int count = (int)command.ExecuteScalar();

                    return count == 0; // Если count == 0, значит название уникально
                }
            }
        }
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private int SaveExhibitionToDatabase(Exhibition exhibition)
        {
            int newId = 0;

            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO [dbo].[Exhibitions] (Title, StartDate, EndDate, Description) " +
                               "OUTPUT INSERTED.Id_Exhibition VALUES (@Title, @StartDate, @EndDate, @Description)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", exhibition.Name);
                    command.Parameters.AddWithValue("@StartDate", exhibition.StartDate);
                    command.Parameters.AddWithValue("@EndDate", exhibition.EndDate);
                    command.Parameters.AddWithValue("@Description", exhibition.Description);

                    // Получаем идентификатор новой выставки
                    newId = (int)command.ExecuteScalar();
                }
            }

            return newId; // Возвращаем идентификатор новой выставки
        }

        // Метод для создания отчета с нулевыми данными
        private void CreateReportForExhibition(int exhibitionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO [dbo].[Reports] (ReportDate, VisitorCount, Revenue, Id_Exhibition) " +
                                   "VALUES (@ReportDate, @VisitorCount, @Revenue, @Id_Exhibition)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReportDate", DateTime.Now); // Устанавливаем дату отчета на текущее время
                        command.Parameters.AddWithValue("@VisitorCount", 0); // Нулевое количество посетителей
                        command.Parameters.AddWithValue("@Revenue", 0.0m); // Нулевая выручка
                        command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId); // Идентификатор выставки

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Логика возврата на предыдущую страницу или закрытия окна
            MainWindow.Instance.FrameMain.NavigationService.GoBack();
        }
    }
}
