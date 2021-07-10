using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Data.Entity;

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для WorkerInterface.xaml
    /// </summary>
    public partial class WorkerInterface : Page
    {
        static MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
        static Page mainPage = mainWindow.Content as Page;

        List<Order> source = new List<Order>();

        GridViewColumnHeader listViewSortColumn = null;
        SortAdorner listViewSortAdornerElement = null;

        public static string mode;
        public static int orderId = 0;

        SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        SolidColorBrush red = new SolidColorBrush(Color.FromRgb(230, 170, 170));

        public WorkerInterface()
        {
            InitializeComponent();
            addOrder.IsEnabled = false;
            copyOrder.IsEnabled = false;
            editOrder.IsEnabled = false;

            using (UserContext3 db = new UserContext3())
            {
                DateTime dateLimit = DateTime.Now.AddDays(-30);
                source.AddRange(db.Orders.Where(o => o.DateOfOrder >= dateLimit).Include(o => o.Client).Include(o => o.Car).Where(o => o.Worker.WorkerId == MainWindow.id));
            }
            ListViewDraw(source);
        }

        private void ImageExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(mainPage);

            mainWindow.textBox_login.Foreground = Brushes.Gray;
            mainWindow.textBox_login.Text = "Логин";
            mainWindow.textBox_password.Text = "Пароль";
            mainWindow.passwordBox.Password = "";
            mainWindow.passwordBox.Visibility = Visibility.Hidden;
            mainWindow.textBox_password.Visibility = Visibility.Visible;
        }

        private void FullName_Loaded(object sender, RoutedEventArgs e)
        {
            using (UserContext3 db = new UserContext3())
            {
                Worker worker = db.Workers.Where(x => x.WorkerId == MainWindow.id).First();
                fullName.Content = worker.Surname + " " + worker.Name + " " + worker.Patronymic;
            }
        }

        private void searchStroke_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "Поиск в актуальных заявках...") searchStroke.Text = "";
            searchStroke.Foreground = Brushes.Black;
        }

        private void SearchStroke_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "")
            {
                searchStroke.Foreground = Brushes.Gray;
                searchStroke.Text = "Поиск в актуальных заявках...";
            }
        }

        private void SearchStroke_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchStroke.Text != "Поиск в актуальных заявках...")
                CollectionViewSource.GetDefaultView(listViewOrders.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(source);
        }

        private void listViewOrdersShowColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader sortedColumn = (GridViewColumnHeader)sender;
            string sortDir = sortedColumn.Tag.ToString();
            if (listViewSortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortColumn).Remove(listViewSortAdornerElement);
                listViewOrders.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDirection = ListSortDirection.Ascending;
            if (listViewSortColumn == sortedColumn && listViewSortAdornerElement.sortDirection == newDirection)
            {
                newDirection = ListSortDirection.Descending;
            }

            listViewSortColumn = sortedColumn;
            listViewSortAdornerElement = new SortAdorner(listViewSortColumn, newDirection);
            AdornerLayer.GetAdornerLayer(listViewSortColumn).Add(listViewSortAdornerElement);
            listViewOrders.Items.SortDescriptions.Add(new SortDescription(sortDir, newDirection));
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            mode = "Создание";
            NavigationService.Navigate(new MakeOrder1());
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            mode = "Редактирование";
            NavigationService.Navigate(new MakeOrder1());
        }

        void ListViewDraw(List<Order> source)
        {
            //Выводим в элемент ListView список список отобранных заявок
            listViewOrders.ItemsSource = source;
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(source);
            //Сортировка по дате создания
            collectionView.SortDescriptions.Add(new SortDescription("DateOfOrder", ListSortDirection.Descending));
            collectionView.Filter = UserFilter;
        }

        //Фильтрация
        private bool UserFilter(object item)
        {
            //Если значения не задано, то считаем что любое значение удовлетворяет условию и должно быть включено в выборку
            if (String.IsNullOrEmpty(searchStroke.Text) | searchStroke.Text == "Поиск в актуальных заявках...") return true;
            //в ином случае отбираем заявки в которых фамилия клиента совпадает с введенным значением
            else return ((item as Order).Client.Surname.IndexOf(searchStroke.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //Обработчик кнопки "Поиск" (поиск заявки в единой БД по сочетанию фамилии и даты рождения клиента)
        //Поиск выдает последнюю заявку клиента (при наличии), без учета даты создания. Это позволит использовать последнюю
        //заявку как шаблон (для предзаполнения полей)
        private void FindOrder_Click(object sender, RoutedEventArgs e)
        {
            surname.Background = white;
            birthday.Background = white;

            List<Order> order = new List<Order>();
            if (surname.Text != "" & birthday.Text !="")
            {
                using (UserContext3 db = new UserContext3())
                {
                    try
                    {
                        DateTime date = Convert.ToDateTime(birthday.Text);
                        source.Clear();
                        source.AddRange(db.Orders.Include(o => o.Client).Include(o => o.Car)
                            .Where(o => o.Client.Surname == surname.Text.ToUpper()).Where(o => o.Client.DateOfBirth == date));
                    }
                    catch { return; }
                }
                if (source.Count > 1) order.Add(source.Last());
                else { if (source.Count == 1) order.Add(source.First()); }
                ListViewDraw(order);
                addOrder.IsEnabled = true;
            }

            if (surname.Text == "" & birthday.Text == "")
            {
                using (UserContext3 db = new UserContext3())
                {
                    DateTime dateLimit = DateTime.Now.AddDays(-30);
                    order.AddRange(db.Orders.Where(o => o.DateOfOrder >= dateLimit).Include(o => o.Client).Include(o => o.Car).Where(o => o.Worker.WorkerId == MainWindow.id));
                }
                ListViewDraw(order);
            }
            else
            {
                if(surname.Text == "") surname.Background = red;
                if(birthday.Text == "") birthday.Background = red;
            }
            copyOrder.IsEnabled = false;
            editOrder.IsEnabled = false;
        }

        //Приведение введенной даты к стандартному виду (ДД.ММ.ГГГГ)
        void DateInputCheck()
        {
            birthday.Background = white;
            List<char> alphabet = new List<char>(References.alphabet4);
            string correctFormat = "";
            foreach (char c in birthday.Text)
            {
                if (!alphabet.Contains(c)) continue;
                else correctFormat += c;
            }
            if (correctFormat.Length == 8)
            {
                correctFormat = correctFormat.Insert(2, ".").Insert(5, ".");
                birthday.Text = correctFormat;
                try
                {
                    DateTime date;
                    date = Convert.ToDateTime(birthday.Text);
                }
                catch
                {
                    birthday.Background = red;
                    return;
                }
            }
            else
            {
                birthday.Background = red;
                return;
            }
        }

        private void Birthday_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck();
        }

        //Обработчик кнопки "Копировать"
        private void CopyOrder_Click(object sender, RoutedEventArgs e)
        {
            mode = "Копирование";
            NavigationService.Navigate(new MakeOrder1());
        }

        //Обработчик выбора в ListView
        private void ListViewOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (Order)listViewOrders.SelectedItem;

            using (UserContext3 db = new UserContext3())
            {
                if (temp != null)
                {
                    orderId = temp.OrderId;
                    Order order = db.Orders.Find(temp.OrderId);

                    copyOrder.IsEnabled = false;
                    editOrder.IsEnabled = false;

                    switch (order.StatusOfOrder)
                    {
                        
                        case "Отказано":
                            if (order.DateOfOrder < DateTime.Now.AddDays(-90)) { copyOrder.IsEnabled = true; }
                            break;
                        case "Оформлено":
                            if (order.DateOfOrder < DateTime.Now.AddDays(-90)) { copyOrder.IsEnabled = true; }
                            break;
                        default:
                            if (order.DateOfOrder < DateTime.Now.AddDays(-30)) { copyOrder.IsEnabled = true; }
                            if (order.DateOfOrder >= DateTime.Now.AddDays(-30)) { editOrder.IsEnabled = true; }
                            break;
                    }
                }
            }
        }
    }



    //Реализация интерфейса IValueConverter для даты создания заявки в выбраном стиле
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).ToString("dd-MM-yyyy");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    //Реализация интерфейса IValueConverter для вывода фамилии клиента из заявки
    public class SurnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Client)value).Surname;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    //Реализация интерфейса IValueConverter для вывода имени клиента из заявки
    public class NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Client)value).Name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    //Реализация интерфейса IValueConverter для вывода отчества клиента из заявки
    public class PatronymicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Client)value).Patronymic;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    //Реализация интерфейса IValueConverter для вывода даты рождения клиента
    public class DateConverterBirthday : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Client)value).DateOfBirth.ToString("dd-MM-yyyy");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    //Реализация интерфейса IValueConverter для вывода номера мобильного телефона клиента из заявки
    public class PhoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Client)value).MobilePhoneNumber;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
