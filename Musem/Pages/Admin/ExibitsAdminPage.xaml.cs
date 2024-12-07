using Musem.Database;
using Musem.Pages.Admin.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Musem.Pages.Admin.Components.CreateExibit;
using static Musem.Pages.Admin.ExibitsAdminPage;

namespace Musem.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для Exibits_ExibitionsAdminPage.xaml
    /// </summary>
    public partial class ExibitsAdminPage : Page
    {
        public ExibitsAdminPage()
        {
            InitializeComponent();
            LoadExhibits();
            LoadAuthorsData();
            LoadComboBoxData();
        }
        public class ExhibitViewModel
        {
            public int Id_Exhibit { get; set; }
            public string Title { get; set; }
            public DateTime CreationDate { get; set; }
            public string Description { get; set; }
            public string AuthorName { get; set; }
            public string ConditionName { get; set; }
            public string TypeName { get; set; }
        }
        public class Author
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name;
        }
        public class AuthorTab
        {
            public int Id_Author { get; set; }
            public string Name { get; set; }

        }
        public class ExhibitType
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name;
        }
        private void LoadComboBoxData()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string authorsQuery = "SELECT Id_Author, Name FROM Authors";
                using (SqlCommand command = new SqlCommand(authorsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBoxAuthors.Items.Add(new Author { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                        }
                    }
                }
                string typesQuery = "SELECT Id_Type, Name FROM Types";
                using (SqlCommand command = new SqlCommand(typesQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBoxTypes.Items.Add(new ExhibitType { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                        }
                    }
                }
            }
        }
        private void ComboBoxAuthors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterExhibits();
        }

        private void ComboBoxTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterExhibits();
        }

        private void DatePickerCreationDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterExhibits();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxAuthors.SelectedItem = null;
            ComboBoxTypes.SelectedItem = null;
            DatePickerCreationDate.SelectedDate = null;
            LoadExhibits();
        }

        private void FilterExhibits()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    e.Id_Exhibit,
                    e.Title,
                    e.CreationDate,
                    e.Description,
                    a.Name AS AuthorName,
                    c.Name AS ConditionName,
                    t.Name AS TypeName
                FROM 
                    Exhibits e
                JOIN 
                    Authors a ON e.Id_Author = a.Id_Author
                JOIN 
                    Conditions c ON e.Id_Condition = c.Id_Condition
                JOIN 
                    Types t ON e.Id_Type = t.Id_Type
                WHERE 
                    (@SelectedAuthor IS NULL OR a.Id_Author = @SelectedAuthor) AND
                    (@SelectedType IS NULL OR t.Id_Type = @SelectedType) AND
                    (@SelectedDate IS NULL OR CAST(e.CreationDate AS DATE) = @SelectedDate)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        int? selectedAuthor = (ComboBoxAuthors.SelectedItem as Author)?.Id;
                        int? selectedType = (ComboBoxTypes.SelectedItem as ExhibitType)?.Id;
                        DateTime? selectedDate = DatePickerCreationDate.SelectedDate;

                        command.Parameters.AddWithValue("@SelectedAuthor", selectedAuthor.HasValue ? (object)selectedAuthor.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@SelectedType", selectedType.HasValue ? (object)selectedType.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@SelectedDate", selectedDate.HasValue ? (object)selectedDate.Value : DBNull.Value);

                        DataTable dataTable = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }

                        ListExibits.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при фильтрации экспонатов: " + ex.Message);
            }
        }
        private void LoadExhibits()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                    SELECT 
                        e.Id_Exhibit,
                        e.Title,
                        e.CreationDate,
                        e.Description,
                        a.Name AS AuthorName,
                        c.Name AS ConditionName,
                        t.Name AS TypeName
                    FROM 
                        Exhibits e
                    JOIN 
                        Authors a ON e.Id_Author = a.Id_Author
                    JOIN 
                        Conditions c ON e.Id_Condition = c.Id_Condition
                    JOIN 
                        Types t ON e.Id_Type = t.Id_Type";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                        ListExibits.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке экспонатов: " + ex.Message);
            }
        }
        private void CreatqeExibit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new CreateExibit());
        }

        private void CreateAuthor_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new CreateAuthor());
        }

        private void EditExibit_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = ListExibits.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                MessageBox.Show("Выберите экспонат");
                return;
            }
            var selectedEx = new Exhibits
            {
                Id_Exhibit = (int)selectedRow["Id_Exhibit"],
                Title = (string)selectedRow["Title"],
                CreationDate = (DateTime)selectedRow["CreationDate"],
                Description = (string)selectedRow["Description"],
            };

            MainWindow.Instance.FrameMain.NavigationService.Navigate(new EditExibit(selectedEx.Id_Exhibit));
        }

        private void DeleteExibit_Click(object sender, RoutedEventArgs e)
        {
            var selectedExhibit = ListExibits.SelectedItem as DataRowView;

            if (selectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для удаления.");
                return;
            }

            int exhibitId = (int)selectedExhibit["Id_Exhibit"];

            // Проверяем, участвует ли экспонат в выставках
            if (IsExhibitInExhibitions(exhibitId))
            {
                MessageBox.Show("Этот экспонат не может быть удален, так как он участвует в выставках.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот экспонат?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DeleteExhibit(exhibitId);
                    MessageBox.Show("Экспонат удален успешно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении экспоната: " + ex.Message);
                }
            }
        }
        string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private bool IsExhibitInExhibitions(int exhibitId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM [Exhibit-Exhibition] WHERE Id_Exhibit = @ExhibitId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExhibitId", exhibitId);
                    int count = (int)command.ExecuteScalar();
                    return count > 0; // Возвращает true, если экспонат участвует в выставках
                }
            }
        }
        private void DeleteExhibit(int exhibitId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Exhibits WHERE Id_Exhibit = @Id_Exhibit";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibit", exhibitId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Экспонат успешно удален!");
                        LoadExhibits();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении экспоната.");
                    }
                }
            }
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorsList.SelectedItem as Author;
            if (selectedAuthor == null)
            {
                MessageBox.Show("Выберите автора для редактирования.");
                return;
            }
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new EditAuthor(selectedAuthor.Id));
        }
        private void LoadAuthorsData()
        {
            
            var authors = Database.DbConn.DbConnect.Authors.ToList();
            var authorsList = authors.Select(author => new Author
            {
                Id = author.Id_Author,
                Name = author.Name,
            }).ToList();

            AuthorsList.ItemsSource = authorsList; 
        }
        private bool IsAuthorInExhibit(int authorId)
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Exhibits WHERE Id_Author = @AuthorId";

                using (SqlCommand command = new SqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@AuthorId", authorId);
                    int count = (int)command.ExecuteScalar();

                    return count > 0; 
                }
            }
        }

        private void DeleteAuthorMeth(int authorId)
        {

            if (IsAuthorInExhibit(authorId))
            {
                MessageBox.Show("Невозможно удалить автора, так как он участвует в экспонате.");
                return;
            }

            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Authors WHERE Id_Author = @Id_Author";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id_Author", authorId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Автор успешно удален!");
                        LoadAuthorsData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении автора.");
                    }
                }
            }
        }
        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {

            var selectedAuthor = AuthorsList.SelectedItem as Author;

            if (selectedAuthor == null)
            {
                MessageBox.Show("Выберите автора для удаления.");
                return;
            }
            int authorId = selectedAuthor.Id;

            var result = MessageBox.Show("Вы уверены, что хотите удалить этого автора?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DeleteAuthorMeth(authorId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении автора: " + ex.Message);
                }
            }
        }
    }
}
