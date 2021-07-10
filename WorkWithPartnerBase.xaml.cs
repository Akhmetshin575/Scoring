using System;
using System.Collections;
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
    /// Логика взаимодействия для WorkWithPartnerBase.xaml
    /// </summary>
    public partial class WorkWithPartnerBase : Page
    {
        List<Partner> source = new List<Partner>();
        GridViewColumnHeader listViewSortColumn = null;
        SortAdorner listViewSortAdornerElement = null;

        public WorkWithPartnerBase()
        {
            InitializeComponent();
            comboBoxType.ItemsSource = References.typeOfPartner;
            listBoxCities.ItemsSource = References.citiesList;
            ListViewDraw();
        }

        private void listViewPartnersShowColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader sortedColumn = (GridViewColumnHeader)sender;
            string sortDir = sortedColumn.Tag.ToString();
            if (listViewSortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortColumn).Remove(listViewSortAdornerElement);
                listViewPartnersShow.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDirection = ListSortDirection.Ascending;
            if (listViewSortColumn == sortedColumn && listViewSortAdornerElement.sortDirection == newDirection)
            {
                newDirection = ListSortDirection.Descending;
            }

            listViewSortColumn = sortedColumn;
            listViewSortAdornerElement = new SortAdorner(listViewSortColumn, newDirection);
            AdornerLayer.GetAdornerLayer(listViewSortColumn).Add(listViewSortAdornerElement);
            listViewPartnersShow.Items.SortDescriptions.Add(new SortDescription(sortDir, newDirection));
        }

        private void searchStroke_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "Поиск по наименованию юридического лица...") searchStroke.Text = "";
            searchStroke.Foreground = Brushes.Black;
        }

        private void SearchStroke_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchStroke.Text == "")
            {
                searchStroke.Foreground = Brushes.Gray;
                searchStroke.Text = "Поиск по наименованию юридического лица...";
            }
        }

        private void SearchStroke_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchStroke.Text != "Поиск по наименованию юридического лица...")
                CollectionViewSource.GetDefaultView(listViewPartnersShow.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(source);
        }

        private void ListView_PartnersShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (Partner)listViewPartnersShow.SelectedItem;
            using (UserContext3 db = new UserContext3())
            {
                if (temp != null)
                {
                    Partner partner = db.Partners.Find(temp.PartnerId);
                    comboBoxType.Text = partner.TypeOfCounterParty;
                    textBoxInn.Text = partner.Inn;
                    textBoxKpp.Text = partner.Kpp;
                    textBoxBic.Text = partner.Bic;
                    textBoxName.Text = partner.Name;
                    textBoxRs.Text = partner.Rs;
                    textBoxKs.Text = partner.Ks;
                    textBoxBank.Text = partner.Bank;

                    foreach (ServiceAndInsurance s in partner.ServicesAndInsurances)
                    {
                        if (s == db.ServicesAndInsurances.Find(8)) checkBoxKASKO.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(9)) checkBoxGAP.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(10)) checkBoxOSAGO.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(11)) checkBoxDSAGO.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(12)) checkBoxLife2.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(13)) checkBoxLife3.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(14)) checkBoxPersonalItems.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(15)) checkBoxExtendedWarranty.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(16)) checkBoxCOVID.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(17)) checkBoxSMS.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(18)) checkBoxBankServices.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(19)) checkBoxAdvance.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(20)) checkBoxRING.IsChecked = true;
                        if (s == db.ServicesAndInsurances.Find(21)) checkBoxRAT.IsChecked = true;
                    }
                }
                else
                {
                    CleanControlls(container, CitiesAndProducts);
                }
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            labelWarning.Content = "";

            //Проверка заполнения всех обязательных данных на странице...
            foreach (var c in container.Children)
            {
                //...текстовых полей
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Background = SystemColors.WindowBrush;
                    if (((TextBox)c).Text == "" & ((TextBox)c).Name != "textBoxKpp")
                    {
                        labelWarning.Content = "Не заполнены обязательные реквизиты";
                        ((TextBox)c).Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                }

                //...выбор в комбобоксе
                if (c.GetType() == typeof(ComboBox))
                {
                    labelType.Foreground = SystemColors.WindowTextBrush;
                    if (((ComboBox)c).SelectedItem == null)
                    {
                        labelWarning.Content = "Не выбран тип партнёра";
                        labelType.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        return;
                    }
                }

                else { continue; }
            }

            int count = 0;
            foreach (var c in CitiesAndProducts.Children)
            {
                //...выбор в листбоксе
                if (c.GetType() == typeof(ListBox))
                {
                    labelCities.Foreground = SystemColors.WindowTextBrush;
                    labelProducts.Foreground = SystemColors.WindowTextBrush;

                    if (((ListBox)c).Name == "listBoxCities" & ((ListBox)c).SelectedItems.Count == 0)
                    {
                        labelWarning.Content = "Не выбраны города присутствия партнёра";
                        labelCities.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        return;
                    }
                }

                if (c.GetType() == typeof(CheckBox))
                {
                    if (((CheckBox)c).IsChecked == true) count++;
                }
            }
            if (count == 0 & comboBoxType.Text != "Автосалон")
            {
                labelWarning.Content = "Не выбраны предоставляемые партнёром услуги";
                labelProducts.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                return;
            }


            //Проверка корректности введенных пользователем данных...
            //Локальная функция для проверки данных:
            bool CheckData(string inputFieldText, int correctLength, string nameOfParameter)
            {
                if (inputFieldText.Length != correctLength)
                {
                    MessageBox.Show($"Неверная длина {nameOfParameter}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                try
                {
                    foreach (char c in inputFieldText) { Convert.ToInt32(c); }
                    return true;
                }
                catch
                {
                    MessageBox.Show($"Ошибка при вводе {nameOfParameter}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            //Проверка полей:
            if (textBoxInn.Text.Length == 10)
            {
                if (CheckData(textBoxInn.Text, 10, "ИНН") == false) return;
            }
            else
            {
                if (CheckData(textBoxInn.Text, 12, "ИНН") == false) return;
            }
            if (textBoxKpp.Text != "") if(CheckData(textBoxKpp.Text, 9, "КПП") == false) return;
            if(CheckData(textBoxBic.Text, 9, "БИК") == false) return;
            if(CheckData(textBoxRs.Text, 20, "р/с") == false) return;
            if(CheckData(textBoxKs.Text, 20, "к/с") == false) return;


            //Создание записи о работнике и сохранение в БД
            Partner partner = new Partner(textBoxName.Text, comboBoxType.Text, textBoxBic.Text, textBoxInn.Text,
                                          textBoxRs.Text, textBoxKs.Text, textBoxBank.Text, textBoxKpp.Text);
            using (UserContext3 db = new UserContext3())
            {
                //проверка наличия в БД записи о сотруднике
                if (db.Partners.FirstOrDefault(x => (x.TypeOfCounterParty == partner.TypeOfCounterParty &&
                                               x.Rs == partner.Rs)) != null)
                {
                    MessageBox.Show("В базе данных имеются данные о данном партнёре. Дублирование данных запрещено!",
                                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var cities = listBoxCities.SelectedItems;
                foreach (string city in cities)
                {
                    City n = db.Cities.Where(x => x.Name.Equals(city)).First();
                    partner.Cities.Add(n);
                }

                if (checkBoxKASKO.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(8));
                if (checkBoxGAP.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(9));
                if (checkBoxOSAGO.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(10));
                if (checkBoxDSAGO.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(11));
                if (checkBoxLife2.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(12));
                if (checkBoxLife3.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(13));
                if (checkBoxPersonalItems.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(14));
                if (checkBoxExtendedWarranty.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(15));
                if (checkBoxCOVID.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(16));
                if (checkBoxSMS.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(17));
                if (checkBoxBankServices.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(18));
                if (checkBoxAdvance.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(19));
                if (checkBoxRING.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(20));
                if (checkBoxRAT.IsChecked == true) partner.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(21));

                db.Partners.Add(partner);
                db.SaveChanges();
                ListViewDraw();
                CleanControlls(container, CitiesAndProducts);
            }
        }

        private void Button_Replace_Click(object sender, RoutedEventArgs e)
        {
            if (listViewPartnersShow.SelectedItem == null)
            {
                MessageBox.Show("Не выбран партнёр для редактирования данных", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Определение записи, которую надо отредактировать
            int id = (listViewPartnersShow.SelectedItem as Partner).PartnerId;

            using (UserContext3 db = new UserContext3())
            {
                //Определение соответсвующей записи в БД
                var partner = db.Partners.Find(id);

                //Запоминание текущего количества записей в БД и добавление новой записи о работнике
                int countPartners = db.Partners.Count();
                Partner p = new Partner(textBoxName.Text, comboBoxType.Text, textBoxBic.Text, textBoxInn.Text,
                                        textBoxRs.Text, textBoxKs.Text, textBoxBank.Text, textBoxKpp.Text);

                if (listBoxCities.SelectedItems.Count == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Не выбраны города присутствия. Использовать текущие данные?",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            foreach(City c in partner.Cities)
                            {
                                int temp = c.CityId;
                                p.Cities.Add(db.Cities.Find(temp));
                            }
                            break;
                        case MessageBoxResult.No: return;
                    }
                }
                else
                {
                    var cities = listBoxCities.SelectedItems;
                    foreach (string city in cities)
                    {
                        City n = db.Cities.Where(x => x.Name.Equals(city)).First();
                        p.Cities.Add(n);
                    }
                }

                int checkedBox = 0;
                foreach (Control c in CitiesAndProducts.Children)
                {
                    if (c.GetType() == typeof(CheckBox))
                    {
                        if (((CheckBox)c).IsChecked == true) checkedBox++;
                    }
                }
                if(checkedBox == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Не заданы страховки/услуги. Использовать текущие данные?",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.Yes: p.ServicesAndInsurances = new List<ServiceAndInsurance>(partner.ServicesAndInsurances); break;
                        case MessageBoxResult.No: return;
                    }
                }
                else
                {
                    if (checkBoxKASKO.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(8));
                    if (checkBoxGAP.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(9));
                    if (checkBoxOSAGO.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(10));
                    if (checkBoxDSAGO.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(11));
                    if (checkBoxLife2.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(12));
                    if (checkBoxLife3.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(13));
                    if (checkBoxPersonalItems.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(14));
                    if (checkBoxExtendedWarranty.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(15));
                    if (checkBoxCOVID.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(16));
                    if (checkBoxSMS.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(17));
                    if (checkBoxBankServices.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(18));
                    if (checkBoxAdvance.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(19));
                    if (checkBoxRING.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(20));
                    if (checkBoxRAT.IsChecked == true) p.ServicesAndInsurances.Add(db.ServicesAndInsurances.Find(21));
                }

                db.Partners.Add(p);
                db.SaveChanges();

                //если добавление прошло успешно, то удаляем старую запись из БД
                if (db.Partners.Count() > countPartners)
                {
                    db.Partners.Remove(partner);
                    db.SaveChanges();

                    //Обновляем данные на контролле
                    ListViewDraw();
                    CleanControlls(container, CitiesAndProducts);
                }
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Partner partner = (Partner)listViewPartnersShow.SelectedItem;
            if (partner == null)
            {
                MessageBox.Show("Не выбрана запись о партнёре для удаления из базы данных", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                int id = partner.PartnerId;
                using (UserContext3 db = new UserContext3())
                {
                    Partner temp = db.Partners.Find(id);
                    db.Partners.Remove(temp);
                    db.SaveChanges();
                    ListViewDraw();
                    CleanControlls(container, CitiesAndProducts);
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        void ListViewDraw()
        {
            using (UserContext3 db = new UserContext3())
            {
                //Выводим в элемент ListView список сотрудников банка, исключая самого пользователя
                source.Clear();
                foreach (Partner p in db.Partners.Include(p => p.Cities).Include(p => p.ServicesAndInsurances))
                {
                    string cities = "";
                    string products = "";

                    var cities1 = p.Cities;
                    foreach (City c in cities1)
                    {
                        string city = c.ToString();
                        cities += city;
                    }
                    if (cities != "") cities = cities.Substring(0, cities.Length - 2);

                    var products1 = p.ServicesAndInsurances;
                    foreach (ServiceAndInsurance s in products1)
                    {
                        string product = s.ToString();
                        products += product;
                    }
                    if (products != "") products = products.Substring(0, products.Length - 2);

                    if(p.Name != "ООО РУСФИНАНС БАНК")
                        source.Add(new Partner(p.PartnerId, p.Name, p.TypeOfCounterParty, products, cities));
                }
            }

            listViewPartnersShow.ItemsSource = source;
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(source);
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.Filter = UserFilter;
        }

        //Фильтрация
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(searchStroke.Text) | searchStroke.Text == "Поиск по наименованию юридического лица...") return true;
            else return ((item as Partner).Name.IndexOf(searchStroke.Text, StringComparison.OrdinalIgnoreCase) >= 0);
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
                    if (c.GetType() == typeof(ListBox)) ((ListBox)c).SelectedItem = null;
                    if (c.GetType() == typeof(CheckBox)) ((CheckBox)c).IsChecked = false;
                    else { continue; }
                }
            }
        }
    }
}
