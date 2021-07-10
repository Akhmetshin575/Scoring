using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Data.Entity;

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для MakeOrder3.xaml
    /// </summary>
    public partial class MakeOrder3 : Page
    {
        public static Client client;
        public static Car car;
        public static int indexOfTariff;
        public static int indexOfDealer;
        public static Worker worker;
        public static Partner dealer;
        public static bool denyOrder;
        public static List<ServiceAndInsurance> serviceAndInsurances;
        static int orderId;
        bool documentIsPrinted = false;
        public static string cityOfSale;
        public static string typeOfDocumentForCredit;
        public static string numberOfDocumentForCredit;
        public static string dateOfDocumentForCredit;
        public static string comment;

        public MakeOrder3()
        {
            InitializeComponent();
            sendAndSave.IsEnabled = true;
            sendNosave.IsEnabled = true;
            printDocuments.IsEnabled = false;
            confirmSuccess.IsEnabled = false;
            denySuccess.IsEnabled = false;
        }

        //Обработчик кнопки Отмена
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            if (sendNosave.IsEnabled == false) { sendNosave.Content = "Закрыть"; NavigationService.Navigate(new WorkerInterface()); }
            else
            {
                MessageBoxResult result = MessageBox.Show("Оменить ввод данных? Введенные значения будут утеряны.",
                        "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes: NavigationService.Navigate(new WorkerInterface()); break;
                    case MessageBoxResult.No: return;
                }
            }
        }

        //Обработчик кнопки "Назад"
        private void ToCar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void CheckFSSP_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(client.DateOfBirth.ToShortDateString());
            Process.Start("https://fssp.gov.ru/iss/ip");
        }

        private void CheckPassport_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(client.PassportNumber.ToString());
            Process.Start("http://xn--b1afk4ade4e.xn--b1ab2a0a.xn--b1aew.xn--p1ai/info-service.htm?sid=2000");
        }

        private void CheckDriverLicense_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(client.DriverLicenseSeries.ToString() + client.DriverLicenseNumber);
            Process.Start("https://xn--90adear.xn--p1ai/check/driver#+");
        }

        private void CheckEmployer_Click(object sender, RoutedEventArgs e)
        {
            if(client.InnOfOrganization != 0) Clipboard.SetText(client.InnOfOrganization.ToString());
            else Clipboard.SetText(client.NameOfOrganization);
            Process.Start("https://egrul.nalog.ru/index.html");
        }

        private void CheckCarPrice_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://amprice.ru/");
        }

        private void CheckEncumbrance_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(car.Vin);
            Process.Start("https://www.reestr-zalogov.ru/search/index");
        }

        private void SendNosave_Click(object sender, RoutedEventArgs e)
        {
            MakeOrder();
            sendNosave.IsEnabled = false;
        }

        //Метод для "сборки" новой заявки и сохранения в БД
        private void MakeOrder()
        {
            using(UserContext3 db = new UserContext3())
            {
                //определяется запись в БД с нужным сотрудником
                worker = db.Workers.Where(w => w.WorkerId == MainWindow.id).First();

                //определяется запись в БД с нужным тарифом
                Tariff tariff = db.Tariffs.Where(t => t.TariffId == indexOfTariff).First();

                //определяется запись в БД с нужным автосалоном
                dealer = db.Partners.Where(p => p.PartnerId == indexOfDealer).First();

                //в БД добавляется запись о клиенте (для нового клиента; для уже имеющегося в БД это не трубуется)
                if (WorkerInterface.mode == "Создание") db.Clients.Add(client);

                //в БД добавляется запись об автомобиле
                db.Cars.Add(car);
                db.SaveChanges();


                if (WorkerInterface.mode != "Редактирование")
                {
                    //Создается заявка
                    Order order = new Order(car, client, dealer, worker, tariff, serviceAndInsurances, DateTime.Now, DateTime.Now, denyOrder);
                    order.Car = car;
                    order.Client = client;
                    order.Dealer = dealer;
                    order.Worker = worker;
                    order.Tariff = tariff;
                    order.CityOfSale = cityOfSale;
                    order.TypeOfDocumentForCredit = typeOfDocumentForCredit;
                    order.NumberOfDocumentForCredit = numberOfDocumentForCredit;
                    if(dateOfDocumentForCredit != "") order.DateOfDocumentForCredit = Convert.ToDateTime(dateOfDocumentForCredit);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    orderId = order.OrderId;
                }
                else
                {
                    Order order = db.Orders.Where(o => o.OrderId == WorkerInterface.orderId).Include(o => o.Car).Include(o => o.Dealer).Include(o => o.Tariff).First();
                    var servicesToDelete = db.ServicesAndInsurances.Where(s => s.OrderId == WorkerInterface.orderId);
                    foreach (var s in servicesToDelete)
                    {
                        db.ServicesAndInsurances.Remove(s);
                    }
                    order.Car = car;
                    order.Dealer = dealer;
                    order.Tariff = tariff;

                    order.CityOfSale = cityOfSale;
                    order.TypeOfDocumentForCredit = typeOfDocumentForCredit;
                    order.NumberOfDocumentForCredit = numberOfDocumentForCredit;
                    if (dateOfDocumentForCredit != "") order.DateOfDocumentForCredit = Convert.ToDateTime(dateOfDocumentForCredit);
                    db.SaveChanges();
                    orderId = order.OrderId;
                }

                //в БД добавляются записи об услугах и/или страховках и они привязываются к созданной заявке
                foreach (var s in serviceAndInsurances)
                {
                    db.ServicesAndInsurances.Add(s);
                    s.OrderId = orderId;
                }
                db.SaveChanges();
            }
        }

        private void SendAndSave_Click(object sender, RoutedEventArgs e)
        {
            //Вариант 1: 
            //Предполагаемая часть для отправки заявки на оценку в рамках кредитного скоринга(пригодно к спользованию после
            //запуска Back - End
            //sendNosave.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); //сохраняем заявку в БД
            //string order = SerializeOrder();        //сериализуем заявку в формат JSON
            //string result = "";
            //SendOrderAsync(order, result);                             //заявка отправляется на рассмотрение в ассинхроном методе
            //Order newOrder = new Order();
            //newOrder = JsonConvert.DeserializeObject<Order>(result); //если ответ получен, то десериализуем данные

            //Вариант 2 (временный, пока не запущен Back-End банка):
            sendNosave.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); //сохраняем заявку в БД
            sendAndSave.IsEnabled = false;
            Order order;
            using (UserContext3 db = new UserContext3())
            {
                order = db.Orders.Find(orderId);
                if (order.RefusalRequired == true)
                {
                    order.StatusOfOrder = "Отказано";
                    order.AcceptableAmount = 0.ToString();
                }
                else
                {
                    Random rnd = new Random();
                    int n = rnd.Next(2);
                    if (n == 0)
                    {
                        order.StatusOfOrder = "Отказано";
                        order.AcceptableAmount = 0.ToString();
                    }
                    else
                    {
                        order.StatusOfOrder = "Одобрено";
                        order.AcceptableAmount = order.RequiestAmount;
                        sendAndSave.IsEnabled = false;
                        printDocuments.IsEnabled = true;
                    }
                }
                db.SaveChanges();
            }
        }

        //Метод для отправки данных в отдельном потоке
        async void SendOrderAsync(string order, string result)
        {
            string token = File.ReadAllText("token.txt");
            string url = $@"https://rusfinancebank.ru/{token}/getDecision?order={order}";   //подготовка адреса для отправки
            Uri uri = new Uri(url);
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            while (result == "")
            {
                try
                {
                    result = await webClient.DownloadStringTaskAsync(uri);    //заявка отправляется на рассмотрение
                    if (result == "") throw new Exception();
                }
                catch
                {
                    await Task.Delay(300000);   //если ответ не получен, то делаем пятиминутную отправку перед повтроной попыткой,
                                                //чтобы не перегружать сеть
                }
            }
        }

        //Метод для сериализации заявки (подготовка к отправке на рассмотрение)
        static string SerializeOrder()
        {
            Order order;
            using(UserContext3 db = new UserContext3())
            {
                order = db.Orders.Find(orderId);
            }
            string json = JsonConvert.SerializeObject(order);
            return json;
        }

        //Метод для печати кредитных документов
        static void PrintDocuments()
        {
            
        }

        private void PrintDocuments_Click(object sender, RoutedEventArgs e)
        {
            Order order;
            using (UserContext3 db = new UserContext3())
            {
                order = db.Orders.Find(orderId);
            }
            if (order.StatusOfOrder == "Одобрено" || order.StatusOfOrder == "Оформлено")
            {
                PrintDocuments();
                confirmSuccess.IsEnabled = true;
                denySuccess.IsEnabled = true;
            }
        }

        private void ConfirmSuccess_Click(object sender, RoutedEventArgs e)
        {
            Order order;
            using (UserContext3 db = new UserContext3())
            {
                order = db.Orders.Find(orderId);
                order.StatusOfOrder = "Оформлено";
                order.DateOfCreditIssued = DateTime.Now;
                db.SaveChanges();
            }
            sendAndSave.IsEnabled = false;
            sendNosave.IsEnabled = false;
            confirmSuccess.IsEnabled = false;
            denySuccess.IsEnabled = false;
        }

        private void DenySuccess_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Клиент действительно отказался от подписания договора?", "ВНИМАНИЕ", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            else { MessageBox.Show("Данные об оформленном кредите анулированы", "УВЕДОМЛЕНИЕ", MessageBoxButton.OK, MessageBoxImage.Information); }
        }
    }
}
