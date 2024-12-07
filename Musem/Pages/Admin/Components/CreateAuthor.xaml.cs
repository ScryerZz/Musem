using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для CreateAuthor.xaml
    /// </summary>
    public partial class CreateAuthor : Page
    {
        public CreateAuthor()
        {
            InitializeComponent();
        }

        private void CreatqeExibit_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Заполните все поля!");
            }
            else
            {
                string Name = txtName.Text;
                var Author = Database.DbConn.DbConnect.Authors.FirstOrDefault(auth => auth.Name == Name);
                if (Author != null)
                {
                    MessageBox.Show("Автор с таким ФИО уже есть!");
                }
                else
                {
                    var tempAuthor = new Database.Authors()
                    {
                        Name = Name,
                    };

                    Database.DbConn.DbConnect.Authors.Add(tempAuthor);
                    Database.DbConn.DbConnect.SaveChanges();
                    MessageBox.Show("Автор создан!");
                    NavigationService.Navigate(new ExibitsAdminPage());
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
