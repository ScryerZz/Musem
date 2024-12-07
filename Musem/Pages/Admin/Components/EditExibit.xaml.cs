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
using static Musem.Pages.Admin.Components.CreateExibit;

namespace Musem.Pages.Admin.Components
{
    /// <summary>
    /// Логика взаимодействия для EditExibit.xaml
    /// </summary>
    public partial class EditExibit : Page
    {
        int _idExibit;
        public EditExibit(int Id_exhibit)
        {
            InitializeComponent();
            _idExibit = Id_exhibit;
            var exibit = Database.DbConn.DbConnect.Exhibits.FirstOrDefault(ex => ex.Id_Exhibit == Id_exhibit);

            if (exibit == null)
            {
                MessageBox.Show("Ошибка!! Аэропорт не найден!");
            }
            else
            {
                nameTxt.Text = exibit.Title;
                descriptionTxt.Text = exibit.Description;
                ComboBoxAuthors.SelectedValue = exibit.Id_Author;
                ComboBoxConditions.SelectedValue = exibit.Id_Condition;
                ComboBoxTypes.SelectedValue = exibit.Id_Type;
                createdDate.SelectedDate = exibit.CreationDate;
                LoadData();
            }

        }
        public class DataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private void LoadData()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            try
            {
                LoadComboBoxData(connectionString, "SELECT Id_Author, Name FROM Authors", ComboBoxAuthors);
                LoadComboBoxData(connectionString, "SELECT Id_Condition, Name FROM Conditions", ComboBoxConditions);
                LoadComboBoxData(connectionString, "SELECT Id_Type, Name FROM Types", ComboBoxTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
        private void LoadComboBoxData(string connectionString, string query, System.Windows.Controls.ComboBox comboBox)
        {
            List<DataItem> items = new List<DataItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new DataItem
                            {
                                Id = (int)reader[0],
                                Name = reader[1].ToString()
                            });
                        }
                    }
                }
            }

            comboBox.ItemsSource = items;
            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "Id";
            comboBox.Items.Refresh();
        }
        private void UpdateExhibit()
        {
            string connectionString = @"Data Source=DESKTOP-MUQH3DF\SQLEXPRESS;Initial Catalog=Музей;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"
                UPDATE Exhibits
                SET 
                    Title = @Title,
                    Description = @Description,
                    Id_Author = @Id_Author,
                    Id_Condition = @Id_Condition,
                    Id_Type = @Id_Type,
                    CreationDate = @CreationDate
                WHERE Id_Exhibit = @Id_Exhibit";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Title", nameTxt.Text);
                    command.Parameters.AddWithValue("@Description", descriptionTxt.Text);
                    command.Parameters.AddWithValue("@Id_Author", ComboBoxAuthors.SelectedValue);
                    command.Parameters.AddWithValue("@Id_Condition", ComboBoxConditions.SelectedValue);
                    command.Parameters.AddWithValue("@Id_Type", ComboBoxTypes.SelectedValue);
                    command.Parameters.AddWithValue("@CreationDate", createdDate.SelectedDate);
                    command.Parameters.AddWithValue("@Id_Exhibit", _idExibit);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные экспоната успешно обновлены!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка обновления данных экспоната.");
                    }
                }
            }
        }
        private void CreateExibitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateExhibit();
                MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExibitsAdminPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении экспоната: " + ex.Message);

            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExibitsAdminPage());
        }
    }
}
