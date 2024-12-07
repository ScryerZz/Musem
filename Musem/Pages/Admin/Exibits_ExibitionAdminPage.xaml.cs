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
    /// Логика взаимодействия для Exibits_ExibitionAdminPage.xaml
    /// </summary>
    public partial class Exibits_ExibitionAdminPage : Page
    {
        public Exibits_ExibitionAdminPage()
        {
            InitializeComponent();
            LoadExhibits();
            LoadExhibitions();
        }
        private string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExhibitionComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Пожалуйста, выберите выставку.";
                return;
            }

            if (ExhibitComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Пожалуйста, выберите экспонат для добавления.";
                return;
            }

            int exhibitId = (int)((ComboBoxItem)ExhibitComboBox.SelectedItem).Tag;
            int exhibitionId = (int)((ComboBoxItem)ExhibitionComboBox.SelectedItem).Tag;

            // Проверка, существует ли уже запись о данном экспонате на выставке
            if (IsExhibitAlreadyAdded(exhibitId, exhibitionId))
            {
                StatusTextBlock.Text = "Этот экспонат уже добавлен на эту выставку.";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [dbo].[Exhibit-Exhibition] (Id_Exhibit, Id_Exhibition) VALUES (@Id_Exhibit, @Id_Exhibition)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id_Exhibit", exhibitId);
                        command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            StatusTextBlock.Text = "Экспонат успешно добавлен на выставку.";
                            LoadExhibitsForExhibition(exhibitionId); 
                        }
                        else
                        {
                            StatusTextBlock.Text = "Ошибка при добавлении экспоната на выставку.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StatusTextBlock.Text = "Ошибка базы данных: " + ex.Message;
            }
        }

        private bool IsExhibitAlreadyAdded(int exhibitId, int exhibitionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM [dbo].[Exhibit-Exhibition] WHERE Id_Exhibit = @Id_Exhibit AND Id_Exhibition = @Id_Exhibition";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id_Exhibit", exhibitId);
                    command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);

                    int count = (int)command.ExecuteScalar();
                    return count > 0; // Если найдено больше 0, значит экспонат уже добавлен на выставку
                }
            }
        }
        private void LoadExhibits()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id_Exhibit, Title FROM [dbo].[Exhibits]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    ExhibitComboBox.Items.Clear(); // Очищаем текущие элементы

                    while (reader.Read())
                    {
                        // Создаем новый элемент ComboBox
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["Title"].ToString(),
                            Tag = reader["Id_Exhibit"] // Сохраняем Id_Exhibit в Tag
                        };
                        ExhibitComboBox.Items.Add(item); // Добавляем элемент в ComboBox
                    }
                }
            }
        }

        private void LoadExhibitions()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id_Exhibition, Title FROM [dbo].[Exhibitions]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    ExhibitionComboBox.Items.Clear(); // Очищаем текущие элементы

                    while (reader.Read())
                    {
                        // Создаем новый элемент ComboBox
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["Title"].ToString(),
                            Tag = reader["Id_Exhibition"] // Сохраняем Id_Exhibition в Tag
                        };
                        ExhibitionComboBox.Items.Add(item); // Добавляем элемент в ComboBox
                    }
                }
            }
        }
        private void ExhibitionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExhibitionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int exhibitionId = (int)selectedItem.Tag;
                LoadExhibitsForExhibition(exhibitionId);
            }
        }

        private void LoadExhibitsForExhibition(int exhibitionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT e.Id_Exhibit, e.Title FROM [dbo].[Exhibits] e " +
                               "JOIN [dbo].[Exhibit-Exhibition] ee ON e.Id_Exhibit = ee.Id_Exhibit " +
                               "WHERE ee.Id_Exhibition = @ExhibitionId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExhibitionId", exhibitionId);
                    SqlDataReader reader = command.ExecuteReader();
                    ExhibitListView.Items.Clear(); // Очищаем текущие элементы

                    while (reader.Read())
                    {
                        // Добавляем экспонаты в ListView
                        ExhibitListView.Items.Add(new ListViewItem
                        {
                            Content = reader["Title"].ToString(),
                            Tag = reader["Id_Exhibit"] // Сохраняем Id_Exhibit в Tag
                        });
                    }
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExhibitListView.SelectedItem == null)
            {
                StatusTextBlock.Text = "Пожалуйста, выберите экспонат для удаления.";
                return;
            }

            if (ExhibitionComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Пожалуйста, выберите выставку.";
                return;
            }

            int exhibitId = (int)((ListViewItem)ExhibitListView.SelectedItem).Tag;
            int exhibitionId = (int)((ComboBoxItem)ExhibitionComboBox.SelectedItem).Tag;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [dbo].[Exhibit-Exhibition] WHERE Id_Exhibit = @Id_Exhibit AND Id_Exhibition = @Id_Exhibition";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id_Exhibit", exhibitId);
                        command.Parameters.AddWithValue("@Id_Exhibition", exhibitionId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            StatusTextBlock.Text = "Экспонат успешно удалён с выставки.";
                            LoadExhibitsForExhibition(exhibitionId); // Обновляем список экспонатов
                        }
                        else
                        {
                            StatusTextBlock.Text = "Ошибка при удалении экспоната с выставки.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StatusTextBlock.Text = "Ошибка базы данных: " + ex.Message;
            }
        }
    }
}
