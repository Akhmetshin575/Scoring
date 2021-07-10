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

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для DirectorInterface.xaml
    /// </summary>
    public partial class DirectorInterface : Page
    {
        static MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
        static Page mainPage = mainWindow.Content as Page;

        public DirectorInterface()
        {
            InitializeComponent();
        }

        //Подгрузка данных (ФИО) в label после загрузки страницы
        private void FullName_Loaded(object sender, RoutedEventArgs e)
        {
            using (UserContext3 db = new UserContext3())
            {
                Worker worker = db.Workers.Where(x => x.WorkerId == MainWindow.id).First();
                fullName.Content = worker.Surname + " " + worker.Name + " " + worker.Patronymic;
            }
        }

        //Выход на страницу авторизации при нажатии на иконку выхода
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

        //Обработчик событий при нажатии кнопки корректировки данных о сотрудниках
        private void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkWithWorkerBase());
        }

        private void PartnerWork_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkWithPartnerBase());
        }

        private void TariffWork_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkWithTariffs());
        }
    }
}
