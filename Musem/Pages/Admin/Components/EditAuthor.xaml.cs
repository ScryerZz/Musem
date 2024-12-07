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
using static Musem.Pages.Admin.ExibitsAdminPage;

namespace Musem.Pages.Admin.Components
{
    /// <summary>
    /// Логика взаимодействия для EditAuthor.xaml
    /// </summary>
    public partial class EditAuthor : Page
    {
        int _id;
        public EditAuthor(int Id)
        {
            InitializeComponent();
            _id = Id;
            var selectedAuthor = Database.DbConn.DbConnect.Authors.FirstOrDefault(a => a.Id_Author == _id);
            txtName.Text = selectedAuthor.Name;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.FrameMain.GoBack();
        }

        private void CreatqeExibit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = Database.DbConn.DbConnect.Authors.FirstOrDefault(a => a.Id_Author == _id);
            if (selectedAuthor != null)
            {
                if (txtName.Text.Trim().Length > 0)
                {
                    selectedAuthor.Name = txtName.Text;
                    MessageBox.Show("Изменения сохранены!");
                    MainWindow.Instance.FrameMain.NavigationService.Navigate(new ExibitsAdminPage());
                }
                else
                {
                    MessageBox.Show("Заполните все поля!");
                }

            }
            else
            {
                MessageBox.Show("Автор не найден");
                MainWindow.Instance.FrameMain.GoBack();
            }
        }
    }
}
