using Musem.Database;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Musem.Pages.Admin.Components
{
    /// <summary>
    /// Логика взаимодействия для CreateExibit.xaml
    /// </summary>
    public partial class CreateExibit : Page
    {
        public CreateExibit()
        {
            InitializeComponent();
            LoadData();
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

        private void CreateExibitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (nameTxt.Text == "" || descriptionTxt.Text == "" || createdDate.SelectedDate == null || ComboBoxAuthors.SelectedItem == null || ComboBoxConditions.SelectedItem == null || ComboBoxTypes.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!");
            }
            else
            {
                string Exibit = nameTxt.Text;
                var exibit = Database.DbConn.DbConnect.Exhibits.FirstOrDefault(ex => ex.Title == Exibit);
                if (exibit != null)
                {
                    MessageBox.Show("Экспонат с таким именем уже существует!");
                }
                else
                {
                    var tempEx = new Exhibits()
                    {
                        Title = Exibit,
                        CreationDate = createdDate.SelectedDate.Value,
                        Description = descriptionTxt.Text,
                        AddedDate = DateTime.Now,
                        Id_Author = (ComboBoxAuthors.SelectedItem as DataItem)?.Id,
                        Id_Condition = (ComboBoxConditions.SelectedItem as DataItem)?.Id,
                        Id_Type = (ComboBoxTypes.SelectedItem as DataItem)?.Id,
                    };

                    Database.DbConn.DbConnect.Exhibits.Add(tempEx);
                    Database.DbConn.DbConnect.SaveChanges();
                    MessageBox.Show("Экпонат зарегестрирван!");
                    MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExibitsAdminPage());
                    return;

                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
