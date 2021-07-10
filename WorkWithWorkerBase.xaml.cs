using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    /// Логика взаимодействия для WorkWithWorkerBase.xaml
    /// </summary>
    public partial class WorkWithWorkerBase : Page
    {
        List<Worker> source = new List<Worker>();
        GridViewColumnHeader listViewSortColumn = null;
        SortAdorner listViewSortAdornerElement = null;

        public WorkWithWorkerBase()
        {
            InitializeComponent();
            comboBox_Post.ItemsSource = References.post;
            listBox_Cities.ItemsSource = References.citiesList;
            ListViewDraw();
        }

        private void searchStroke_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "Поиск по фамилии...") searchStroke.Text = "";
            searchStroke.Foreground = Brushes.Black;
        }

        private void SearchStroke_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "")
            {
                searchStroke.Foreground = Brushes.Gray;
                searchStroke.Text = "Поиск по фамилии...";
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            label_warning.Content = "";

            //Проверка заполнения всех обязательных данных на странице...
            foreach (Control c in container.Children)
            {
                //...текстовых полей
                if (c.GetType() == typeof(TextBox))
                {
                    c.Background = SystemColors.WindowBrush;
                    if (((TextBox)c).Text == "")
                    {
                        label_warning.Content = "Не заполнены все данные о сотруднике";
                        c.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                }

                //...выбор в комбобоксе
                if (c.GetType() == typeof(ComboBox))
                {
                    label3.Foreground = SystemColors.WindowTextBrush;
                    if (((ComboBox)c).SelectedItem == null)
                    {
                        label_warning.Content = "Не заполнена должность сотрудника";
                        label3.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        return;
                    }
                }

                //...выбор в листбоксе
                if (c.GetType() == typeof(ListBox))
                {
                    label4.Foreground = SystemColors.WindowTextBrush;
                    if (((ListBox)c).SelectedItems.Count == 0)
                    {
                        label_warning.Content = "Не задан доступ сотрудника к городам";
                        label4.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        return;
                    }
                }
            }


            //Проверка корректности ввода телефона (только цифры общим числом 10 штук)
            string correctPhoneFormat = "";

            char[] acceptableValues = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+' };
            foreach (char symbol in textBox_Phone.Text)
            {
                if (acceptableValues.Contains(symbol)) correctPhoneFormat += symbol;
                else
                {
                    label_warning.Content = "Номер мобильного телефона задан неправильно";
                    return;
                }
            }

            while (correctPhoneFormat.Length > 10)
            {
                correctPhoneFormat = correctPhoneFormat.TrimStart('7', '+', '8');
                if (correctPhoneFormat.Length == 0) break;
            }
            textBox_Phone.Text = correctPhoneFormat;



            //Создание записи о работнике и сохранение в БД
            Worker worker = new Worker(textBox_Surname.Text, textBox_Name.Text,
                                                 textBox_Patronymic.Text, comboBox_Post.Text, textBox_Phone.Text);
            using (UserContext3 db = new UserContext3())
            {
                //проверка наличия в БД записи о сотруднике
                if (db.Workers.FirstOrDefault(x => (x.Surname == worker.Surname && x.Name == worker.Name &&
                                              x.Patronymic == worker.Patronymic)) != null)
                {
                    MessageBox.Show("В базе данных имеются данные о данном сотруднике. Дублирование данных запрещено!",
                                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var cities = listBox_Cities.SelectedItems;
                foreach (string city in cities)
                {
                    City n = db.Cities.Where(x => x.Name.Equals(city)).First();
                    worker.Cities.Add(n);
                }
                db.Workers.Add(worker);
                db.SaveChanges();
                ListViewDraw();
                CleanControlls(container);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Worker worker = (Worker)listView_workersShow.SelectedItem;
            if (worker == null)
            {
                MessageBox.Show("Не выбран работник для удаления из базы данных", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                int id = worker.WorkerId;
                using (UserContext3 db = new UserContext3())
                {
                    Worker temp = db.Workers.Find(id);
                    db.Workers.Remove(temp);
                    db.SaveChanges();
                    ListViewDraw();
                    CleanControlls(container);
                }
            }
        }


        /// <summary>
        /// Метод для вывода списка работников в элемент ListView
        /// </summary>
        void ListViewDraw()
        {
            using (UserContext3 db = new UserContext3())
            {
                //Выводим в элемент ListView список сотрудников банка, исключая самого пользователя
                source.Clear();
                foreach (Worker w in db.Workers.Include(w => w.Cities))
                {
                    if (w.WorkerId == MainWindow.id) continue;
                    string cities = "";
                    var cities1 = w.Cities;
                    foreach (City c in cities1)
                    {
                        string city = c.ToString();
                        cities += city;
                    }
                    if(cities != "")cities = cities.Substring(0, cities.Length - 2);
                    source.Add(new Worker(w.WorkerId, w.Surname, w.Name, w.Patronymic, w.Post, w.Phone, cities));
                }
            }

            listView_workersShow.ItemsSource = source;
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(source);
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Surname", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.Filter = UserFilter;
        }


        //Метод для очистки значений в элементах управления контейнера 
        void CleanControlls(Grid grid)
        {
            foreach (Control c in grid.Children)
            {
                if (c == searchStroke) continue;
                if (c.GetType() == typeof(TextBox)) ((TextBox)c).Text = "";
                if (c.GetType() == typeof(ComboBox)) ((ComboBox)c).SelectedItem = null;
                if (c.GetType() == typeof(ListBox)) ((ListBox)c).SelectedItem = null;
            }
        }


        //Обработчик выбора элемента в списке работников
        private void ListView_workersShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (Worker)listView_workersShow.SelectedItem;
            using(UserContext3 db = new UserContext3())
            {
                if(temp != null)
                {
                    Worker worker = db.Workers.Include(w => w.Cities).Where(w => w.WorkerId == temp.WorkerId).FirstOrDefault();
                    textBox_Surname.Text = worker.Surname;
                    textBox_Name.Text = worker.Name;
                    textBox_Patronymic.Text = worker.Patronymic;
                    comboBox_Post.Text = worker.Post;
                    textBox_Phone.Text = worker.Phone;

                    var cities = worker.Cities;
                    foreach (City city in cities)
                    {
                        listBox_Cities.SelectedValue = city.Name;
                    }
                }
                else
                {
                    CleanControlls(container);
                }
            }
        }


        //Обработчик события - изменение текста в строке поиска
        private void SearchStroke_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(searchStroke.Text != "Поиск по фамилии...")
                CollectionViewSource.GetDefaultView(listView_workersShow.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(source);
        }
        //Фильтрация
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(searchStroke.Text) | searchStroke.Text == "Поиск по фамилии...") return true;
            else return ((item as Worker).Surname.IndexOf(searchStroke.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //Обработчик события - клик по заголовку столбца (сортировка)
        private void listView_workersShowColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader sortedColumn = (GridViewColumnHeader)sender;
            string sortDir = sortedColumn.Tag.ToString();
            if (listViewSortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortColumn).Remove(listViewSortAdornerElement);
                listView_workersShow.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDirection = ListSortDirection.Ascending;
            if (listViewSortColumn == sortedColumn && listViewSortAdornerElement.sortDirection == newDirection)
            {
                newDirection = ListSortDirection.Descending;
            }

            listViewSortColumn = sortedColumn;
            listViewSortAdornerElement = new SortAdorner(listViewSortColumn, newDirection);
            AdornerLayer.GetAdornerLayer(listViewSortColumn).Add(listViewSortAdornerElement);
            listView_workersShow.Items.SortDescriptions.Add(new SortDescription(sortDir, newDirection));
        }

        private void Button_Replace_Click(object sender, RoutedEventArgs e)
        {
            if(listView_workersShow.SelectedItem == null)
            {
                MessageBox.Show("Не выбран работник для редактирования данных", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Определение записи, которую надо отредактировать
            Worker temp = listView_workersShow.SelectedItem as Worker;
            int id = temp.WorkerId;

            using(UserContext3 db = new UserContext3())
            {
                //Определение соответсвующей записи в БД
                var worker = db.Workers.Find(id);

                //Запоминание текущего количества записей в БД и добавление новой записи о работнике
                int countWorkers = db.Workers.Count();
                Worker w = new Worker(textBox_Surname.Text, textBox_Name.Text,
                                                 textBox_Patronymic.Text, comboBox_Post.Text, textBox_Phone.Text);

                if (listBox_Cities.SelectedItems == null)
                {
                    MessageBox.Show("Не выбраны города доступа",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var cities = listBox_Cities.SelectedItems;
                foreach (string city in cities)
                {
                    City n = db.Cities.Where(x => x.Name.Equals(city)).First();
                    w.Cities.Add(n);
                }
                db.Workers.Add(w);
                db.SaveChanges();

                //если добавление прошло успешно, то удаляем старую запись из БД
                if (db.Workers.Count() > countWorkers)
                {
                    db.Workers.Remove(worker);
                    db.SaveChanges();

                    //Обновляем данные на контролле
                    ListViewDraw();
                    CleanControlls(container);
                }
            }
        }
    }
}
