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
using System.Security.Cryptography;

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int id;

        public MainWindow()
        {
            InitializeComponent();

            //using (UserContext3 db = new UserContext3())
            //{
            //    Worker worker = new Worker
            //    {
            //        Surname = "Шайхелов",
            //        Name = "Айрат",
            //        Patronymic = "Мирзарифович",
            //        Post = "Директор",
            //        Login = "ShaykhelovAM"
            //    };
            //    List<City> cities = new List<City>();
            //    cities.Add(new City("Казань"));
            //    cities.Add(new City("Набережные Челны"));
            //    cities.Add(new City("Нижнекамск"));
            //    cities.Add(new City("Альметьевск"));
            //    cities.Add(new City("Елабуга"));
            //    cities.Add(new City("Чистополь"));
            //    worker.Cities = cities;
            //    db.Workers.Add(worker);
            //    db.SaveChanges();
            //}
        }

        private void TextBox_login_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_login.Text == "Логин") textBox_login.Text = "";
            textBox_login.Foreground = Brushes.Black;
        }

        private void TextBox_login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_login.Text == "")
            {
                textBox_login.Foreground = Brushes.Gray;
                textBox_login.Text = "Логин";
            }
        }

        private void TextBox_password_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox_password.Visibility = Visibility.Hidden;
            passwordBox.Visibility = Visibility.Visible;
            passwordBox.Focus();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == "")
            {
                passwordBox.Visibility = Visibility.Hidden;
                textBox_password.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //1. Проверка заполнения полей для ввода логина и пароля пользователя с выделением цветом
            textBox_login.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            textBox_password.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            if (textBox_login.Text == "" || textBox_login.Text == "Логин")
            {
                textBox_login.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
            if (textBox_password.Visibility == Visibility.Visible)
            {
                textBox_password.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }

            //2. Вычисляем хэш-значение пароля
            //Перевод введенного пользователем пароля в байтовый массив
            byte[] original = ASCIIEncoding.ASCII.GetBytes(passwordBox.Password);
            //Вычисление хэша
            byte[] calculate = new MD5CryptoServiceProvider().ComputeHash(original);
            //перевод byte массива с хэшем в строковое представление
            string hashToString = "";
            for (int n = 0; n < calculate.Length; n++)
            {
                hashToString += calculate[n].ToString();
            }

            //3. Проверяем наличие в базе данных пары логин/хэш пароля с введенными пользователем знечениями.
            //В зависимости от результата проверки либо выводится уведомление об ошибке, либо идет переход на след.страницу
            using (UserContext3 db = new UserContext3())
            {
                label_warning.Content = "";
                //Проверка наличия в БД пользователя с указанным логином.
                //Двойная фильтрация применена для обхождения ограничения SQL-сервера (сравнение без учета регистра)
                var worker = db.Workers.Where(x => x.Login == textBox_login.Text).AsEnumerable()
                                                                  .FirstOrDefault(x => x.Login == textBox_login.Text);
                if (worker == default(Worker))
                {
                    label_warning.Content = "ЛОГИН И/ИЛИ ПАРОЛЬ УКАЗАНЫ НЕ ВЕРНО";
                    return;
                }

                //Проверка совпадения хэшей введенного пользователем пароля и значения сохраненного в БД
                if (worker.Password == hashToString)
                {
                    id = worker.WorkerId;   //сохранение Id пользователя в статическом поле
                    //если у входящего в программу пользователя стоит признак первого входа, 
                    //то открыть окно запроса на изменение пароля пользователем
                    if (worker.FirstEntry == true) MainFrame.Navigate(new ChangePassword());
                    else
                    {
                        if(worker.Post == "Директор") MainFrame.Navigate(new DirectorInterface());
                        else MainFrame.Navigate(new WorkerInterface());
                    }
                }
                else
                {
                    label_warning.Content = "ЛОГИН И/ИЛИ ПАРОЛЬ УКАЗАНЫ НЕ ВЕРНО";
                    return;
                }
            }
        }
    }
}
