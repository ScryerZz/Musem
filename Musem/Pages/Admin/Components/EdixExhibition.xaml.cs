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
    /// Логика взаимодействия для EdixExhibition.xaml
    /// </summary>
    public partial class EdixExhibition : Page
    {
        public class Exhibition
        {
            public int Id_Exhibition { get; set; }
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
        }
        public class Curator
        {
            public int Id_Curator { get; set; }
            public int Id_User { get; set; }
            public string ContactInfo { get; set; }
        }


        private Exhibition _exhibition; // Измените _exhibitionId на _exhibition

        public EdixExhibition(int exhibitionId)
        {
            InitializeComponent();
            LoadExhibitionData(exhibitionId);
        }

        private void LoadExhibitionData(int exhibitionId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Id_Exhibition, Title, StartDate, EndDate, Description FROM Exhibitions WHERE Id_Exhibition = @Id";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", exhibitionId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Инициализируем _exhibition
                            _exhibition = new Exhibition
                            {
                                Id_Exhibition = exhibitionId,
                                Title = reader["Title"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                Description = reader["Description"].ToString()
                            };

                            // Заполняем поля данными выставки
                            TitleTxt.Text = _exhibition.Title;
                            StartDatePicker.SelectedDate = _exhibition.StartDate;
                            EndDatePicker.SelectedDate = _exhibition.EndDate;
                            DescriptionTxt.Text = _exhibition.Description;
                        }
                        else
                        {
                            MessageBox.Show("Выставка не найдена.");
                            MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExhibitionsAdminPage());
                        }
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем данные выставки
            _exhibition.Title = TitleTxt.Text;
            _exhibition.StartDate = StartDatePicker.SelectedDate ?? DateTime.Now;
            _exhibition.EndDate = EndDatePicker.SelectedDate ?? DateTime.Now;
            _exhibition.Description = DescriptionTxt.Text;

            // Сохраняем изменения в базе данных
            UpdateExhibitionInDatabase(_exhibition);

            // Возвращаемся на предыдущую страницу
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExhibitionsAdminPage());
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на предыдущую страницу без сохранения
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExhibitionsAdminPage());
        }

        private void UpdateExhibitionInDatabase(Exhibition exhibition)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Exhibitions SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, Description = @Description WHERE Id_Exhibition = @Id";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Title", exhibition.Title);
                    command.Parameters.AddWithValue("@StartDate", exhibition.StartDate);
                    command.Parameters.AddWithValue("@EndDate", exhibition.EndDate);
                    command.Parameters.AddWithValue("@Description", exhibition.Description);
                    command.Parameters.AddWithValue("@Id", exhibition.Id_Exhibition);
                    command.ExecuteNonQuery();
                }
                MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExhibitionsAdminPage());
            }
        }
    }
}

