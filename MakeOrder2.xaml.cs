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
using System.Data.Entity;

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для MakeOrder2.xaml
    /// </summary>
    public partial class MakeOrder2 : Page
    {
        References references = new References();
        TextBox[] boxes;
        bool flag = true;
        public static bool expressFilling = true;

        public MakeOrder2()
        {
            InitializeComponent();

            comboBoxDocumentType.ItemsSource = References.typeOfDocument;
            comboBoxEngineType.ItemsSource = References.typeOfMotor;
            comboBoxEcologicalClass.ItemsSource = References.ecologicalClass;
            comboBoxDocumentForCredit.ItemsSource = References.typeOfDocumentForCredit;
            comboBoxPeriod.ItemsSource = References.creditPeriod;
            comboBoxBrand.ItemsSource = references.car.Keys.ToList();

            List<ServiceAndInsurance> servicesList;
            //Заполняется список доступных авторизованному пользователю городов
            using (UserContext3 db = new UserContext3())
            {
                var workers = db.Workers.Include(w => w.Cities);
                var partners = db.Partners.Include(p => p.Cities);
                servicesList = db.ServicesAndInsurances.Where(s => s.OrderId == WorkerInterface.orderId).Include(s => s.Partners).ToList();

                Worker worker = workers.Where(w => w.WorkerId == MainWindow.id).First();

                List<string> cities = new List<string>();
                foreach(City c in worker.Cities) { cities.Add(c.ToString().TrimEnd(',', ' ')); }
                comboBoxCity.ItemsSource = cities;
            }

            //Формирование массива типа TextBox для последующей проверки корректности ввода стоимости
            //в соответсвующих полях автомобиля, ПВ, стоимости услуг и страховок
            boxes = new TextBox[] { textBoxPrice , textBoxInitialPayment , textBoxAdvancePrice ,
                                          textBoxRingPrice, textBoxRATPrice, textBoxKASKOSum, textBoxKASKOPrice,
                                          textBoxGAPSum, textBoxGAPPrice, textBoxLife2Sum, textBoxLife2Price,
                                          textBoxLife3Sum, textBoxLife3Price, textBoxCoronaSum, textBoxCoronaPrice,
                                          textBoxOSAGOSum, textBoxOSAGOPrice, textBoxDSAGOSum, textBoxDSAGOPrice,
                                          textBoxExtendedWarrantySum, textBoxExtendedWarrantyPrice,
                                          textBoxProtectionItemsSum, textBoxProtectionItemsPrice, textBoxEnginePower };

            if (WorkerInterface.mode == "Редактирование")
            {
                LoadData();
                LoadServices(servicesList);
                LoadInsurances();
            }
        }

        //Обработчик кнопки Отмена
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Оменить ввод данных? Введенные значения будут утеряны.",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes: NavigationService.Navigate(new WorkerInterface()); break;
                case MessageBoxResult.No: return;
            }
        }

        //Обработчик кнопки Назад
        private void ToClient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        //Обработчик для заполнения списка моделей авто на основе выбранного бренда
        private void ComboBoxModel_MouseEnter(object sender, MouseEventArgs e)
        {
            FillModels();
        }
        //Метод для заполнения списка моделей по выбранному бренду
        void FillModels()
        {
            if (comboBoxBrand.Text == "" || comboBoxBrand.Text == "Не выбрано") { comboBoxModel.ItemsSource = new List<string>(); }
            else
            {
                string keyword = comboBoxBrand.Text;
                int index = references.car.Keys.IndexOf(keyword);
                comboBoxModel.ItemsSource = references.car.Values[index].ToList();
            }
        }

        //Обработчик для заполнения списка партнеров (автосалонов) на основе выбранного города
        private void ComboBoxDealer_MouseEnter(object sender, MouseEventArgs e)
        {
            FindPartners();
        }
        //Метод отбора партнеров по выбранному городу
        void FindPartners ()
        {
            if (comboBoxCity.Text == "" || comboBoxCity.Text == "Не выбрано") { comboBoxDealer.ItemsSource = new List<string>(); }
            else
            {
                using (UserContext3 db = new UserContext3())
                {
                    var partners = db.Partners.Include(p => p.Cities).AsEnumerable();
                    //Определяется нужный город в БД
                    City city = db.Cities.Where(c => c.Name == comboBoxCity.Text).First();
                    //отбираются автосалоны, которые работают в данном городе 
                    var selected = partners.Where(p => p.TypeOfCounterParty == "Автосалон").Where(p => p.Cities.Contains(city));

                    //Создается коллекция уникальных значений (наименований контрагентов) для источника данных combobox
                    List<string> partnersList = new List<string>();
                    foreach (Partner p in selected)
                    {
                        string temp = p.Name;
                        if (partnersList.Contains(temp)) { continue; }
                        else { partnersList.Add(temp); }
                    }
                    partnersList.Sort();
                    comboBoxDealer.ItemsSource = partnersList;
                }
            }
        }

        //Обработчик для заполнения списка доступных реквизитов выбранного автосалона
        List<int> listOfPartnersIndexes = new List<int>();
        private void ComboBankDetails_MouseEnter(object sender, MouseEventArgs e)
        {
            FillBankDetails();
        }
        //Метод для отбора реквизитов по выбранному партнеру
        void FillBankDetails()
        {
            listOfPartnersIndexes.Clear();

            if (comboBoxDealer.Text == "" || comboBoxDealer.Text == "Не выбрано") { comboBankDetails.ItemsSource = new List<string>(); }
            else
            {
                using (UserContext3 db = new UserContext3())
                {
                    var partners = db.Partners.Where(p => p.Name == comboBoxDealer.Text);

                    //Создается коллекция уникальных значений (банковских реквизитов контрагентов) для источника данных combobox
                    List<string> partnersBankDetails = new List<string>();
                    foreach (Partner p in partners)
                    {
                        string rs = p.Rs;
                        if (partnersBankDetails.Contains(rs)) { continue; }
                        else { partnersBankDetails.Add(rs); listOfPartnersIndexes.Add(p.PartnerId); }
                    }
                    comboBankDetails.ItemsSource = partnersBankDetails;
                }
            }
        }

        //Обработчик для заполнения списка доступных тарифов на основе заданных параметров: новый/поддержанный автомобиль,
        //размер ПВ клиента, выбранный срок кредитования
        List<int> arrayOfTariffIndexes = new List<int>();       //для сохранения индексов отобранных тарифов (нужен для выбора
                                                                //тарифа при создании заявки)
        private void ComboBoxTariff_MouseEnter(object sender, MouseEventArgs e)
        {
            FillTariffs();
        }
        //Метод для заполнения списка подходящих тарифов на основе введенной ранее информации
        void FillTariffs()
        {
            arrayOfTariffIndexes.Clear();
            using (UserContext3 db = new UserContext3())
            {
                IEnumerable<Tariff> tariffs;

                //Отбрасываются "лишние" тарифы (для новых а/м или а/м с пробегом)
                if (radioButtonNewCar.IsChecked == false & radioButtonUsedCar.IsChecked == false) { return; }
                if (radioButtonNewCar.IsChecked == true) { tariffs = db.Tariffs.Where(t => t.TariffForNewCar == true); }
                else { tariffs = db.Tariffs.Where(t => t.TariffForNewCar == false); }

                //Отбрасываются "лишние" тарифы (со сроком не равным выбранному клиентом)
                if (comboBoxPeriod.Text == "") { return; }
                else { tariffs = tariffs.Where(t => t.Months == Convert.ToInt32(comboBoxPeriod.Text)); }

                //Отбрасываются "лишние" тарифы (там где размер ПВ не соответсвует указанному клиентом)
                if (textBoxPrice.Text == "" || textBoxInitialPayment.Text == "") { return; }
                else
                {
                    double initialPayment = (Convert.ToDouble(textBoxInitialPayment.Text) / Convert.ToDouble(textBoxPrice.Text)) * 100;
                    initialPayment = Math.Round(initialPayment, 4);
                    tariffs = tariffs.Where(t => (t.PercentOfMinimalInitaialPayment <= initialPayment && initialPayment < t.PercentOfMaximumInitaialPayment));
                }

                //Создается коллекция уникальных значений (банковских реквизитов контрагентов) для источника данных combobox
                List<string> tariffsName = new List<string>();
                foreach (Tariff t in tariffs)
                {
                    arrayOfTariffIndexes.Add(t.TariffId);

                    string temp = t.FullName;
                    tariffsName.Add(temp);
                }
                comboBoxTariff.ItemsSource = tariffsName;
            }
        }


        /// <summary>
        /// Метод для заполнения источника данных по услугам и страховкам в combobox
        /// </summary>
        /// <param name="box">ComboBox, который подлежит заполнению</param>
        /// <param name="item">Название продукта</param>
        /// <param name="type">Тип контрагента</param>
        void ServicesToSource(ComboBox box, string item, string type)
        {
            if (comboBoxCity.Text == "" || comboBoxCity.Text == "Не выбрано") { box.ItemsSource = new List<string>(); }
            else
            {
                using (UserContext3 db = new UserContext3())
                {
                    var partners = db.Partners.Include(p => p.Cities).Include(p => p.ServicesAndInsurances).AsEnumerable();
                    //Определяется нужный город и нужную услугу в БД
                    if (comboBoxCity.Text != "" & comboBoxCity.Text != "Не выбрано")
                    {
                        City city = db.Cities.Where(c => c.Name == comboBoxCity.Text).First();
                        ServiceAndInsurance service = db.ServicesAndInsurances.Where(s => s.NameOfProduct == item).First();
                        //отбираются поставщики услуг, которые работают в данном городе и предоставляют данную услугу
                        var selected = partners.Where(p => p.TypeOfCounterParty == type).Where(p => p.Cities.Contains(city)).
                                                Where(p => p.ServicesAndInsurances.Contains(service));

                        //Создается коллекция уникальных значений (наименований контрагентов) для источника данных combobox
                        List<string> partnersList = new List<string>();
                        foreach (Partner p in selected)
                        {
                            string temp = p.Name;
                            if (partnersList.Contains(temp)) { continue; }
                            else { partnersList.Add(temp); }
                        }
                        partnersList.Sort();
                        box.ItemsSource = partnersList;
                    }
                    else
                    {
                        return;
                    }
                    
                }
            }
        }

        //"Наполнение" услуг
        private void ComboBoxAdvancePartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxAdvancePartner, "Карта Адванс", "Услуги");
        }

        private void ComboBoxRingPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxRingPartner, "Карта РИНГ", "Услуги");
        }

        private void ComboBoxRATPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxRATPartner, "Карта РАТ", "Услуги");
        }

        //"Наполнение" страховок
        private void ComboBoxKASKOPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxKASKOPartner, "КАСКО", "Страховщик");
        }

        private void ComboBoxGAPPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxGAPPartner, "GAP", "Страховщик");
        }

        private void ComboBoxLife2Partner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxLife2Partner, "СЖ_2", "Страховщик");
        }

        private void ComboBoxLife3Partner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxLife3Partner, "СЖ_3", "Страховщик");
        }

        private void ComboBoxCoronaPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxCoronaPartner, "Страхование от коронавируса", "Страховщик");
        }

        
        private void ComboBoxDSAGOPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxDSAGOPartner, "ДСАГО", "Страховщик");
        }

        private void ComboBoxExtendedWarrantyPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxExtendedWarrantyPartner, "Продленная гарантия", "Страховщик");
        }

        private void ComboBoxProtectionItemsPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxProtectionItemsPartner, "Защита личных вещей", "Страховщик");
        }

        private void ComboBoxOSAGOPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            ServicesToSource(comboBoxOSAGOPartner, "ОСАГО", "Страховщик");
        }


        //копирование VIN в поле ввода номера кузова автомобиля
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            textBoxBody.Text = textBoxVIN.Text;
        }


        //Проставляем текущую дату в полях ввода даты при получении фокуса (если ранее не было введено значение)
        void WriteDate(TextBox box)
        {
            if (box.Text == "") { box.Text = DateTime.Now.ToShortDateString(); }
        }

        private void TextBoxDocumentForCreditDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxDocumentForCreditDate); }
        private void TextBoxInvoiceDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxInvoiceDate); }
        private void TextBoxRingInvoiceDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxRingInvoiceDate); }
        private void TextBoxRATInvoiceDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxRATInvoiceDate); }
        private void TextBoxKASKORegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxKASKORegistartionDate); }
        private void TextBoxKASKOStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxKASKOStartDat); }
        private void TextBoxGAPRegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxGAPRegistartionDate); }
        private void TextBoxGAPStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxGAPStartDat); }
        private void TextBoxLife2RegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxLife2RegistartionDate); }
        private void TextBoxLife2StartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxLife2StartDat); }
        private void TextBoxLife3RegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxLife3RegistartionDate); }
        private void TextBoxLife3StartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxLife3StartDat); }
        private void TextBoxCoronaRegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxCoronaRegistartionDate); }
        private void TextBoxCoronaStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxCoronaStartDat); }
        private void TextBoxOSAGORegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxOSAGORegistartionDate); }
        private void TextBoxOSAGOStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxOSAGOStartDat); }
        private void TextBoxDSAGORegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxDSAGORegistartionDate); }
        private void TextBoxDSAGOStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxDSAGOStartDat); }
        private void TextBoxExtendedWarrantyRegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxExtendedWarrantyRegistartionDate); }
        private void TextBoxExtendedWarrantyStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxExtendedWarrantyStartDat); }
        private void TextBoxProtectionItemsRegistartionDate_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxProtectionItemsRegistartionDate); }
        private void TextBoxProtectionItemsStartDat_GotFocus(object sender, RoutedEventArgs e) { WriteDate(textBoxProtectionItemsStartDat); }


        //Метод для проверки допустимости вводимых в текстовое поле символов
        bool CheckInput(string text, char[] alphabet)
        {
            if(text != "")
            {
                foreach(char c in text)
                {
                    if (!alphabet.Contains(c)) return false;
                }
            }
            return true;
        }

        //Проверка корректности ввода года выпуска автомобиля и соответствия а/м условиям банка (не старше 10 лет)
        private void TextBoxYear_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxYear.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (textBoxYear.Text == "") { return; }
            if (CheckInput(textBoxYear.Text, References.alphabet4) == false || textBoxYear.Text.Length != 4)
            {
                textBoxYear.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
            else
            {
                try
                {
                    int year = Convert.ToInt32(textBoxYear.Text);
                    if (DateTime.Now.Year - year < 0)
                    {
                        MessageBox.Show("Неверно указан год выпуска автомобиля", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        textBoxYear.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                    }
                    if (DateTime.Now.Year - year > 10)
                    {
                        MessageBox.Show("Автомобиль не удовлетворяет условиям банка", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        textBoxYear.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                    }
                }
                catch
                {
                    MessageBox.Show("Введено некорректное значение", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBoxYear.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                }
            }
        }

        //Проверка ввода серии документа на автомобиль (ПТС или аналог) и ее формата (2 цифры + 2 буквы кириллицей)
        private void TextBoxSeries_LostFocus(object sender, RoutedEventArgs e)
        {
            //очистка поля, проверка на ввод значения, перевод в верхний регистр
            textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (comboBoxDocumentType.Text == "ПТС" & textBoxSeries.Text == "") textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
            if (textBoxSeries.Text == "") return;
            else { textBoxSeries.Text = textBoxSeries.Text.ToUpper().Replace(" ", ""); }

            //проверяем что пользователь ввел допустимые символы (кириллица, цифры, пробел)
            char[] newAlphabet = References.alphabet4.Union(References.alphabet2).ToArray();
            if (CheckInput(textBoxSeries.Text, newAlphabet) == false || textBoxSeries.Text.Length != 4)
            {
                textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }

            //проверяем что в указанном значении два первых символа - цифры, два последних - буквы кириллицы
            if (!References.alphabet4.Contains(textBoxSeries.Text[0]) && !References.alphabet4.Contains(textBoxSeries.Text[1]))
            {
                textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
            if (!References.alphabet1.Contains(textBoxSeries.Text[2]) && !References.alphabet1.Contains(textBoxSeries.Text[3]))
            {
                textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
        }

        //Проверка формата ввода номера документа на автомобиль (ПТС или аналог):
        //ПТС - 6 цифр; ЭПТС - 15 цифр; ФУТС - 1 буква кириллицей, 3 цифры, 2 буквы кириллицей, 2 цифры; 
        private void TextBoxDocumentNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (comboBoxDocumentType.Text == "" || comboBoxDocumentType.Text == "Не выбрано") { return; }
            textBoxDocumentNumber.Text = textBoxDocumentNumber.Text.Replace(" ", "").ToUpper();

            switch (comboBoxDocumentType.Text)
            {
                case "ПТС":
                    if(textBoxDocumentNumber.Text == "" | textBoxDocumentNumber.Text.Length != 6)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    if (CheckInput(textBoxDocumentNumber.Text, References.alphabet4) == false)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    break;
                case "Электронный ПТС":
                    if (textBoxDocumentNumber.Text == "" | textBoxDocumentNumber.Text.Length != 15)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    if (CheckInput(textBoxDocumentNumber.Text, References.alphabet4) == false)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    break;
                case "Форма учета транспортного средства":
                    if (textBoxDocumentNumber.Text == "" | textBoxDocumentNumber.Text.Length != 8)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    char[] newalphabet = References.alphabet1.Union(References.alphabet4).ToArray();
                    if (CheckInput(textBoxDocumentNumber.Text, newalphabet) == false)
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    if (!References.alphabet1.Contains(textBoxDocumentNumber.Text[0]) |
                        !References.alphabet1.Contains(textBoxDocumentNumber.Text[4]) |
                        !References.alphabet1.Contains(textBoxDocumentNumber.Text[5]) |
                        !References.alphabet4.Contains(textBoxDocumentNumber.Text[1]) |
                        !References.alphabet4.Contains(textBoxDocumentNumber.Text[2]) |
                        !References.alphabet4.Contains(textBoxDocumentNumber.Text[3]) |
                        !References.alphabet4.Contains(textBoxDocumentNumber.Text[6]) |
                        !References.alphabet4.Contains(textBoxDocumentNumber.Text[7]) )
                    {
                        textBoxDocumentNumber.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                    break;
            }
        }

        #region Проверка корректности ввода дат в соответсвующих полях, обеспечение единообразия отображения дат
        //Метод для проверки корректности введенных в текстовые поля дат и приведения их к единому стилю
        public static void DateInput(TextBox box)
        {
            box.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (box.Text == "") return;
            else
            {
                string rightFormat = "";
                foreach (char c in box.Text)
                {
                    if (References.alphabet4.Contains(c)) rightFormat += c;
                }
                if (rightFormat.Length != 8) { box.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170)); return; }
                rightFormat = rightFormat.Insert(2, ".").Insert(5, ".");
                box.Text = rightFormat;
            }
            try
            {
                Convert.ToDateTime(box.Text);
                if (Convert.ToDateTime(box.Text) > DateTime.Now) throw new Exception();
                box.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            catch
            {
                box.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
            }
        }

        
        //Проверка корректности ввода дат в соответствующих полях
        private void TextBoxDocumentDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxDocumentDate);
        }

        private void TextBoxDocumentForCreditDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxDocumentForCreditDate);
        }

        private void TextBoxInvoiceDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxInvoiceDate);
        }

        private void TextBoxRingInvoiceDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxRingInvoiceDate);
        }

        private void TextBoxRATInvoiceDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxRATInvoiceDate);
        }

        private void TextBoxKASKORegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxKASKORegistartionDate);
        }

        private void TextBoxKASKOStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxKASKOStartDat);
        }

        private void TextBoxGAPRegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxGAPRegistartionDate);
        }

        private void TextBoxGAPStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxGAPStartDat);
        }

        private void TextBoxLife2RegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxLife2RegistartionDate);
        }

        private void TextBoxLife2StartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxLife2StartDat);
        }

        private void TextBoxLife3RegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxLife3RegistartionDate);
        }

        private void TextBoxLife3StartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxLife3StartDat);
        }

        private void TextBoxCoronaRegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxCoronaRegistartionDate);
        }

        private void TextBoxCoronaStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxCoronaStartDat);
        }

        private void TextBoxOSAGORegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxOSAGORegistartionDate);
        }

        private void TextBoxOSAGOStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxOSAGOStartDat);
        }

        private void TextBoxDSAGORegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxDSAGORegistartionDate);
        }

        private void TextBoxDSAGOStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxDSAGOStartDat);
        }

        private void TextBoxExtendedWarrantyRegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxExtendedWarrantyRegistartionDate);
        }

        private void TextBoxExtendedWarrantyStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxExtendedWarrantyStartDat);
        }

        private void TextBoxProtectionItemsRegistartionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxProtectionItemsRegistartionDate);
        }

        private void TextBoxProtectionItemsStartDat_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInput(textBoxProtectionItemsStartDat);
        }

        #endregion


        //Подстановка типового значения срока действия страховки (1 год) при получении фокуса в поле
        private void TextBoxKASKOPeriod_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxKASKOPeriod.Text == "") textBoxKASKOPeriod.Text = "12";
        }


        //Проверка корректности указания срока действия страховок
        void PeriodInputCheck(TextBox box)
        {
            box.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (box.Text == "") return;
            try
            {
                if (Convert.ToInt32(box.Text) < 1) throw new Exception();
            }
            catch
            {
                box.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
            }
        }

        private void TextBoxKASKOPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxKASKOPeriod);
        }

        private void TextBoxGAPPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxGAPPeriod);
        }

        private void TextBoxLife2Period_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxLife2Period);
        }

        private void TextBoxLife3Period_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxLife3Period);
        }

        private void TextBoxCoronaPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxCoronaPeriod);
        }

        private void TextBoxOSAGOPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxOSAGOPeriod);
        }

        private void TextBoxDSAGOPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxDSAGOPeriod);
        }

        private void TextBoxExtendedWarrantyPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxExtendedWarrantyPeriod);
        }

        private void TextBoxProtectionItemsPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxProtectionItemsPeriod);
        }

        private void TextBoxMass_LostFocus(object sender, RoutedEventArgs e)
        {
            PeriodInputCheck(textBoxMass);
        }

        //Метод для проверка корректности ввода стоимости в соответствующих полях автомобиля, ПВ, стоимости услуг и страховок
        bool CheckinAmounts()
        {
            bool result = true;
            foreach (TextBox t in boxes)
            {
                t.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                if (t.Text == "") continue;
                try
                {
                    if(Convert.ToDouble(t.Text) < 1) throw new Exception();
                }
                catch
                {
                    t.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                    result = false;
                }
            }
            return result;
        }

        //Метод для проверки заполнения данных в контроллах указанной Wrap-панели (все поля - при оформлении кредита)
        void CheckControlsFilling(WrapPanel wrap, TextBox[] optionalFields)
        {
            flag = true;
            textBlockCity.Foreground = Brushes.Black;
            textBlockDealer.Foreground = Brushes.Black;
            textBlockBankDetails.Foreground = Brushes.Black;
            textBlockNew.Foreground = Brushes.Black;
            textBlockModel.Foreground = Brushes.Black;
            textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            textBoxUVEOS.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            var textBoxes = wrap.Children.OfType<TextBox>();
            foreach (var tb in textBoxes)
            {
                if (optionalFields.Contains(tb)) continue;
                tb.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

                if (tb.Text == "")
                {
                    tb.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 170, 170));
                    flag = false;
                }
            }

            var comboBoxes = wrap.Children.OfType<ComboBox>();
            foreach (var cb in comboBoxes)
            {
                cb.Foreground = System.Windows.Media.Brushes.Black;

                if (cb.Text == "" || cb.Text == "Не выбрано" || cb.Text == "0")
                {
                    flag = false;
                    cb.Text = "Не выбрано";
                    if(cb == comboBoxCity) textBlockCity.Foreground = Brushes.Red;
                    if(cb == comboBoxDealer) textBlockDealer.Foreground = Brushes.Red;
                    if(cb == comboBankDetails) textBlockBankDetails.Foreground = Brushes.Red;
                    if(cb == comboBoxEcologicalClass) comboBoxEcologicalClass.Text = "0";
                    if(cb == comboBoxModel) textBlockModel.Foreground = Brushes.Red;
                    if(comboBoxDocumentType.Text == "ПТС")
                    {
                        if(textBoxSeries.Text == "") textBoxSeries.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        if(textBoxUVEOS.Text == "") textBoxUVEOS.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                    }

                    cb.Foreground = System.Windows.Media.Brushes.Red;
                    flag = false;
                }
            }

            if(radioButtonNewCar.IsChecked == false & radioButtonUsedCar.IsChecked == false) textBlockNew.Foreground = Brushes.Red;
            if (comboBoxPeriod.Text == "") { flag = false; textBlockPeriod.Foreground = Brushes.Red; }
            if (textBoxInitialPayment.Text == "") { flag = false; textBlockInitialPayment.Foreground = Brushes.Red; }
            if (comboBoxTariff.Text == "") { flag = false; textBlockTariff.Foreground = Brushes.Red; }
        }

        //Метод для проверки заполнения данных в контроллах указанной Wrap-панели (только поля для рассмотрения заявки)
        void CheckControlsFilling_Light()
        {
            flag = true;

            textBlockCity.Foreground = Brushes.Black;
            textBlockDealer.Foreground = Brushes.Black;
            textBlockBankDetails.Foreground = Brushes.Black;
            textBlockBrand.Foreground = Brushes.Black;
            textBlockModel.Foreground = Brushes.Black;
            textBlockYear.Foreground = Brushes.Black;
            textBlockNew.Foreground = Brushes.Black;
            textBlockPrice.Foreground = Brushes.Black;

            textBlockPeriod.Foreground = Brushes.Black;
            textBlockInitialPayment.Foreground = Brushes.Black;
            textBlockTariff.Foreground = Brushes.Black;

            if (comboBoxCity.Text == "") { flag = false; textBlockCity.Foreground = Brushes.Red; }
            if (comboBoxDealer.Text == "") { flag = false; textBlockDealer.Foreground = Brushes.Red; }
            if (comboBankDetails.Text == "") { flag = false; textBlockBankDetails.Foreground = Brushes.Red; }
            if (comboBoxBrand.Text == "") { flag = false; textBlockBrand.Foreground = Brushes.Red; }
            if (comboBoxModel.Text == "") { flag = false; textBlockModel.Foreground = Brushes.Red; }
            if (textBoxYear.Text == "") { flag = false; textBlockYear.Foreground = Brushes.Red; }
            if (radioButtonNewCar.IsChecked == false & radioButtonUsedCar.IsChecked == false) { flag = false; textBlockNew.Foreground = Brushes.Red; }
            if (textBoxPrice.Text == "") { flag = false; textBlockPrice.Foreground = Brushes.Red; }

            if (comboBoxPeriod.Text == "") { flag = false; textBlockPeriod.Foreground = Brushes.Red; }
            if (textBoxInitialPayment.Text == "") { flag = false; textBlockInitialPayment.Foreground = Brushes.Red; }
            if (comboBoxTariff.Text == "") { flag = false; textBlockTariff.Foreground = Brushes.Red; return; }
        }

        //Обработчик кнопки перехода с ледующей странице
        private void ToCheck_Click(object sender, RoutedEventArgs e)
        {
            if (CheckinAmounts() == false) return;      //проверка корректности ввода сумм

            //проверка выбора переключателей для выбранных услуг/страховок
            textBlockSMS.Foreground = Brushes.Black;
            if (radioButtonSMS_On.IsChecked == false & radioButtonSMS_Off.IsChecked == false)
                textBlockSMS.Foreground = Brushes.Red;
            if (radioButtonSMS_On.IsChecked == true & comboBoxPeriod.Text != "")
                textBlockSMS_Price.Text = $"(Сумма в кредит: {Convert.ToInt32(comboBoxPeriod.Text) * 100})";

            textBlockPBU.Foreground = Brushes.Black;
            if (radioButtonPBU_On.IsChecked == false & radioButtonPBU_Off.IsChecked == false)
                textBlockPBU.Foreground = Brushes.Red;

            //Выбор метода проверки заполнения данных по автомобилю - упрощенный вариант для первичного рассмотрения, либо
            //полная проверка перед оформлением
            if (expressFilling) CheckControlsFilling_Light();
            else CheckControlsFilling(car, new TextBox[] { textBoxSeries, textBoxUVEOS });
            if (flag == false) return;

            CheckFilling();
            if (flag == false) return;

            //Если все проверки завершились успешно
            else
            {
                //то на основе введенных данных создаем экземляр класса Car (автомобиль)
                Car car = MakeCar();
                if (car != default(Car)) MakeOrder3.car = car;

                //по выбранным позициям в раскрывающихся списках определяем id выбранного тарифа и автосалона, создаем и наполняем коллекция выбранных услуг и страховок
                int checkedTariff = comboBoxTariff.SelectedIndex;
                int checkedPartner = comboBankDetails.SelectedIndex;
                using (UserContext3 db = new UserContext3())
                {
                    //тариф
                    MakeOrder3.indexOfTariff = arrayOfTariffIndexes[checkedTariff];

                    //автосалон
                    MakeOrder3.indexOfDealer = listOfPartnersIndexes[checkedPartner];                   

                    //услуги
                    List<ServiceAndInsurance> serviceAndInsurances = new List<ServiceAndInsurance>();
                    if (radioButtonSMS_On.IsChecked == true) serviceAndInsurances.Add(new ServiceAndInsurance("СМС-информирование", "", DateTime.Now.ToShortDateString(),
                                                                                                              true, $"{110 * Convert.ToInt32(comboBoxPeriod.Text)}", 14));
                    if (radioButtonPBU_On.IsChecked == true) serviceAndInsurances.Add(new ServiceAndInsurance("Пакет банковских услуг", "", DateTime.Now.ToShortDateString(),
                                                                                                              true, "0", 14));
                    AddServices(services, serviceAndInsurances);
                    AddInsurances(insurances, serviceAndInsurances);
                    MakeOrder3.serviceAndInsurances = serviceAndInsurances;
                }

                MakeOrder3.cityOfSale = comboBoxCity.Text;
                MakeOrder3.typeOfDocumentForCredit = comboBoxDocumentForCredit.Text;
                MakeOrder3.numberOfDocumentForCredit = textBoxDocumentForCreditNumber.Text;
                MakeOrder3.dateOfDocumentForCredit = textBoxDocumentForCreditDate.Text;
                NavigationService.Navigate(new MakeOrder3());
            }
        }

        //Метод для добавления услуг
        void AddServices(WrapPanel panel, List<ServiceAndInsurance> list)
        {
            foreach(var c in panel.Children)
            {
                if (c.GetType() != typeof(WrapPanel)) continue;
                else
                {
                    var temp = c as WrapPanel;
                    string name = (temp.Children[0] as TextBlock).Text.TrimEnd(':');

                    bool toCredit;
                    if ((temp.Children[10] as CheckBox).IsChecked == true) toCredit = true; else toCredit = false;

                    if ((temp.Children[2] as TextBox).Text != "")
                    {
                        int id;
                        using (UserContext3 db = new UserContext3())
                        {
                            string request = (temp.Children[4] as ComboBox).Text;
                            var partner = db.Partners.Where(p => p.Name == request).First();
                            id = partner.PartnerId;
                        }
                        list.Add(new ServiceAndInsurance(name, (temp.Children[6] as TextBox).Text, (temp.Children[8] as TextBox).Text,
                                                         toCredit, (temp.Children[2] as TextBox).Text, id));
                    }
                }
            }
        }

        //Метод для добавления страховок
        void AddInsurances(StackPanel panel, List<ServiceAndInsurance> list)
        {
            foreach (var c in panel.Children)
            {
                if (c.GetType() != typeof(WrapPanel)) continue;
                else
                {
                    var temp = c as WrapPanel;
                    string name = (temp.Children[0] as TextBlock).Text.TrimEnd(':');

                    bool toCredit;
                    if ((temp.Children[6] as CheckBox).IsChecked == true) toCredit = true; else toCredit = false;

                    if ((temp.Children[2] as TextBox).Text != "")
                    {
                        int id;
                        using (UserContext3 db = new UserContext3())
                        {
                            string request = (temp.Children[8] as ComboBox).Text;
                            var partner = db.Partners.Where(p => p.Name == request).First();
                            id = partner.PartnerId;
                        }
                        list.Add(new ServiceAndInsurance(name, (temp.Children[14] as TextBox).Text, toCredit,
                                                               (temp.Children[16] as TextBox).Text,
                                                               (temp.Children[18] as TextBox).Text,
                                                               (temp.Children[10] as TextBox).Text,
                                                               (temp.Children[12] as TextBox).Text,
                                                               (temp.Children[2] as TextBox).Text,
                                                               (temp.Children[4] as TextBox).Text, id));
                    }
                }
            }
        }

        //Метод для создания экземпляра класса автомобиль на основе введенных данных на странице
        private Car MakeCar()
        {
            try
            {
                Car car = new Car();
                car.Brand = comboBoxBrand.Text;
                car.Model = comboBoxModel.Text;
                car.NameFromDocument = textBoxNameFromDocument.Text;
                car.TypeOfDocument = comboBoxDocumentType.Text;
                car.DocumentSeries = textBoxSeries.Text;
                car.DocumentNumber = textBoxDocumentNumber.Text;
                if (textBoxDocumentDate.Text == "") car.DocumentDateOfIssue = Convert.ToDateTime("01.01.1900"); else car.DocumentDateOfIssue = Convert.ToDateTime(textBoxDocumentDate.Text);
                if (radioButtonNewCar.IsChecked == true) car.NewCar = true; else car.NewCar = false;
                car.YearOfRelease = Convert.ToInt32(textBoxYear.Text);
                car.Price = Convert.ToInt32(textBoxPrice.Text);
                car.InitialPayment = Convert.ToInt32(textBoxInitialPayment.Text);
                car.Vin = textBoxVIN.Text;
                car.Uveos = textBoxUVEOS.Text;
                car.MotorNumber = textBoxEngine.Text;
                car.BodyNumber = textBoxBody.Text;
                if (textBoxEnginePower.Text == "") car.Powerful = 0; else car.Powerful = Convert.ToDouble(textBoxEnginePower.Text);
                car.TypeOfMotor = comboBoxEngineType.Text;
                if (comboBoxEcologicalClass.Text == "") car.EcologicalClass = 0; else car.EcologicalClass = Convert.ToInt32(comboBoxEcologicalClass.Text);
                if (textBoxMass.Text == "") car.PermittedWeight = 0; else car.PermittedWeight = Convert.ToInt32(textBoxMass.Text);
                car.Color = textBoxColor.Text;

                return car;
            }
            catch
            {
                MessageBox.Show("Сохранение данных не удалось. Допущена ошибка в введенных значениях (автомобиль). Проверьте данные.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return new Car();
            }
        }

        //Проверка корректности введенного VIN-номера автомобиля: 17 символов - цифры и буквы латиницы (кроме I, O, Q)
        private void TextBoxVIN_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxVIN.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (textBoxVIN.Text == "") return;
            textBoxVIN.Text = textBoxVIN.Text.ToUpper();

            char[] newAlphabet = References.alphabet4.Union(References.alphabet5).Except(new char[] { 'I', 'O', 'Q' }).ToArray();
            if(textBoxVIN.Text.Length != 17)
            {
                textBoxVIN.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                return;
            }
            else
            {
                foreach(char c in textBoxVIN.Text)
                {
                    if (!newAlphabet.Contains(c))
                    {
                        textBoxVIN.Background = new SolidColorBrush(Color.FromRgb(230, 170, 170));
                        return;
                    }
                }
            }
        }

        //Метод для проверки полноты заполнения данных по услугам и страховкам
        void CheckFilling()
        {
            //панели для проверки
            WrapPanel[] wraps = new WrapPanel[] { wpADVANCE, wpRING, wpRAT, wpKASKO, wpGAP, wpLife2, wpLife3, wpCorona,
                                                  wpOSAGO, wpDSAGO, wpExtendedWarranty, wpProtectionItems};
            //заголовки панелей. Они будут выделяться цветом при обнаружении расхождений
            TextBlock[] blocks = new TextBlock[] { textBlockAdvance, textBlockRING, textBlockRAT, textBlockKASKO,
                                                   textBlockGAP, textBlockLife2, textBlockLife3, textBlockCorona,
                                                   textBlockOSAGO, textBlockDSAGO, textBlockExtendedWarranty, textBlockProtectionItems};
            //Возврат черного цвета шрифта всем заголовкам (н., после предыдущей неудачной проверки)
            foreach(TextBlock block in blocks)
            {
                block.Foreground = Brushes.Black;
            }

            //перебираем панели
            for(int n = 0; n< wraps.Length; n++)
            {
                int filled = 0, notFilled = 0;      //счетчики заполненных и не заполненных контроллов
                foreach(var control in wraps[n].Children)   //проверяем каждый конролл в панели и инкрементируем счетчик
                {
                    if(control.GetType() == typeof(TextBox)) { if (((TextBox)control).Text == "") notFilled++; else filled++; }
                    if(control.GetType() == typeof(ComboBox)) { if (((ComboBox)control).Text == "") notFilled++; else filled++; }
                    if(control.GetType() == typeof(CheckBox)) { if (((CheckBox)control).IsChecked == true) filled++; }
                }

                //Проверяем расхождения (т.е. все поля должны быть либо заполненными(если услуга добавляется в кредит), либо
                //либо пустыми, если услуга реально не требуется). При наличии расхождений выделяем красным, уточняем
                //реальную потребность в услуге и если она не нужна, то очищаем поля
                if (filled != 0 && notFilled != 0)
                {
                    blocks[n].Foreground = Brushes.Red;
                    flag = false;
                    MessageBoxResult result = MessageBox.Show($"В услуге/страховке {blocks[n].Text} заполнены не все поля." +
                            " Требуется ли ее добавление в кредит?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result.ToString() == "Yes") return;
                    else
                    {
                        foreach (var control in wraps[n].Children)   //проверяем каждый контролл в панели и инкрементируем счетчик
                        {
                            if (control.GetType() == typeof(TextBox)) ((TextBox)control).Text = "";
                            if (control.GetType() == typeof(ComboBox)) ((ComboBox)control).SelectedIndex = -1;
                            if (control.GetType() == typeof(CheckBox)) ((CheckBox)control).IsChecked = false;
                        }
                        flag = true;
                        blocks[n].Foreground = Brushes.Black;
                    }
                }
            }
        }

        //Автоматическая подстановка страховой суммы (должна быть равна стоимости автомобиля)
        private void TextBoxKASKOSum_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxKASKOSum.Text = textBoxPrice.Text;
        }


        //Метод для предзаполнения полей заявки из БД при редактировании или копировании заявки
        void LoadData()
        {
            using (UserContext3 db = new UserContext3())
            {
                Order order = db.Orders.Where(o => o.OrderId == WorkerInterface.orderId).Include(o => o.Car).Include(o => o.Dealer)
                    .Include(o => o.Tariff).Include(o => o.ServicesAndInsurances).Include(o => o.Dealer).First();
                Car car = order.Car;
                Partner partner = order.Dealer;
                Tariff tariff = order.Tariff;
                var services = order.ServicesAndInsurances;

                comboBoxCity.Text = order.CityOfSale;
                FindPartners();
                comboBoxDealer.Text = partner.Name;
                FillBankDetails();
                comboBankDetails.Text = partner.Rs;

                comboBoxBrand.Text = car.Brand;
                FillModels();
                comboBoxModel.Text = car.Model;
                textBoxYear.Text = car.YearOfRelease.ToString();
                if (car.NewCar == true) radioButtonNewCar.IsChecked = true;
                else radioButtonUsedCar.IsChecked = true;

                comboBoxDocumentType.Text = car.TypeOfDocument;
                textBoxSeries.Text = car.DocumentSeries;
                textBoxDocumentNumber.Text = car.DocumentNumber;
                if(car.DocumentDateOfIssue.ToShortDateString() != "01.01.1900") textBoxDocumentDate.Text = car.DocumentDateOfIssue.ToShortDateString();
                textBoxNameFromDocument.Text = car.NameFromDocument;
                textBoxVIN.Text = car.Vin;
                textBoxBody.Text = car.BodyNumber;
                textBoxUVEOS.Text = car.Uveos;
                textBoxEngine.Text = car.MotorNumber;
                comboBoxEngineType.Text = car.TypeOfMotor;
                if(car.EcologicalClass != 0) comboBoxEcologicalClass.Text = car.EcologicalClass.ToString();
                if (car.Powerful != 0) textBoxEnginePower.Text = car.Powerful.ToString();
                if (car.PermittedWeight != 0) textBoxMass.Text = car.PermittedWeight.ToString();
                textBoxColor.Text = car.Color;
                textBoxPrice.Text = car.Price.ToString();
                comboBoxDocumentForCredit.Text = order.TypeOfDocumentForCredit;
                textBoxDocumentForCreditNumber.Text = order.NumberOfDocumentForCredit;
                if (order.DateOfDocumentForCredit.ToShortDateString() != "01.01.1900") textBoxDocumentForCreditDate.Text = order.DateOfDocumentForCredit.ToShortDateString();

                comboBoxPeriod.Text = tariff.Months.ToString();
                textBoxInitialPayment.Text = car.InitialPayment.ToString();
                FillTariffs();
                comboBoxTariff.Text = tariff.FullName;

                var temp1 = db.ServicesAndInsurances.Where(s => (s.NameOfProduct == "СМС-информирование") & (s.OrderId == WorkerInterface.orderId));
                if (temp1.Count() != 0) radioButtonSMS_On.IsChecked = true;
                else radioButtonSMS_Off.IsChecked = true;

                var temp2 = db.ServicesAndInsurances.Where(s => (s.NameOfProduct == "Пакет банковских услуг") & (s.OrderId == WorkerInterface.orderId));
                if (temp2.Count() != 0) radioButtonPBU_On.IsChecked = true;
                else radioButtonPBU_Off.IsChecked = true;
            }
        }


        //Метод для загрузки услуг
        void LoadServices(List<ServiceAndInsurance> service)
        {
            radioButtonSMS_Off.IsChecked = true;
            radioButtonPBU_Off.IsChecked = true;

            using (UserContext3 db = new UserContext3())
            {
                foreach (var s in service)
                {
                    if (s.TypeOfAdditionalProduct == "Страхование") continue;
                    else
                    {
                        switch (s.NameOfProduct)
                        {
                            case "СМС-информирование":
                                radioButtonSMS_On.IsChecked = true; break;
                            case "Пакет банковских услуг":
                                radioButtonPBU_On.IsChecked = true; break;
                            case "АДВАНС":
                                textBoxAdvancePrice.Text = s.Price.ToString();
                                textBoxAdvanceInvoiceNumber.Text = s.InvoiceNumber;
                                textBoxInvoiceDate.Text = s.InvoiceDate.ToShortDateString();
                                if (s.OnCredit == true) radioButtonToCredit1_Yes.IsChecked = true;
                                ServicesToSource(comboBoxAdvancePartner, "Карта Адванс", "Услуги");
                                comboBoxAdvancePartner.Text = db.Partners.Find(s.PartnerId).Name;
                                break;
                            case "РИНГ":
                                textBoxRingPrice.Text = s.Price.ToString();
                                textBoxRingInvoiceNumber.Text = s.InvoiceNumber;
                                textBoxRingInvoiceDate.Text = s.InvoiceDate.ToShortDateString();
                                if (s.OnCredit == true) radioButtonToCredit2_Yes.IsChecked = true;
                                ServicesToSource(comboBoxRingPartner, "Карта РИНГ", "Услуги");
                                comboBoxRingPartner.Text = db.Partners.Find(s.PartnerId).Name;
                                break;
                            case "РАТ":
                                textBoxRATPrice.Text = s.Price.ToString();
                                textBoxRATInvoiceNumber.Text = s.InvoiceNumber;
                                textBoxRATInvoiceDate.Text = s.InvoiceDate.ToShortDateString();
                                if (s.OnCredit == true) radioButtonToCredit3_Yes.IsChecked = true;
                                ServicesToSource(comboBoxRATPartner, "Карта РАТ", "Услуги");
                                comboBoxRATPartner.Text = db.Partners.Find(s.PartnerId).Name;
                                break;
                        }
                    }
                }
            }
        }

        //Метод для загрузки страховок
        void LoadInsurances()
        {
            List<ServiceAndInsurance> listOfServicesInOrder;
            using (UserContext3 db = new UserContext3())
            {
                listOfServicesInOrder = db.ServicesAndInsurances.Where(s => s.OrderId == WorkerInterface.orderId).ToList();

                Dictionary<string, string> insurancesDict = new Dictionary<string, string>();
                insurancesDict.Add("wpKASKO", "КАСКО");
                insurancesDict.Add("wpGAP", "GAP");
                insurancesDict.Add("wpLife2", "СЖ_2");
                insurancesDict.Add("wpLife3", "СЖ_3");
                insurancesDict.Add("wpCorona", "Страхование от коронавируса");
                insurancesDict.Add("wpOSAGO", "ОСАГО");
                insurancesDict.Add("wpDSAGO", "ДСАГО");
                insurancesDict.Add("wpExtendedWarranty", "Продленная гарантия");
                insurancesDict.Add("wpProtectionItems", "Защита личных вещей");

                foreach (var s in listOfServicesInOrder)
                {
                    string key = "";
                    string value = "";
                    foreach (var n2 in insurancesDict)
                    {
                        if (n2.Value == s.NameOfProduct)
                        {
                            key = n2.Key;
                            value = n2.Value;
                        }
                    }

                    foreach (var c in insurances.Children)
                    {
                        if (c.GetType() != typeof(WrapPanel)) continue;
                        else
                        {
                            var temp = c as WrapPanel;
                            if (temp.Name == key)
                            {
                                if (s.OnCredit == true) (temp.Children[6] as CheckBox).IsChecked = true;
                                (temp.Children[2] as TextBox).Text = s.InsuredSum.ToString();
                                (temp.Children[4] as TextBox).Text = s.Price.ToString();
                                (temp.Children[10] as TextBox).Text = s.Series;
                                (temp.Children[12] as TextBox).Text = s.Number;
                                (temp.Children[14] as TextBox).Text = s.InvoiceDate.ToShortDateString();
                                (temp.Children[16] as TextBox).Text = s.StartDate.ToShortDateString();
                                (temp.Children[18] as TextBox).Text = s.DurationMonths.ToString();
                                ServicesToSource((temp.Children[8] as ComboBox), value, "Страховщик");
                                (temp.Children[8] as ComboBox).Text = db.Partners.Find(s.PartnerId).Name;
                            }
                        }
                    }
                }
            }
        }
    }
}
