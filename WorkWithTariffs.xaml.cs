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

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для WorkWithTariffs.xaml
    /// </summary>
    public partial class WorkWithTariffs : Page
    {
        List<Tariff> source = new List<Tariff>();
        GridViewColumnHeader listViewSortColumn = null;
        SortAdorner listViewSortAdornerElement = null;

        public WorkWithTariffs()
        {
            InitializeComponent();
            comboBoxMonths.ItemsSource = new int[] { 12, 24, 36, 48, 60, 72, 84};
            comboBoxPercentOfMinimalInitaialPayment.ItemsSource = new int[] { 0, 10, 20, 30, 40, 50 };
            comboBoxPercentOfMaximumInitaialPayment.ItemsSource = new int[] { 10, 20, 30, 40, 50, 70 };
            ListViewDraw();
        }


        void ListViewDraw()
        {
            using (UserContext3 db = new UserContext3())
            {
                //Выводим в элемент ListView список сотрудников банка, исключая самого пользователя
                source.Clear();
                foreach (Tariff t in db.Tariffs)
                {
                    source.Add(t);
                }
            }

            listViewTariffsShow.ItemsSource = source;
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(source);
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("FullName", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Months", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("PercentOfMinimalInitaialPayment", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.Filter = UserFilter;
        }

        //Фильтрация
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(searchStroke.Text) | searchStroke.Text == "Поиск тарифа...") return true;
            else return ((item as Tariff).FullName.IndexOf(searchStroke.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //Метод для очистки значений в элементах управления контейнера 
        void CleanControlls(params Grid[] grid)
        {
            foreach (var g in grid)
            {
                foreach (var c in g.Children)
                {
                    if (c == searchStroke) continue;
                    if (c.GetType() == typeof(TextBox)) ((TextBox)c).Text = "";
                    if (c.GetType() == typeof(ComboBox)) ((ComboBox)c).SelectedItem = null;
                    if (c.GetType() == typeof(RadioButton)) ((RadioButton)c).IsChecked = false;
                    else { continue; }
                }
            }
        }


        //Обработчики
        private void searchStroke_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "Поиск тарифа...") searchStroke.Text = "";
            searchStroke.Foreground = Brushes.Black;
        }

        private void SearchStroke_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "")
            {
                searchStroke.Foreground = Brushes.Gray;
                searchStroke.Text = "Поиск тарифа...";
            }
        }

        private void SearchStroke_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchStroke.Text != "Поиск тарифа...")
                CollectionViewSource.GetDefaultView(listViewTariffsShow.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(source);
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            labelWarning.Content = "";

            int flag = 0;
            //Проверка заполнения всех обязательных данных на странице...
            foreach (var c in container.Children)
            {
                //...текстовых полей
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Background = SystemColors.WindowBrush;
                    if (((TextBox)c).Text == "")
                    {
                        ((TextBox)c).Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                }

                //...выбор в комбобоксе
                if (c.GetType() == typeof(ComboBox))
                {
                    if (((ComboBox)c).SelectedItem == null)
                    {
                        labelWarning.Content = "Не заполнены обязательные сведения";
                        return;
                    }
                }

                //...выбор Radiobutton
                if (c.GetType() == typeof(RadioButton))
                {
                    if (((RadioButton)c).IsChecked == true) flag++;
                }

                else { continue; }
            }
            if (flag < 2)
            {
                labelWarning.Content = "Не заполнены обязательные сведения";
                return;
            }

            //Проверка корректности заданного значения процентной ставки:
            try
            {
                double percent = Convert.ToDouble(textBoxPercentageRate.Text);
                if (percent <= 0 | percent > 50) throw new Exception();
            }
            catch
            {
                labelWarning.Content = "Неверно задано значение процентной ставкиы";
                return;
            }

            bool condition = false, kasko = false;
            if (radioButtonNew.IsChecked == true) condition = true;
            if (radioButtonRequired.IsChecked == true) kasko = true;

            if(Convert.ToInt32(comboBoxPercentOfMinimalInitaialPayment.Text) > Convert.ToInt32(comboBoxPercentOfMaximumInitaialPayment.Text))
            {
                labelWarning.Content = "Ошибка при задании значений размера мин./макс. ПВ";
                return;
            }

            //Создание записи о работнике и сохранение в БД
            Tariff tariff = new Tariff(textBoxName.Text, Convert.ToInt32(comboBoxMonths.Text),
                Convert.ToDouble(textBoxPercentageRate.Text), kasko, condition,
                Convert.ToInt32(comboBoxPercentOfMinimalInitaialPayment.Text),
                Convert.ToInt32(comboBoxPercentOfMaximumInitaialPayment.Text));


            using (UserContext3 db = new UserContext3())
            {

                //проверка наличия в БД записи о тарифе
                if (db.Tariffs.FirstOrDefault(t => (t.FullName == tariff.FullName && t.Months == tariff.Months)) != null)
                {
                    MessageBox.Show("В базе данных уже имеются сведения о данном тарифе. Дублирование данных запрещено!",
                                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                db.Tariffs.Add(tariff);
                db.SaveChanges();
                ListViewDraw();
                CleanControlls(container);
            }
        }

        private void Button_Replace_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTariffsShow.SelectedItem == null)
            {
                MessageBox.Show("Не выбран тариф для редактирования", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Определение записи, которую надо отредактировать
            int id = (listViewTariffsShow.SelectedItem as Tariff).TariffId;

            using (UserContext3 db = new UserContext3())
            {
                //Определение соответсвующей записи в БД
                var temp = db.Tariffs.Find(id);
                bool kasko = false, condition = false;
                if (radioButtonRequired.IsChecked == true) kasko = true;
                if (radioButtonNew.IsChecked == true) condition = true;

                //Запоминание текущего количества записей в БД и добавление новой записи о работнике
                int countTariffs = db.Tariffs.Count();
                Tariff tariff = new Tariff(textBoxName.Text, Convert.ToInt32(comboBoxMonths.Text),
                                Convert.ToDouble(textBoxPercentageRate.Text), kasko, condition,
                                Convert.ToInt32(comboBoxPercentOfMinimalInitaialPayment.Text),
                                Convert.ToInt32(comboBoxPercentOfMaximumInitaialPayment.Text));

                db.Tariffs.Add(tariff);
                db.SaveChanges();

                //если добавление прошло успешно, то удаляем старую запись из БД
                if (db.Tariffs.Count() > countTariffs)
                {
                    db.Tariffs.Remove(temp);
                    db.SaveChanges();

                    //Обновляем данные на контролле
                    ListViewDraw();
                    CleanControlls(container);
                }
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Tariff tariff = (Tariff)listViewTariffsShow.SelectedItem;
            if (tariff == null)
            {
                MessageBox.Show("Не выбран тариф для удаления", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                int id = tariff.TariffId;
                using (UserContext3 db = new UserContext3())
                {
                    Tariff temp = db.Tariffs.Find(id);
                    db.Tariffs.Remove(temp);
                    db.SaveChanges();
                    ListViewDraw();
                    CleanControlls(container);
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ListView_TariffsShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (Tariff)listViewTariffsShow.SelectedItem;
            using (UserContext3 db = new UserContext3())
            {
                if (temp != null)
                {
                    Tariff tariff = db.Tariffs.Find(temp.TariffId);
                    textBoxName.Text = tariff.Name;
                    comboBoxMonths.Text = tariff.Months.ToString();
                    comboBoxPercentOfMinimalInitaialPayment.Text = tariff.PercentOfMinimalInitaialPayment.ToString();
                    comboBoxPercentOfMaximumInitaialPayment.Text = tariff.PercentOfMaximumInitaialPayment.ToString();
                    textBoxPercentageRate.Text = tariff.PercentageRate.ToString();
                    if (tariff.TariffForNewCar == true) radioButtonNew.IsChecked = true;
                    else radioButtonOld.IsChecked = true;
                    if (tariff.CarInsuranceRequired == true) radioButtonRequired.IsChecked = true;
                    else radioNotRequired.IsChecked = true;
                }
                else
                {
                    CleanControlls(container);
                }
            }
        }

        private void listViewTariffsShowColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader sortedColumn = (GridViewColumnHeader)sender;
            string sortDir = sortedColumn.Tag.ToString();
            if (listViewSortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortColumn).Remove(listViewSortAdornerElement);
                listViewTariffsShow.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDirection = ListSortDirection.Ascending;
            if (listViewSortColumn == sortedColumn && listViewSortAdornerElement.sortDirection == newDirection)
            {
                newDirection = ListSortDirection.Descending;
            }

            listViewSortColumn = sortedColumn;
            listViewSortAdornerElement = new SortAdorner(listViewSortColumn, newDirection);
            AdornerLayer.GetAdornerLayer(listViewSortColumn).Add(listViewSortAdornerElement);
            listViewTariffsShow.Items.SortDescriptions.Add(new SortDescription(sortDir, newDirection));
        }
    }

    //Реализация интерфейса IValueConverter для вывода типа автомобиля для которого предназначен тариф (новый/с пробегом)
    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "";

            if (value is bool)
            {
                if ((bool)value == true)
                {
                    result = "Новый";
                }
                else
                {
                    result = "С пробегом";
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "Новый":
                    return true;
                case "С пробегом":
                    return false;
            }
            return false;
        }
    }

    //Реализация интерфейса IValueConverter для вывода данных об обязательности страхования КАСКО в тарифе)
    public class YesNoToBooleanConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "";

            if (value is bool)
            {
                if ((bool)value == true)
                {
                    result = "Требуется";
                }
                else
                {
                    result = "Не требуется";
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "Требуется":
                    return true;
                case "Не требуется":
                    return false;
            }
            return false;
        }
    }
}
