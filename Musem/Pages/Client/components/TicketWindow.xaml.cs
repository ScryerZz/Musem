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
using System.Windows.Shapes;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using static Musem.Pages.Client.TicketsPageClient;

namespace Musem.Pages.Client.components
{
    /// <summary>
    /// Логика взаимодействия для TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : Window
    {
        public TicketWindow(int ticketId)
        {
            InitializeComponent();
            var ticket = Database.DbConn.DbConnect.Tickets.FirstOrDefault(t => t.Id_Ticket == ticketId);
            LoadTicketData(ticketId);
            string qrInfo = $"Ticket ID: {ticket.Id_Ticket}\n" +
            $"Exhibition: {ticket.Exhibitions.Title}\n" +
                                 $"Date: {ticket.PurchaseDate}\n" +
                                 $"Price: {ticket.Price:C}";
            QrCodeImage.Source = GenerateQrCodeBitmapImage(qrInfo);
        }
        private void LoadTicketData(int ticketId)
        {
            // Загрузка данных о билете по ticketId
            var ticket = Database.DbConn.DbConnect.Tickets.FirstOrDefault(t => t.Id_Ticket == ticketId);

            if (ticket != null)
            {
                // Заполнение текстовых полей
                IdTicketTxt.Text = $"{ticket.Id_Ticket}";
                IdTicket2.Text = $"Ticket ID: {ticket.Id_Ticket}";

                // Получение выставки
                var exhibition = Database.DbConn.DbConnect.Exhibitions.FirstOrDefault(e => e.Id_Exhibition == ticket.Id_Exhibition);

                if (exhibition != null)
                {
                    NameOfExhibition.Text = $"Название: {Convert.ToString(exhibition.Title)}"; // Например, поле Title выставки
                }
                else
                {
                    NameOfExhibition.Text = "Выставка не найдена";
                }

                // Обработка PurchaseDate как строки
                string purchaseDateString = Convert.ToString(ticket.PurchaseDate); // Предполагаем, что это строка
                DateTime purchaseDate;

                if (DateTime.TryParse(purchaseDateString, out purchaseDate)) // Пробуем преобразовать строку в DateTime
                {
                    DateOfExhibition.Text = $"Дата: {purchaseDate.ToString("dd.MM.yyyy")}"; // Форматируем дату
                }
                else
                {
                    DateOfExhibition.Text = "Дата: Не указана"; // Обработка случая, когда преобразование не удалось
                }

                PriceTicket1.Text = $"Цена: {Convert.ToString(ticket.Price)}"; // Предполагая, что у вас есть поле Price
            }
            else
            {
                MessageBox.Show("Билет не найден");
            }
        }
        private BitmapImage GenerateQrCodeBitmapImage(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q); using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            ms.Position = 0;
                            BitmapImage bitmapImage = new BitmapImage(); bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();
                            return bitmapImage;
                        }
                    }
                }
            }
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
