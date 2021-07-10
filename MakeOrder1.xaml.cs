using Saraff.Twain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;
using BaseLibS;
using Microsoft.Win32;
using System.IO;
using WebEye.Controls.Wpf;
using System.Windows.Interop;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.Entity;

namespace Scoring3
{
    /// <summary>
    /// Логика взаимодействия для MakeOrder1.xaml
    /// </summary>
    public partial class MakeOrder1 : System.Windows.Controls.Page
    {
        bool flag = true;

        private System.Windows.Point pointRectangleToMouse = new System.Windows.Point(-1, -1);

        Uri uriOriginalImage;
        System.Windows.Shapes.Rectangle selRect;
        System.Windows.Point startSelection;
        System.Windows.Media.Pen pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Blue, 0.8f) { DashStyle = DashStyles.Dash };
        Twain32 twain = new Twain32();
        private TesseractEngine engine;

        SolidColorBrush white = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        SolidColorBrush red = new SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 170, 170));

        public MakeOrder1()
        {
            InitializeComponent();
            engine = new TesseractEngine(@"C:\Users\XtremeStation\Desktop\Diplom\Scoring3\tessdata", "rus");

            ocrDocument.Visibility = Visibility.Hidden;
            turnLeft.Visibility = Visibility.Hidden;
            turnRight.Visibility = Visibility.Hidden;
            cut.Visibility = Visibility.Hidden;
            clear.Visibility = Visibility.Hidden;
            textBlockReason.Visibility = Visibility.Hidden;
            textBoxReason.Visibility = Visibility.Hidden;
            denyConfirm.Visibility = Visibility.Hidden;
            comboBoxDocumentIncomeConfirmation.Visibility = Visibility.Hidden;

            comboBoxMobilePhoneResult.ItemsSource = References.resultOfCallList;
            comboBoxWorkPhoneResult.ItemsSource = References.resultOfCallList;
            comboBoxSpousePhoneResult.ItemsSource = References.resultOfCallList;
            comboBoxAdditionalPhoneResult.ItemsSource = References.resultOfCallList;
            comboboxEducation.ItemsSource = References.typeOfEducation;
            comboboxSocialStatus.ItemsSource = References.socialStatus;
            comboBoxPostType.ItemsSource = References.typeOfPost;
            comboboxDependentsNumber.ItemsSource = References.numberOfDependents;
            comboBoxCurrentExperience.ItemsSource = References.experience;
            comboBoxGeneralExperience.ItemsSource = References.experience;
            comboBoxDocumentIncomeConfirmation.ItemsSource = References.typeOfIncomeConfirmation;

            //если страница открывается в режиме редактирования или копирования заявки, то предзаполняем поля данными из БД
            if(WorkerInterface.mode == "Редактирование" || WorkerInterface.mode == "Копирование")
            {
                LoadClient();
            }
        }

        //Метод для предзаполнения полей заявки из БД при редактировании или копировании заявки
        void LoadClient()
        {
            using (UserContext3 db = new UserContext3())
            {
                Order order = db.Orders.Where(o => o.OrderId == WorkerInterface.orderId).Include(o => o.Client).First();
                Client client = order.Client;

                textBoxSurname.Text = client.Surname.ToUpper();
                textBoxName.Text = client.Name.ToUpper();
                textBoxPatronymic.Text = client.Patronymic.ToUpper();
                textBoxBirthday.Text = client.DateOfBirth.ToShortDateString();
                if (client.ClientIsMale == true) radioButtonMale.IsChecked = true;
                else radioButtonFemale.IsChecked = true;

                textBoxPassportSeries.Text = client.PassportSeries.ToString().ToUpper();
                textBoxPassportNumber.Text = client.PassportNumber.ToString().ToUpper();
                textBoxDate.Text = client.PassportDateOfIssue.ToShortDateString().ToUpper();
                textBoxCode.Text = client.PassportCodeOfDivision.ToString().ToUpper().Insert(3, "-");
                textBoxDivision.Text = client.PassportDivision.ToUpper();
                textBoxPlaceOfBirth.Text = client.PlaceOFBirth.ToUpper();

                textBoxDriverLicenseSeries.Text = client.DriverLicenseSeries.ToString().ToUpper();
                textBoxDriverLicenseNumber.Text = client.DriverLicenseNumber.ToString().ToUpper();
                textBoxDriverLicenseDate.Text = client.DriverLicenseDateofIssue.ToShortDateString();

                if (client.Married == true) radioButtonMarried.IsChecked = true;
                else radioButtonNotMarried.IsChecked = true;
                if (client.IncomeIsChecked == true) radioButtonIncomeConfirmed.IsChecked = true;
                else radioButtonNotIncomeConfirmed.IsChecked = true;

                textBoxRegistrationIndex.Text = client.ZipRegistration.ToString().ToUpper();
                if (textBoxRegistrationIndex.Text == "0") textBoxRegistrationIndex.Text = "";
                textBoxRegistrationRegion.Text = client.Subject_Registration.ToUpper();
                textBoxRegistrationDistrict.Text = client.District_Registration.ToUpper();
                textBoxRegistrationLocality.Text = client.NameOfPlaceOfResidence_Registration.ToUpper();
                textBoxRegistrationSubordinateLocality.Text = client.TypeOfPlaceOfResidence_Registration.ToUpper();
                textBoxRegistrationStreet.Text = client.Street_Registration.ToUpper();
                textBoxRegistrationHome.Text = client.NumberOfHouse_Registration.ToUpper();
                textBoxRegistrationApartment.Text = client.NumberOfApartment_Registration.ToUpper();
                textBoxRegistrationDate.Text = client.DateOfRegistartion.ToShortDateString();

                textBoxFactIndex.Text = client.Zip_Fact.ToString();
                if (textBoxFactIndex.Text == "0") textBoxFactIndex.Text = "";
                textBoxFactRegion.Text = client.Subject_Fact.ToUpper();
                textBoxFactDistrict.Text = client.District_Fact.ToUpper();
                textBoxFactLocality.Text = client.NameOfPlaceOfResidence_Fact.ToUpper();
                textBoxFactSubordinateLocality.Text = client.TypeOfPlaceOfResidence_Fact.ToUpper();
                textBoxFactStreet.Text = client.Street_Fact.ToUpper();
                textBoxFactHome.Text = client.NumberOfHouse_Fact.ToUpper();
                textBoxFactApartment.Text = client.NumberOfApartment_Fact.ToUpper();

                textBoxWeddingDate.Text = client.DateOfWedding.ToShortDateString();
                comboboxDependentsNumber.Text = client.NumberOfDependents.ToString();
                comboboxEducation.Text = client.Education;
                comboboxSocialStatus.Text = client.SocialStatus;

                textBoxEmployer.Text = client.NameOfOrganization.ToUpper();
                textBoxEmployerInn.Text = client.InnOfOrganization.ToString();
                if (textBoxEmployerInn.Text == "0") textBoxEmployerInn.Text = "";
                textBoxEmployerRegion.Text = client.Subject_Organization.ToUpper();
                textBoxEmployerDistrict.Text = client.District_Organization.ToUpper();
                textBoxEmployerLocality.Text = client.NameOfPlaceOfResidence_Organization.ToUpper();
                textBoxEmployerSubordinateLocality.Text = client.TypeOfPlaceOfResidence_Organization.ToUpper();
                textBoxEmployerStreet.Text = client.Street_Organization.ToUpper();
                textBoxEmployerHome.Text = client.NumberOfHouse_Organization.ToUpper();
                textBoxEmployerApartment.Text = client.NumberOfApartment_Organization.ToUpper();
                textBoxPost.Text = client.Post.ToUpper();
                comboBoxPostType.Text = client.TypeOfPost;
                comboBoxCurrentExperience.Text = client.Experience_current.ToString();
                comboBoxGeneralExperience.Text = client.Experience_general.ToString();
                textBoxGeneralIncome.Text = client.BasicIncome.ToString();
                textBoxAdditionalIncome.Text = client.AdditionalIncome.ToString();
                textBoxExpences.Text = client.Expenses.ToString();
                comboBoxDocumentIncomeConfirmation.Text = client.TypeOfIncomeConfirmation;

                textBoxMobilePhone.Text = client.MobilePhoneNumber.ToString();
                textBoxMobilePhoneDate.Text = client.DateOfCall_mobilePhoneNumber.ToShortDateString();
                textBoxMobilePhoneResponder.Text = client.Responder_mobilePhoneNumber.ToUpper();
                comboBoxMobilePhoneResult.Text = client.ResultOfCall_mobilePhoneNumber;

                textBoxWorkPhone.Text = client.WorkingPhoneNumber.ToString();
                textBoxWorkPhoneDate.Text = client.DateOfCall_workingPhoneNumber.ToShortDateString();
                textBoxWorkPhoneResponder.Text = client.Responder_workingPhoneNumber.ToUpper();
                comboBoxWorkPhoneResult.Text = client.ResultOfCall_workingPhoneNumber;

                textBoxSpousePhone.Text = client.SpousePhoneNumber.ToString();
                textBoxSpousePhoneDate.Text = client.DateOfCall_spousePhoneNumber.ToShortDateString();
                if (textBoxSpousePhone.Text == "8888888888")
                {
                    textBoxSpousePhone.Text = "";
                    textBoxSpousePhoneDate.Text = "";
                }
                textBoxSpousePhoneResponder.Text = client.Responder_spousePhoneNumber.ToUpper();
                comboBoxSpousePhoneResult.Text = client.ResultOfCall_spousePhoneNumber;

                textBoxAdditionalPhone.Text = client.AdditionalPhoneNumber.ToString();
                textBoxAdditionalPhoneDate.Text = client.DateOfCall_additionalPhoneNumber.ToShortDateString();
                if (textBoxAdditionalPhone.Text == "8888888888")
                {
                    textBoxAdditionalPhone.Text = "";
                    textBoxAdditionalPhoneDate.Text = "";
                }
                textBoxAdditionalPhoneResponder.Text = client.Responder_additionalPhoneNumber.ToUpper();
                comboBoxAdditionalPhoneResult.Text = client.ResultOfCall_additionalPhoneNumber;
            }
        }

        //Обработчик кнопки "Сохранить и продолжить"
        private void ToCar_Click(object sender, RoutedEventArgs e)
        {
            if (WorkerInterface.mode == "Создание")
            {
                using (UserContext3 db = new UserContext3())
                {
                    DateTime date = Convert.ToDateTime(textBoxBirthday.Text);
                    var temp = db.Clients.Where(s => (s.Surname == textBoxSurname.Text.ToUpper()) &
                                                     (s.Name == textBoxName.Text.ToUpper()) &
                                                     (s.Patronymic == textBoxPatronymic.Text.ToUpper()) &
                                                     (s.DateOfBirth == date));
                    if (temp.Count() != 0)
                    {
                        MessageBox.Show("В базе данных имеются данные об этом клиенте. Необходимо вернуться в" +
                                        " окно выбора заявок и осуществить поиск. Создание заявки прервано.", "ПРЕДУПРЕЖДЕНИЕ",
                                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }
            //контроль корректности распознавания(или ввода) кода подразделения паспорта
            if (textBoxCode.Text.Length != 7) { textBoxCode.Background = red; return; }

            //Проверка выбора radiobutton'ов на странице
            if (radioButtonMale.IsChecked == false & radioButtonFemale.IsChecked == false)
            {
                MessageBox.Show("Не выбран пол клиента. Сделайте выбор", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (radioButtonIncomeConfirmed.IsChecked == false & radioButtonNotIncomeConfirmed.IsChecked == false)
            {
                MessageBox.Show("Не выбрано предоставил ли клиент документы подтверждающие доход. Сделайте выбор", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (radioButtonMarried.IsChecked == false & radioButtonNotMarried.IsChecked == false)
            {
                MessageBox.Show("Не выбрано семейное положение клиента. Сделайте выбор", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            TextBox[] optionalFields = new TextBox[] {
                textBoxRegistrationIndex, textBoxFactIndex,
                textBoxRegistrationDistrict, textBoxFactDistrict, textBoxEmployerDistrict,
                textBoxRegistrationSubordinateLocality, textBoxFactSubordinateLocality, textBoxEmployerSubordinateLocality,
                textBoxRegistrationApartment, textBoxFactApartment, textBoxEmployerApartment,
                textBoxEmployerInn, textBoxAdditionalIncome
            };

            CheckControlsFilling(aboutClient, optionalFields);
            CheckControlsFilling(passport, optionalFields);
            CheckControlsFilling(DriverLicense, optionalFields);
            CheckControlsFilling(registration, optionalFields);
            CheckControlsFilling(factLiving, optionalFields);
            CheckControlsFilling(mobilePhone, optionalFields);
            CheckControlsFilling(workPhone, optionalFields);
            if(radioButtonMarried.IsChecked == true) CheckControlsFilling(Married, optionalFields);
            CheckControlsFilling(additional, optionalFields);
            CheckControlsFilling(aboutEmployee, optionalFields);

            if (flag == false) return;
            Client client;
            if (WorkerInterface.mode == "Создание") client = MakeClient();
            else { client = MakeClient(WorkerInterface.orderId); }
            if (client != default(Client)) MakeOrder3.client = client;
            else return;

            if (NavigationService.CanGoForward) NavigationService.GoForward();
            else NavigationService.Navigate(new MakeOrder2());
        }

        //Метод для создания экземпляра класса клиент на основе введенных данных страницы
        private Client MakeClient(params int[] id)
        {
            try
            {
                if (id.Length != 0)
                {
                    using (UserContext3 db = new UserContext3())
                    {
                        var temp = db.Orders.Include(o => o.Client);
                        Order order = temp.Where(o => o.OrderId == WorkerInterface.orderId).First();
                        Client client = order.Client;
                        FillFields(client);
                        db.SaveChanges();
                        return client;
                    }
                }
                else
                {
                    Client client = new Client();
                    FillFields(client);
                    return client;
                }
            }
            catch
            {
                MessageBox.Show("Сохранение данных не удалось. Допущена ошибка в введенных значениях. Проверьте данные.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return new Client();
            }
        }
        //Метод для заполнения свойств экземпляра класса Client
        void FillFields(Client client)
        {
            client.Surname = textBoxSurname.Text;
            client.Name = textBoxName.Text;
            client.Patronymic = textBoxPatronymic.Text;
            client.DateOfBirth = Convert.ToDateTime(textBoxBirthday.Text);
            if (radioButtonMale.IsChecked == true) client.ClientIsMale = true;
            else client.ClientIsMale = false;


            client.PlaceOFBirth = textBoxPlaceOfBirth.Text;
            client.Education = comboboxEducation.Text;
            client.SocialStatus = comboboxSocialStatus.Text;

            client.PassportSeries = Convert.ToInt32(textBoxPassportSeries.Text);
            client.PassportNumber = Convert.ToInt32(textBoxPassportNumber.Text);
            client.PassportDateOfIssue = Convert.ToDateTime(textBoxDate.Text);
            client.PassportDivision = textBoxDivision.Text;

            string temp = "";
            char[] arr = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            foreach (char c in textBoxCode.Text)
            {
                if (arr.Contains(c)) temp += c;
            }
            client.PassportCodeOfDivision = Convert.ToInt32(temp);

            client.DriverLicenseSeries = Convert.ToInt32(textBoxDriverLicenseSeries.Text);
            client.DriverLicenseNumber = Convert.ToInt32(textBoxDriverLicenseNumber.Text);
            client.DriverLicenseDateofIssue = Convert.ToDateTime(textBoxDriverLicenseDate.Text);

            if (textBoxRegistrationIndex.Text != "") client.ZipRegistration = Convert.ToInt32(textBoxRegistrationIndex.Text); else client.ZipRegistration = 0;
            if (textBoxFactIndex.Text != "") client.Zip_Fact = Convert.ToInt32(textBoxFactIndex.Text); else client.Zip_Fact = 0;
            client.Subject_Registration = textBoxRegistrationRegion.Text;
            client.District_Registration = textBoxRegistrationDistrict.Text;
            client.NameOfPlaceOfResidence_Registration = textBoxRegistrationLocality.Text;
            client.TypeOfPlaceOfResidence_Registration = textBoxRegistrationSubordinateLocality.Text;
            client.Street_Registration = textBoxRegistrationStreet.Text;
            client.NumberOfHouse_Registration = textBoxRegistrationHome.Text;
            client.NumberOfApartment_Registration = textBoxRegistrationApartment.Text;
            client.DateOfRegistartion = Convert.ToDateTime(textBoxRegistrationDate.Text);

            client.Subject_Fact = textBoxFactRegion.Text;
            client.District_Fact = textBoxFactDistrict.Text;
            client.TypeOfPlaceOfResidence_Fact = textBoxFactSubordinateLocality.Text;
            client.NameOfPlaceOfResidence_Fact = textBoxFactLocality.Text;
            client.Street_Fact = textBoxFactStreet.Text;
            client.NumberOfHouse_Fact = textBoxFactHome.Text;
            client.NumberOfApartment_Fact = textBoxFactApartment.Text;

            client.MobilePhoneNumber = Convert.ToInt64(textBoxMobilePhone.Text);
            client.DateOfCall_mobilePhoneNumber = Convert.ToDateTime(textBoxMobilePhoneDate.Text);
            client.Responder_mobilePhoneNumber = textBoxMobilePhoneResponder.Text;
            client.ResultOfCall_mobilePhoneNumber = comboBoxMobilePhoneResult.Text;

            client.WorkingPhoneNumber = Convert.ToInt64(textBoxWorkPhone.Text);
            client.DateOfCall_workingPhoneNumber = Convert.ToDateTime(textBoxWorkPhoneDate.Text);
            client.Responder_workingPhoneNumber = textBoxWorkPhoneResponder.Text;
            client.ResultOfCall_workingPhoneNumber = comboBoxWorkPhoneResult.Text;

            if (textBoxSpousePhone.Text != "") client.SpousePhoneNumber = Convert.ToInt64(textBoxSpousePhone.Text); else client.SpousePhoneNumber = 8888888888;
            if (textBoxSpousePhoneDate.Text != "") client.DateOfCall_spousePhoneNumber = Convert.ToDateTime(textBoxSpousePhoneDate.Text); else client.DateOfCall_spousePhoneNumber = DateTime.Now;
            client.Responder_spousePhoneNumber = textBoxSpousePhoneResponder.Text;
            client.ResultOfCall_spousePhoneNumber = comboBoxSpousePhoneResult.Text;

            if (textBoxAdditionalPhone.Text != "") client.AdditionalPhoneNumber = Convert.ToInt64(textBoxAdditionalPhone.Text); else client.AdditionalPhoneNumber = 8888888888;
            if (textBoxAdditionalPhoneDate.Text != "") client.DateOfCall_additionalPhoneNumber = Convert.ToDateTime(textBoxAdditionalPhoneDate.Text); else client.DateOfCall_additionalPhoneNumber = DateTime.Now;
            client.Responder_additionalPhoneNumber = textBoxAdditionalPhoneResponder.Text;
            client.ResultOfCall_additionalPhoneNumber = comboBoxAdditionalPhoneResult.Text;

            client.NameOfOrganization = textBoxEmployer.Text;
            if (textBoxEmployerInn.Text != "") client.InnOfOrganization = Convert.ToInt64(textBoxEmployerInn.Text); else client.InnOfOrganization = 0;
            client.Post = textBoxPost.Text;
            client.TypeOfPost = comboBoxPostType.Text;

            client.Subject_Organization = textBoxEmployerRegion.Text;
            client.District_Organization = textBoxEmployerDistrict.Text;
            client.TypeOfPlaceOfResidence_Organization = textBoxEmployerSubordinateLocality.Text;
            client.NameOfPlaceOfResidence_Organization = textBoxEmployerLocality.Text;
            client.Street_Organization = textBoxEmployerStreet.Text;
            client.NumberOfHouse_Organization = textBoxEmployerHome.Text;
            client.NumberOfApartment_Organization = textBoxEmployerApartment.Text;

            client.Experience_current = Convert.ToInt32(comboBoxCurrentExperience.Text);
            client.Experience_general = Convert.ToInt32(comboBoxGeneralExperience.Text);

            client.BasicIncome = Convert.ToInt32(textBoxGeneralIncome.Text);
            if (textBoxAdditionalIncome.Text != "") client.AdditionalIncome = Convert.ToInt32(textBoxAdditionalIncome.Text); else client.AdditionalIncome = 0;
            if (radioButtonIncomeConfirmed.IsChecked == true) client.IncomeIsChecked = true; else client.IncomeIsChecked = false;
            client.TypeOfIncomeConfirmation = comboBoxDocumentIncomeConfirmation.Text;
            client.Expenses = Convert.ToInt32(textBoxExpences.Text);

            if (radioButtonMarried.IsChecked == true) client.Married = true; else client.Married = false;
            double fullYears = 0;
            if (textBoxWeddingDate.Text != "") { fullYears = (DateTime.Now - Convert.ToDateTime(textBoxWeddingDate.Text)).Days / 365; }
            client.YearsOfMarriage = Convert.ToInt32(Math.Floor(fullYears));
            client.NumberOfDependents = Convert.ToInt32(comboboxDependentsNumber.Text);
            if (textBoxWeddingDate.Text != "") client.DateOfWedding = Convert.ToDateTime(textBoxWeddingDate.Text);
            else { client.DateOfWedding = Convert.ToDateTime("01.01.1900"); }
            if (radioButtonMale.IsChecked == true) client.ClientIsMale = true;
            else { client.ClientIsMale = false; }
        }

        //Метод для проверки заполнения данных в контроллах указанного грида
        void CheckControlsFilling(Grid grid, TextBox[] optionalFields)
        {
            flag = true;

            var textBoxes = grid.Children.OfType<TextBox>();
            foreach (var tb in textBoxes)
            {
                if (optionalFields.Contains(tb)) continue;

                tb.Background = white;

                if (tb.Text == "")
                {
                    tb.Background = red;
                    flag = false;
                }
            }

            var comboBoxes = grid.Children.OfType<ComboBox>();
            foreach (var cb in comboBoxes)
            {
                cb.Foreground = System.Windows.Media.Brushes.Black;

                if (cb.Text == "" || cb.Text == "Не выбрано")
                {
                    if (cb == comboBoxDocumentIncomeConfirmation & radioButtonNotIncomeConfirmed.IsChecked == true) continue;
                    flag = false;
                    cb.Text = "Не выбрано";
                    cb.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }
        //Метод для проверки заполнения данных в контроллах указанной Wrap-панели
        void CheckControlsFilling(WrapPanel wrap, TextBox[] optionalFields)
        {
            flag = true;

            var textBoxes = wrap.Children.OfType<TextBox>();
            foreach (var tb in textBoxes)
            {
                if (optionalFields.Contains(tb)) continue;
                tb.Background = white;

                if (tb.Text == "")
                {
                    tb.Background = red;
                    flag = false;
                }
            }

            var comboBoxes = wrap.Children.OfType<ComboBox>();
            foreach (var cb in comboBoxes)
            {
                cb.Foreground = System.Windows.Media.Brushes.Black;

                if (cb.Text == "" || cb.Text == "Не выбрано")
                {
                    if (cb == comboBoxDocumentIncomeConfirmation & radioButtonNotIncomeConfirmed.IsChecked == true) continue;
                    flag = false;
                    cb.Text = "Не выбрано";
                    cb.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }

        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите проставить признак отказа для текущей заявки?", "Предупреждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes: { textBlockReason.Visibility = Visibility.Visible; textBoxReason.Visibility = Visibility.Visible;
                        denyConfirm.Visibility = Visibility.Visible;  break; }
                case MessageBoxResult.No: return;
            }
        }

        //Обработчик кнопки "Отмена"
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


        //Метод для проверки ввода (по базовому алфавиту и (опционально) дополнительным символам)
        void CheckInput(TextBox box, char[] alphabet, params char[] additionalSymbols)
        {
            box.Background = white;
            if (box.Text == "") { box.Background = red; return; }
            box.Text = box.Text.ToUpper();

            char[] newAlphabet;
            if (additionalSymbols.Count() == 0) newAlphabet = alphabet;
            else
            {
                List<char> temp = new List<char>(alphabet);
                foreach (var s in additionalSymbols) temp.Add(s);
                newAlphabet = temp.ToArray();
            }
            foreach(char c in box.Text)
            {
                if (newAlphabet.Contains(c)) continue;
                else box.Background = red;
            }
        }

        //Проверка корректности ввода фамилии - только буквы кириллицей и символ"-" (н., Римский-Корсаков)
        private void TextBoxSurname_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckInput(textBoxSurname, References.alphabet1, new char[] { '-' });
        }
        //Проверка корректности ввода имени - только буквы кириллицей
        private void TextBoxName_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckInput(textBoxName, References.alphabet1);
        }
        //Проверка корректности ввода отчества - только буквы кириллицей и символ " " (н., Актай Оглы)
        private void TextBoxPatronymic_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckInput(textBoxPatronymic, References.alphabet1, new char[] { ' ' });
        }
        //Проверка корректности ввода кода подразделения - только цифры и символ "-" (н., 162-010)
        private void TextBoxCode_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxCode.Text = textBoxCode.Text.Replace(" ", "");
            CheckInput(textBoxCode, References.alphabet4, new char[] { '-' });
            flag = true;
            for (int n = 0; n < textBoxCode.Text.Length; n++)
            {
                if (n == 3) continue;
                if (!References.alphabet4.Contains(textBoxCode.Text[n])) flag = false;
            }
            if (textBoxCode.Text.Length == 7) { if (textBoxCode.Text[3] != '-') flag = false; }
            if (!flag) textBoxCode.Background = red;
        }

        //Метод для проверки корректности ввода дат и обеспечения единообразия отображения даты
        void DateInputCheck (TextBox box)
        {
            ToCar.IsEnabled = true;
            box.Background = white;
            List<char> alphabet = new List<char>(References.alphabet4);
            string correctFormat = "";
            foreach (char c in box.Text)
            {
                if (!alphabet.Contains(c)) continue;
                else correctFormat += c;
            }
            if (correctFormat.Length == 8)
            {
                correctFormat = correctFormat.Insert(2, ".").Insert(5, ".");
                box.Text = correctFormat;
                try
                {
                    if (DateTime.Now < Convert.ToDateTime(box.Text))
                    {
                        MessageBox.Show("Введенная дата превышает текущую. Проверьте данные", "Предупреждение");
                        box.Background = red;
                        ToCar.IsEnabled = false;
                    }
                }
                catch
                {
                    box.Background = red;
                    return;
                }
            }
        }

        //Проверка указанной даты рождения клиента и соответствия условиям банка возраста клиента (не менее 21 года)
        private void TextBoxBirthday_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck(textBoxBirthday);
            try
            {
                if (DateTime.Now.Year - Convert.ToDateTime(textBoxBirthday.Text).Year < 21)
                {
                    MessageBox.Show("Клиент моложе 21 года и не удовлетворяет условиям банка. Продолжение работы с заявкой" +
                        " невозможно. Проверьте корректность ввода", "Предупреждение");
                    throw new Exception();
                }
            }
            catch
            {
                textBoxBirthday.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка корректности ввода даты выдачи водительского удостоверения и срока его действия (текущая дата не более
        //чем на 10 лет превышает дату выдачи)
        private void TextBoxDriverLicenseDate_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxDriverLicenseDate.Background = white;
            DateInputCheck(textBoxDriverLicenseDate);
            try
            {
                DateTime temp = Convert.ToDateTime(textBoxDriverLicenseDate.Text).AddYears(10);
                if (DateTime.Now > temp)
                {
                    MessageBox.Show("Водительское удостоверение просроченно. Продолжение работы с заявкой" +
                        " невозможно. Проверьте корректность ввода", "Предупреждение");
                    throw new Exception();
                }
            }
            catch
            {
                textBoxDriverLicenseDate.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка корректности ввода даты выдачи паспорта и срока его действия (паспорт был заменен в 20/45 лет)
        private void TextBoxDate_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxDate.Background = white;
            DateInputCheck(textBoxDate);

            try
            {
                DateTime current = Convert.ToDateTime(textBoxDate.Text);
                DateTime temp20 = Convert.ToDateTime(textBoxBirthday.Text).AddYears(20);
                DateTime temp45 = Convert.ToDateTime(textBoxBirthday.Text).AddYears(45);
                if (DateTime.Now > temp45 & current < temp45 || DateTime.Now > temp20 & current < temp20)
                {
                    MessageBox.Show("Паспорт просрочен. Продолжение работы с заявкой" +
                        " невозможно. Проверьте корректность ввода", "Предупреждение");
                    textBoxDate.Background = red;
                    ToCar.IsEnabled = false;
                }
            }
            catch
            {
                textBoxDate.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка корректности ввода даты регистрации (не может быть раньше даты рождения)
        private void TextBoxRegistrationDate_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationDate.Background = white;
            DateInputCheck(textBoxRegistrationDate);

            try
            {
                DateTime birthday = Convert.ToDateTime(textBoxBirthday.Text);
                DateTime input = Convert.ToDateTime(textBoxRegistrationDate.Text);

                if (input < birthday)
                {
                    MessageBox.Show("Введена ошибочная дата регистрации клиента по месту " +
                        "проживания. Продолжение работы с заявкой невозможно. Проверьте корректность ввода",
                        "Предупреждение");
                    textBoxRegistrationDate.Background = red;
                    ToCar.IsEnabled = false;
                }
            }
            catch
            {
                textBoxRegistrationDate.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка корректности ввода даты регистрации брака (не может быть раньше исполнения 14 лет)
        private void TextBoxWeddingDate_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxWeddingDate.Background = white;
            DateInputCheck(textBoxWeddingDate);

            try
            {
                DateTime birthday = Convert.ToDateTime(textBoxBirthday.Text);
                DateTime input = Convert.ToDateTime(textBoxWeddingDate.Text);

                if (input < birthday.AddYears(14))
                {
                    MessageBox.Show("Введена ошибочная дата заключения брака " +
                        "проживания. Продолжение работы с заявкой невозможно. Проверьте корректность ввода",
                        "Предупреждение");
                    textBoxWeddingDate.Background = red;
                    ToCar.IsEnabled = false;
                }
            }
            catch
            {
                textBoxWeddingDate.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка корректности ввода даты прозовона мобильного телефона
        private void TextBoxMobilePhoneDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck(textBoxMobilePhoneDate);
        }
        //Проверка корректности ввода даты прозовона рабочего телефона
        private void TextBoxWorkPhoneDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck(textBoxWorkPhoneDate);
        }
        //Проверка корректности ввода даты прозовона телефона супруги
        private void TextBoxSpousePhoneDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck(textBoxSpousePhoneDate);
        }
        //Проверка корректности ввода даты прозовона дополнительного телефона
        private void TextBoxAdditionalPhoneDate_LostFocus(object sender, RoutedEventArgs e)
        {
            DateInputCheck(textBoxAdditionalPhoneDate);
        }


        //Метод для проверки корректности ввода данных о финансах
        void FinanceCheck(TextBox box)
        {
            ToCar.IsEnabled = true;
            box.Background = white;
            try
            {
                Convert.ToDouble(box.Text);
            }
            catch
            {
                box.Background = red;
                ToCar.IsEnabled = false;
            }
        }
        //Проверка данных в поле "Основной доход"
        private void TextBoxGeneralIncome_LostFocus(object sender, RoutedEventArgs e)
        {
            FinanceCheck(textBoxGeneralIncome);
        }
        //Проверкаа данных в поле "Дополнительный доход"
        private void TextBoxAdditionalIncome_LostFocus(object sender, RoutedEventArgs e)
        {
            FinanceCheck(textBoxAdditionalIncome);
        }
        //Проверкаа данных в поле "Общие ежемесячные расходы"
        private void TextBoxExpences_LostFocus(object sender, RoutedEventArgs e)
        {
            FinanceCheck(textBoxExpences);
        }


        //Функция для проверки данных в полях предусматривающих ввод только цифр с фиксированным количеством символов:
        void CheckData(TextBox box, int correctLength)
        {
            ToCar.IsEnabled = true;
            box.Background = white;

            try
            {
                if(box.Text.Length != correctLength) { throw new Exception(); }
                foreach (char c in box.Text) { Convert.ToInt32(c); }
            }
            catch
            {
                box.Background = red;
                ToCar.IsEnabled = false;
            }
        }

        //Проверка данных в поле "серия" паспорта
        private void TextBoxPassportSeries_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxPassportSeries.Text = textBoxPassportSeries.Text.Replace(" ", "");
            CheckData(textBoxPassportSeries, 4);
        }
        //Проверка данных в поле "номер" паспорта
        private void TextBoxPassportNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckData(textBoxPassportNumber, 6);
        }
        //Проверка данных в поле "серия" ВУ
        private void TextBoxDriverLicenseSeries_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxDriverLicenseSeries.Text = textBoxDriverLicenseSeries.Text.Replace(" ", "");
            CheckData(textBoxDriverLicenseSeries, 4);
        }
        //Проверка данных в поле "номер" ВУ
        private void TextBoxDriverLicenseNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckData(textBoxDriverLicenseNumber, 6);
        }
        //Проверка данных в поле "ИНН работодателя"
        private void TextBoxEmployerInn_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmployerInn.Text == "") return;
            else
            {
                CheckData(textBoxEmployerInn, 10);
                if (ToCar.IsEnabled == false) { CheckData(textBoxEmployerInn, 12); }
            }
        }


        //Метод для проверки корректности ввода телефонных номеров и обеспечения единоробразия ввода: всего 10 цифр, из них первой могут быть только 3, 4, 8, 9
        void PhoneCheck(TextBox box)
        {
            if(box.Text == "") { return; }
            box.Background = white;
            ToCar.IsEnabled = true;

            char[] alphabet = References.alphabet4;
            string correctNumber = "";
            foreach(char c in box.Text)
            {
                if(alphabet.Contains(c)) { correctNumber += c; }
            }
            correctNumber = correctNumber.TrimStart('+', '7');
            try
            {
                if (correctNumber.Length == 10 & (correctNumber[0] == '3' || correctNumber[0] == '4' || correctNumber[0] == '8' || correctNumber[0] == '9'))
                {
                    box.Text = correctNumber;
                }
                else { throw new Exception(); } 
            }
            catch
            {
                box.Background = red;
                ToCar.IsEnabled = false;
            }
        }
        //Проверка мобильного телефона
        private void TextBoxMobilePhone_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneCheck(textBoxMobilePhone);
        }
        //Проверка телефона супруга/супруги
        private void TextBoxSpousePhone_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneCheck(textBoxSpousePhone);
        }
        //Проверка рабочего телефона
        private void TextBoxWorkPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneCheck(textBoxWorkPhone);
        }
        //Проверка дополнительного телефона
        private void TextBoxAdditionalPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneCheck(textBoxAdditionalPhone);
        }

        private void DenyConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxReason.Text != "")
            {
                MessageBox.Show("Заявке присвоен статус - Отказ", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                MakeOrder3.denyOrder = true;
                MakeOrder3.comment = textBoxReason.Text;

                textBoxReason.IsEnabled = false;
                deny.IsEnabled = false;
                denyConfirm.IsEnabled = false;
                ToCar.IsEnabled = false;
            }
            else { textBoxReason.Background = red; return; }
        }



        //Метод для центрирования загруженного скана документа в центре поля (в случае несовпадения пропорций)
        void SetImageToCenter()
        {
            if (canvas.Children.Contains(selRect)) { canvas.Children.Remove(selRect); }
            double width = scan.Source.Width;
            double multiplierWidth = width / canvas.ActualWidth;
            double height = scan.Source.Height;
            double multiplierHeight = height / canvas.ActualHeight;
            double multiplier = 0;
            if (multiplierWidth > multiplierHeight) multiplier = multiplierWidth;
            else multiplier = multiplierHeight;
            Canvas.SetLeft(scan, (canvas.ActualWidth - width / multiplier) / 2);
            Canvas.SetTop(scan, (canvas.ActualHeight - height / multiplier) / 2);
        }

        //Обработчик кнопки "Открыть файл"
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog passportImage = new OpenFileDialog();// создаем диалоговое окно
                passportImage.ShowDialog();// открываем окно Проводника
                string fileName = passportImage.FileName;// берем полный адрес выбранного файла     

                uriOriginalImage = new Uri($@"{fileName}");
                scan.Source = new BitmapImage(uriOriginalImage); // грузим картинку в pictureBox

                SetImageToCenter();    //центрируем картинку в поле

                openFile.Visibility = Visibility.Hidden;
                scanDocument.Visibility = Visibility.Hidden;
                ocrDocument.Visibility = Visibility.Visible;
                turnLeft.Visibility = Visibility.Visible;
                turnRight.Visibility = Visibility.Visible;
                cut.Visibility = Visibility.Visible;
                clear.Visibility = Visibility.Visible;

                //Делаем техническую копию файла в каталоге программы (для трансформации пользователем)
                //Копируем исходный файл в каталог программы под именем tempStart.jpg
                File.Copy(fileName, "tempStart.jpg", true);
            }
            catch { scan.Source = null; }
        }


        //Реализация сканирования
        //Обработчик кнопки "Сканировать документ"
        private void ScanDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //сканирование
                twain.Acquire();

                //Определяем последний файл в каталоге сканирования
                DateTime date = new DateTime(1990, 01, 01);
                string directory = @"C:\Users\XtremeStation\Pictures\ControlCenter4\Scan";
                string file = "";
                FileSystemInfo[] fsi = new DirectoryInfo(directory).GetFileSystemInfos();
                foreach (var f in fsi)
                {
                    if (date < Convert.ToDateTime(f.CreationTime))
                    {
                        date = Convert.ToDateTime(f.CreationTime);
                        file = f.Name;
                    }
                }

                scan.Source = new BitmapImage(new Uri($@"{directory}" + $@"\{file}")); // подгрузка изображения на страницу
                openFile.Visibility = Visibility.Hidden;
                scanDocument.Visibility = Visibility.Hidden;
                ocrDocument.Visibility = Visibility.Visible;
                turnLeft.Visibility = Visibility.Visible;
                turnRight.Visibility = Visibility.Visible;
                cut.Visibility = Visibility.Visible;
                clear.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        //Обработчик кнопки поворота влево на 90 градусов (т.е. на 270 градусов по часовой стрелке)
        private void TurnLeft_Click(object sender, RoutedEventArgs e)
        {
            string originalFilePath = "tempStart.jpg";
            string temp = "temp.jpg";

            var stream = File.OpenRead(originalFilePath);   //Считываем в поток исходный файл
            BitmapImage bitmap = new BitmapImage();         
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;  //включение кэширования изображения. Так можно обойти особенность классов Bitmap и BitmapImage - при создании
            //экзмепляров классов из указанного файла они не закрывают поток, что не позволяет повторно обратиться к этому файлу. Вместо этого работа идет с кэшем
            bitmap.StreamSource = stream;                   //наполнение экземпляра BitmapImage из потока
            bitmap.Rotation = Rotation.Rotate270;           //поворото на -90 градусов (т.е. 270 градусов по часовой стрелке)
            bitmap.EndInit();
            stream.Close();                                 //закрытие потока

            //Сохранение трансформированного изображения в технической копии файла
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream fs = new FileStream(@"temp.jpg", FileMode.OpenOrCreate)) { encoder.Save(fs); }
            File.Copy(temp, originalFilePath, true);        //заменяем начальный файл трансформированным (поворот на 270 градусов)

            scan.Source = bitmap;                           //изменяем источник Image на обработанную картинку
            SetImageToCenter();                             //Центрируем
        }

        //Обработчик кнопки поворота вправо
        private void TurnRight_Click(object sender, RoutedEventArgs e)
        {
            string originalFilePath = "tempStart.jpg";
            string temp = "temp.jpg";

            var stream = File.OpenRead(originalFilePath);   
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;                   
            bitmap.Rotation = Rotation.Rotate90;           
            bitmap.EndInit();
            stream.Close();                                 

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream fs = new FileStream(@"temp.jpg", FileMode.OpenOrCreate)) { encoder.Save(fs); }
            File.Copy(temp, originalFilePath, true);        
            scan.Source = bitmap;
            SetImageToCenter();
        }

        //Обработчик кнопки "Очистить"
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Contains(selRect)) { canvas.Children.Remove(selRect); }

            scan.Source = null;
            openFile.Visibility = Visibility.Visible;
            scanDocument.Visibility = Visibility.Visible;
            ocrDocument.Visibility = Visibility.Hidden;
            turnLeft.Visibility = Visibility.Hidden;
            turnRight.Visibility = Visibility.Hidden;
            cut.Visibility = Visibility.Hidden;
            clear.Visibility = Visibility.Hidden;
        }

        //Метод для подготовки нового объекта типа BitmapImage
        BitmapImage MakeBitmapImage(string filename)
        {
            //Загружаем картинку для распознавания
            var stream = File.OpenRead(filename);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();
            return bitmap;
        }

        //Метод для распознавания текста
        private void Ocr()
        {
            //Загружаем картинку для распознавания
            BitmapImage bitmap = MakeBitmapImage("temp.jpg");

            ScanFragment(bitmap, 0.09, 0.08, 0.8, 0.125, textBoxDivision);
            ScanFragment(bitmap, 0.155, 0.198, 0.21, 0.055, textBoxDate);
            ScanFragment(bitmap, 0.555, 0.198, 0.28, 0.055, textBoxCode);
            ScanFragment(bitmap, 0.42, 0.55, 0.39, 0.055, textBoxSurname);
            ScanFragment(bitmap, 0.39, 0.635, 0.41, 0.035, textBoxName);
            ScanFragment(bitmap, 0.41, 0.675, 0.4, 0.035, textBoxPatronymic);
            ScanFragment(bitmap, 0.56, 0.715, 0.29, 0.035, textBoxBirthday);
            ScanFragment(bitmap, 0.3, 0.745, 0.58, 0.115, textBoxPlaceOfBirth);
            ScanFragment(bitmap, 0.25, 0.7, 0.22, 0.035, radioButtonMale);
            turnLeft.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            bitmap = MakeBitmapImage("temp.jpg");
            ScanFragment(bitmap, 0.13, -0.03, 0.12, 0.05, textBoxPassportSeries);
            ScanFragment(bitmap, 0.25, -0.03, 0.2, 0.05, textBoxPassportNumber);
            turnRight.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            bitmap = MakeBitmapImage("temp.jpg");
        }

        //Метод для извлечения указанного фрагмента изображения, распознавания текста и передачи информации в указанное текстовое поле
        void ScanFragment(BitmapImage bitmap, double startPointX, double startPointY, double widthRectangle, double heightRectangle, Control control)
        {
            double width = bitmap.PixelWidth;
            double height = bitmap.PixelHeight;

            double x = Canvas.GetLeft(scan) + width * startPointX;
            double y = Canvas.GetTop(scan) + height * startPointY;
            System.Windows.Point pointLeftTop = new System.Windows.Point(x, y);

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Width = width * widthRectangle;
            rectangle.Height = height * heightRectangle;

            CroppedBitmap cropped = new CroppedBitmap(bitmap, new Int32Rect((int)pointLeftTop.X, (int)pointLeftTop.Y, (int)(rectangle.Width), (int)(rectangle.Height)));

            //Сохранение трансформированного изображения в технической копии файла
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(cropped));
            using (FileStream fs = new FileStream(@"toScan.jpg", FileMode.OpenOrCreate)) { encoder.Save(fs); }

            try
            {
                //Конвертируем из BitmapImage в Bitmap
                engine.DefaultPageSegMode = PageSegMode.Auto;
                var stream = File.OpenRead("toScan.jpg");
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                stream.Close();

                Bitmap bt = FromBitmapImageToBitmap(bitmapImage);
                //Конвертируем из Bitmap в Pix (класс из пакета Tesseract)
                PixConverter.ToPix(bt);

                //Непосредственно распознавание текста с использованием пакета Tesseract
                using (Tesseract.Page page = engine.Process(PixConverter.ToPix(bt)))
                {
                    string result = page.GetText().ToUpper();
                    string temp = "";
                    foreach (char c in result)
                    {
                        if (References.alphabet6.Contains(c))
                        {
                            if (c == '—') { temp += "-"; continue; }
                            temp += c;
                        }
                    }
                    temp.Trim(' ');
                    //result = Regex.Replace(result, @"[ \r\n\t\s]", "");
                    if (control.GetType() == typeof(TextBox)) { ((TextBox)control).Text = temp; }
                    else
                    {
                        if (temp.Contains("М") || temp.Contains("У")) ((RadioButton)radioButtonMale).IsChecked = true;
                        else ((RadioButton)radioButtonFemale).IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Обработчик кнопки "Распознать"
        private void OcrDocument_Click(object sender, RoutedEventArgs e)
        {
            Ocr();
        }

        //Обработчик нажатия кнопки мыши в окне паспорта
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                //если ранее на картинке было сделано выделение, то удаляем его
                if (selRect != null) canvas.Children.Remove(selRect);
                //вычисляем и сохраняем координаты точки нажатия
                int positionX = (int)(Math.Floor(e.GetPosition(canvas).X));
                int positionY = (int)(Math.Floor(e.GetPosition(canvas).Y));
                startSelection = new System.Windows.Point(positionX, positionY);
            }
        }

        //Обработчик движения мыши в окне паспорта
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //если зажата левая кнопка мыши...
            if (e.LeftButton.ToString() == "Pressed")
            {
                //если ранее на изображении был нарисован прямоугольник выделения, то убираем его
                if (canvas.Children.Contains(selRect)) { canvas.Children.Remove(selRect);}
                selRect = new System.Windows.Shapes.Rectangle();      //создаем новый прямоугольник
                selRect.Stroke = System.Windows.Media.Brushes.Red;   //с красными линиями
                selRect.StrokeThickness = 2;    //толщиной 2
                int positionX = (int)(Math.Floor(e.GetPosition(canvas).X));     //считываем текущую координату X
                int positionY = (int)(Math.Floor(e.GetPosition(canvas).Y));     //считываем текущую кординату Y
                
                //Предусматривается вариант, когда по оси X мышь движется вправо...
                if(positionX > startSelection.X)
                {
                    Canvas.SetLeft(selRect, startSelection.X);
                    selRect.Width = positionX - startSelection.X;
                }
                //...и влево
                else
                {
                    Canvas.SetLeft(selRect, positionX);
                    selRect.Width = startSelection.X - positionX;
                }

                //Предусматривается вариант, когда по оси Y мышь движется влево...
                if (positionY > startSelection.Y)
                {
                    Canvas.SetTop(selRect, startSelection.Y);
                    selRect.Height = positionY - startSelection.Y;
                }
                //...и влево
                else
                {
                    Canvas.SetTop(selRect, positionY);
                    selRect.Height = startSelection.Y - positionY;
                }
                
                //Добавляем готовый прямоугольник на панель с загруженным материалом
                canvas.Children.Add(selRect);
            }
        }

        //Обработчик кнопки "Обрезать"
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            string originalFilePath = "tempStart.jpg";
            string temp = "temp.jpg";

            var stream = File.OpenRead(originalFilePath);   
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            stream.Close();                                 

            if (!canvas.Children.Contains(selRect)) { return; }
            else
            {
                //Расчет коэффициента масштабирования
                double width = image.PixelWidth;
                double multiplierWidth = width / canvas.ActualWidth;
                double height = image.PixelHeight;
                double multiplierHeight = height / canvas.ActualHeight;
                double multiplier = 0;
                if (multiplierWidth > multiplierHeight) multiplier = multiplierWidth;
                else multiplier = multiplierHeight;

                //определение начальной точки выделения на реальном изображении с учетом коэффициента масштабирования
                //1.Определяется верхний левый угол прямоугольника выделения относительно верхнего левого угла Canvas
                double x = Canvas.GetLeft(selRect);
                double y = Canvas.GetTop(selRect);
                System.Windows.Point pointCanvas = new System.Windows.Point(x, y);

                //2.Расчет до какой степени масштабировано изображение при загрузке в Canvas 
                double miniImageWidth = image.PixelWidth / multiplier;
                double miniImageHeight = image.PixelHeight / multiplier;

                //3.Учитываем свободную площадь в окне, которую не заняло изображение (если не совпадают пропорции)
                double deltaX = (canvas.ActualWidth - miniImageWidth) / 2;
                double deltaY = (canvas.ActualHeight - miniImageHeight) / 2;

                //4.Учитываем расстояние от края загруженной картинки до верхнего левого угла прямоугольника выделения
                //и корректируем на коэффициент масштабирования
                double pointX_Bitmap = (pointCanvas.X - deltaX) * multiplier;
                if (pointX_Bitmap < 0) pointX_Bitmap = 0;
                double pointY_Bitmap = (pointCanvas.Y - deltaY) * multiplier;
                if (pointY_Bitmap < 0) pointY_Bitmap = 0;
                System.Windows.Point pointBitmap = new System.Windows.Point(pointX_Bitmap, pointY_Bitmap);

                //5. - определяется выделенная область на ИСХОДНОМ изображении (с учетом коэффициента масштабирования)
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Width = selRect.Width * multiplier;
                rectangle.Height = selRect.Height * multiplier;

                //6.Создается новый источник изображения для обрезки
                stream = File.OpenRead(originalFilePath);
                image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                stream.Close();

                //7.Создается новое изображение ("обрезок" исходного)
                CroppedBitmap cropped = new CroppedBitmap(image, new Int32Rect((int)pointBitmap.X, (int)pointBitmap.Y, (int)(rectangle.Width), (int)(rectangle.Height)));
                scan.Source = cropped;

                //Сохранение трансформированного изображения в технической копии файла
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(cropped));
                using (FileStream fs = new FileStream(@"temp.jpg", FileMode.OpenOrCreate)) { encoder.Save(fs); }
                File.Copy(temp, originalFilePath, true);        //заменяем начальный файл трансформированным (поворот на 270 градусов)

                canvas.Children.Remove(selRect);
                SetImageToCenter();
            }
        }


        //Метод конвертации из Bitmap в BitmapImage
        BitmapImage FromBitmapToBitmapImage(Bitmap image)
        {
            BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return (BitmapImage)source;
        }


        //Метод конвертации из BitmapImage в Bitmap
        Bitmap FromBitmapImageToBitmap(BitmapImage image)
        {
            using (MemoryStream stream  = new MemoryStream())
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                Bitmap bitmap = new Bitmap(stream);
                return new Bitmap(bitmap);
            }
        }

        private void TextBoxDivision_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxDivision.Text = textBoxDivision.Text.ToUpper();
        }

        //Обработчик RadioButton, подтверждающего фактическое проживание клиента по адресу прописки
        private void RadioButtonAdressMatchYes_Checked(object sender, RoutedEventArgs e)
        {
            textBoxFactIndex.Text = textBoxRegistrationIndex.Text;
            textBoxFactRegion.Text = textBoxRegistrationRegion.Text;
            textBoxFactDistrict.Text = textBoxRegistrationDistrict.Text;
            textBoxFactLocality.Text = textBoxRegistrationLocality.Text;
            textBoxFactSubordinateLocality.Text = textBoxRegistrationSubordinateLocality.Text;
            textBoxFactStreet.Text = textBoxRegistrationStreet.Text;
            textBoxFactHome.Text = textBoxRegistrationHome.Text;
            textBoxFactApartment.Text = textBoxRegistrationApartment.Text;
        }

        //Обработчик RadioButton, отрицающего фактическое проживание клиента по адресу прописки
        private void RadioButtonMatchNo_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var c in factLiving.Children)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Text = "";
                }
            }
        }

        private void RadioButtonIncomeConfirmed_Checked(object sender, RoutedEventArgs e)
        {
            comboBoxDocumentIncomeConfirmation.Visibility = Visibility.Visible;
        }

        private void RadioButtonNotIncomeConfirmed_Checked(object sender, RoutedEventArgs e)
        {
            comboBoxDocumentIncomeConfirmation.Visibility = Visibility.Hidden;
        }

        private void RadioButtonNotMarried_Checked(object sender, RoutedEventArgs e)
        {
            textBoxWeddingDate.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        }

        //Автоматическое добавление текущей даты в поле ввода даты прозвона
        private void TextBoxMobilePhoneDate_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxMobilePhoneDate.Text = DateTime.Now.ToShortDateString();
        }
        private void TextBoxWorkPhoneDate_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxWorkPhoneDate.Text = DateTime.Now.ToShortDateString();
        }
        private void TextBoxSpousePhoneDate_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxSpousePhoneDate.Text = DateTime.Now.ToShortDateString();
        }
        private void TextBoxAdditionalPhoneDate_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxAdditionalPhoneDate.Text = DateTime.Now.ToShortDateString();
        }

        private void TextBoxMobilePhoneResponder_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxMobilePhoneResponder.Text = "Клиент";
        }

        private void TextBoxPlaceOfBirth_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxPlaceOfBirth.Text = textBoxPlaceOfBirth.Text.ToUpper();
        }
    }
}
