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
    /// Логика взаимодействия для ReportsAdminPage.xaml
    /// </summary>
    public partial class ReportsAdminPage : Page
    {
        public class Report
        {
            public int Id_Report { get; set; }
            public DateTime ReportDate { get; set; }
            public int VisitorCount { get; set; }
            public decimal Revenue { get; set; }
            public string ExhibitionName { get; set; } // Добавляем название выставки
        }
        public ReportsAdminPage()
        {
            InitializeComponent();
            LoadReports();
        }
        private void LoadReports()
        {
            List<Report> reports = new List<Report>();

            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                r.Id_Report, 
                r.ReportDate, 
                r.VisitorCount, 
                r.Revenue, 
                e.Title
            FROM 
                Reports r
            JOIN 
                Exhibitions e ON r.Id_Exhibition = e.Id_Exhibition"; // Запрос для получения данных
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        reports.Add(new Report
                        {
                            Id_Report = reader.GetInt32(0), // Получаем Id_Report
                            ReportDate = reader.GetDateTime(1), // Получаем ReportDate
                            VisitorCount = reader.GetInt32(2), // Получаем VisitorCount
                            Revenue = reader.GetDecimal(3), // Получаем Revenue
                            ExhibitionName = reader.GetString(4) // Получаем ExhibitionName
                        });
                    }
                }
            }

            ReportsListView.ItemsSource = reports; // Установка источника данных для ListView
        }
    }
}
