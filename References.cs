using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    class References
    {
        //Перечень городов доступных в АИС
        public static List<string> citiesList = new List<string> {"Альметьевск",
                                                                  "Елабуга",
                                                                  "Набережные Челны",
                                                                  "Нижнекамск",
                                                                  "Казань",
                                                                  "Чистополь"};

        //Перечень возможных результатов прозвона телефонов клиента
        public static List<string> resultOfCallList = new List<string> {"Не выбрано",
                                                                        "Информация подтверждена",
                                                                        "Информация частично подтверждена",
                                                                        "Нет дозвона",
                                                                        "Информация не подтверждена"};

        //Перечень возможных типов контрагентов
        public static List<string> typeOfPartner = new List<string> {"Автосалон",
                                                                     "Страховщик",
                                                                     "Услуги"};

        //Перечень возможных видов образования клиента
        public static List<string> typeOfEducation = new List<string> {"Не выбрано",
                                                                       "Начальное или неполное среднее",
                                                                       "Среднее, в т.ч. специальное",
                                                                       "Неполное высшее",
                                                                       "Высшее",
                                                                       "Ученая степень"};

        //Перечень возможных типов занимаемой должности клиента
        public static List<string> typeOfPost = new List<string> {"Не выбрано",
                                                                  "Неруководящий работник",
                                                                  "Руководитель (и его заместитель)",
                                                                  "Руководитель подразделения/филиала (и его заместитель)",
                                                                  "Руководитель отдела/цеха/бригады (и его заместитель)"};

        //Перечень возможных видов подтверждения дохода клиента
        public static List<string> typeOfIncomeConfirmation = new List<string> {"Не выбрано",
                                                                                "Справка 2-НДФЛ",
                                                                                "Справка по форме банка",
                                                                                "Налоговая декларация/патент",
                                                                                "Выписка со счета"};

        //Тип социального статуса клиента
        public static List<string> socialStatus = new List<string> {"Не выбрано",
                                                                    "Работает/служит",
                                                                    "Работающий пенсионер",
                                                                    "Пенсионер",
                                                                    "Не работает"};

        //Должность сотрудника
        public static List<string> post = new List<string> {"Консультант",
                                                            "Кредитный эксперт",
                                                            "Старший кредитный эксперт",
                                                            "Ведущий кредитный эксперт",
                                                            "Директор"};

        //Вид документа на автомобиль
        public static List<string> typeOfDocument = new List<string> {"Не выбрано",
                                                                      "Электронный ПТС",
                                                                      "ПТС",
                                                                      "Форма учета транспортного средства"};

        //Тип двигателя автомобиля
        public static List<string> typeOfMotor = new List<string> {"Не выбрано",
                                                                   "Бензиновый",
                                                                   "Дизельный",
                                                                   "Газовый",
                                                                   "Электрический"};

        //Экологический класс автомобиля
        public static List<int> ecologicalClass = new List<int> {0, 1, 2, 3, 4, 5, 6 };

        //Срок кредита
        public static List<int> creditPeriod = new List<int> { 0, 12, 24, 36, 48, 60, 72, 84 };

        //Количество иждивенцев
        public static List<string> numberOfDependents = new List<string> { "Не выбрано", "0", "1", "2", "3", "4",
                                                                           "5", "6", "7", "8", "9", "10 и более" };

        //Стаж
        public static List<string> experience = new List<string> { "Не выбрано", "Менее 3 мес.", "3-6 мес.", "6-12 мес.",
                                                                   "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                                                                   "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                                   "20", "21", "22", "23", "24", "25", "26", "27", "28",
                                                                   "29", "30", "31", "32", "33", "34", "35", "36", "37",
                                                                   "38", "39", "40 и более" };

        //Тип продукта добавляемого в сумму кредита
        public static List<string> typeOfAdditionalProduct = new List<string> { "Страховка", "Услуга" };

        //Статус заявки
        public static List<string> statusOfOrder = new List<string> {"Не рассматривалась",
                                                                     "На рассмотрении",
                                                                     "Одобрено",
                                                                     "Отказано",
                                                                     "Возврат на доработку/Альтернитивное предложение",
                                                                     "Оформлено"};

        //Тип документа, предоставленного для оформления кредита
        public static List<string> typeOfDocumentForCredit = new List<string> { "Не выбрано",
                                                                                "Счет на полную стоимость а/м",
                                                                                "Договор купли-продажи" };

        //Тип документа, предоставленного для оформления кредита
        public SortedList<string, List<string>> car = new SortedList<string, List<string>>();

        //1 набор допустимых для ввода символов
        public static char[] alphabet1 = new char[] {'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М',
                            'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я' };
        //2 набор допустимых для ввода символов
        public static char[] alphabet2 = new char[] { ' ', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л',
                            'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я' };
        //3 набор допустимых для ввода символов
        public static char[] alphabet3 = new char[] { '-', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л',
                            'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я' };
        //4 набор допустимых для ввода символов
        public static char[] alphabet4 = new char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        //5 набор допустимых для ввода символов
        public static char[] alphabet5 = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        //6 набор допустимых для ввода символов
        public static char[] alphabet6 = new char[] { '№', '-', ' ', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                                                      'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л',
                                                      'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш',
                                                      'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', '.', '—' };


        public References()
        {
            car.Add("Не выбрано", new List<string> { "Не выбрано" });
            car.Add("Hyundai", new List<string>{ "Accent", "Creta", "Elantra", "Eqqus", "Genesis", "Getz", "Santa Fe", "Grand Santa Fe", "Grandeur", "H1", "Solaris"});
            car.Add("KIA", new List<string> { "Carnival", "Ceed", "Cerato", "Sportage", "K5", "K900", "Magentis", "Mohave", "Optima", "Picanto", "RIO" });
            car.Add("Audi", new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "Q3", "Q5", "Q7", "Q8", "R8", });
            car.Add("BMW", new List<string> { "1 series", "2 series", "3 series", "4 series", "5 series", "6 series", "7 series", "8 series",
                                              "M2", "M3", "M5", "M6", "X1", "X2", "X3", "X4", "X5", "X6" });
            car.Add("Changan", new List<string> { "CS35", "CS55", "CS75" });
            car.Add("Chery", new List<string> { "A11", "A13", "A15", "A19", "Amulet", "Exeed", "Fora" });
            car.Add("Nissan", new List<string> { "Almera", "Altima", "Caravan", "Cube" });
            car.Add("Mazda", new List<string> { "1", "2", "3", "323", "5", "6", "CX3", "CX30", "CX5", "CX7", "CX9", "R8" });
            car.Add("Chevrolet", new List<string> { "Aveo", "Blazer", "Camaro", "Captiva", "Cobalt", "Cruze", "Epica", "Evanda", "Lacetti" });
            car.Add("Renault", new List<string> { "Arkana", "Clio", "Duster", "Fluence", "Kaptur", "Koleos", "Logan", "Megane" });
            car.Add("Skoda", new List<string> { "100", "Fabia", "Felicia", "Karoq", "Kodiaq", "Octavia", "Rapid", "Roomster", "Superb", "Yeti" });
            car.Add("Toyota", new List<string> { "Alphard", "Aurus", "Avalon", "Avensis", "C-HR", "Caldina", "Camry", "Celica", "Corolla" });
            car.Add("Subaru", new List<string> { "Forester", "Impreza", "Legacy", "Outback", "WRX", "XV" });
            car.Add("Volkswagen", new List<string> { "Amarok", "Arteon", "Beetle", "Bora", "Caddy", "Crafter", "Golf", "Jetta", "Passat", "Polo" });
            car.Add("ВАЗ", new List<string> { "Granta", "Largus", "Niva", "Vesta", "XRAY", "2190", "2191", "2192", "ВИС 1705", "ВИС 2345", "ВИС 2347" });
            car.Add("УАЗ", new List<string> { "Hunter", "Patriot", "Pickup", "Profi", "Simbir", "3151", "3159", "3741" });
            car.Add("ГАЗ", new List<string> { "3285", "3307", "3310 Валдай", "3512", "A21R22", "A21R25", "A65R32", "A65R35", "C41R11", "C45R02" });
        } 
    }
}
