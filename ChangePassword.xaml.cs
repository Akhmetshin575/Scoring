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
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //1. Проверка заполнения полей для ввода текущего и нового пароля пользователя с выделением цветом
            passwordChange_firstField.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            passwordChange_secondField.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            if (passwordChange_firstField.Password == "")
            {
                passwordChange_firstField.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
            if (passwordChange_secondField.Password == "")
            {
                passwordChange_secondField.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }

            if (passwordChange_firstField.Password != passwordChange_secondField.Password)
            {
                message_changePassword.Content = "ВВЕДЕННЫЕ ЗНАЧЕНИЯ НЕ СОВПАДАЮТ";
                return;
            }

            //2. Вычисляем хэш-значение пароля
            //Перевод введенного пользователем пароля в байтовый массив
            byte[] original = ASCIIEncoding.ASCII.GetBytes(passwordChange_firstField.Password);
            //Вычисление хэша
            byte[] calculate = new MD5CryptoServiceProvider().ComputeHash(original);
            //перевод byte массива с хэшем в строковое представление
            //string hashToString = Encoding.ASCII.GetString(calculate);
            string hashToString = "";
            for (int n = 0; n < calculate.Length; n++)
            {
                hashToString += calculate[n].ToString();
            }

            using (UserContext3 db = new UserContext3())
            {
                var worker = db.Workers.Where(x => x.WorkerId == MainWindow.id).First();
                worker.Password = hashToString;
                worker.FirstEntry = false;
                db.SaveChanges();
                if(worker.Post == "Директор") NavigationService.Navigate(new DirectorInterface());
                else NavigationService.Navigate(new WorkerInterface());
            }
        }
    }
}
