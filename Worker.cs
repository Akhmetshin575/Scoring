using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Scoring3
{
    public class Worker
    {
        string passwordEncrypted = "";
        public int WorkerId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Post { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool FirstEntry { get; set; }
        public string Town { get; set; }

        //Создание связи типа многие ко многим с таблицей городов присутствия. См.комментарий в классе CityOfPresence
        public ICollection<City> Cities { get; set; }

        public Worker()
        {
            Cities = new List<City>();
        }

        public Worker(string surname, string name, string patronymic, string post, string phone)
        {
            string cyrillic = surname.ToLower() + name.ToUpper().First() + patronymic.ToUpper().First();
            cyrillic = cyrillic[0].ToString().ToUpper() + cyrillic.Substring(1);

            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Post = post;
            Phone = phone;
            FirstEntry = true;
            Login = Translite(cyrillic);
            Password = GeneratePassword();
            Cities = new List<City>();

            ////Отправляем новому пользователю сгенерированные логин/пароль для первого входа в систему
            //WebRequest request = WebRequest.Create($"https://sms.ru/sms/send?api_id=6C92712A-0BE4-E36B-FD7A-0714E11F371A&to={phone}&msg=Vas+privetstvuet+Rusfinance+Bank!+Vash+login:+{Login},+parol:+{passwordEncrypted}&json=1");
            //request.Method = "POST";    //метод запроса (здесь POST)
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Close();
        }

        public Worker(int id, string surname, string name, string patronymic, string post, string phone, string towns)
        {
            WorkerId = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Post = post;
            Phone = phone;
            Town = towns;
            Cities = new List<City>();
        }



        /// <summary>
        /// Метод для автоматической транслитерации ФИО создаваемой записи о сотруднике
        /// </summary>
        /// <param name="cyrillic">ФИО сотрудника на кириллице</param>
        /// <returns>Значение ФИО клиента на латинице</returns>
        public string Translite(string cyrillic)
        {
            string result = "";

            //Транслитерация задана в соответствии с Приказом МВД № 310 (1997—2010)
            Dictionary<char, string> valuePairs = new Dictionary<char, string>();
            valuePairs.Add('А', "A");
            valuePairs.Add('Б', "B");
            valuePairs.Add('В', "V");
            valuePairs.Add('Г', "G");
            valuePairs.Add('Д', "D");
            valuePairs.Add('Е', "E");
            valuePairs.Add('Ё', "Ye");
            valuePairs.Add('Ж', "Zh");
            valuePairs.Add('З', "Z");
            valuePairs.Add('И', "I");
            valuePairs.Add('Й', "Y");
            valuePairs.Add('К', "K");
            valuePairs.Add('Л', "L");
            valuePairs.Add('М', "M");
            valuePairs.Add('Н', "N");
            valuePairs.Add('О', "O");
            valuePairs.Add('П', "P");
            valuePairs.Add('Р', "R");
            valuePairs.Add('С', "S");
            valuePairs.Add('Т', "T");
            valuePairs.Add('У', "U");
            valuePairs.Add('Ф', "F");
            valuePairs.Add('Х', "Kh");
            valuePairs.Add('Ц', "Ts");
            valuePairs.Add('Ч', "Ch");
            valuePairs.Add('Ш', "Sh");
            valuePairs.Add('Щ', "Shch");
            valuePairs.Add('Ъ', "\"");
            valuePairs.Add('Ы', "Y");
            valuePairs.Add('Ь', "'");
            valuePairs.Add('Э', "E");
            valuePairs.Add('Ю', "Yu");
            valuePairs.Add('Я', "Ya");

            valuePairs.Add('а', "a");
            valuePairs.Add('б', "b");
            valuePairs.Add('в', "v");
            valuePairs.Add('г', "g");
            valuePairs.Add('д', "d");
            valuePairs.Add('е', "e");
            valuePairs.Add('ё', "ye");
            valuePairs.Add('ж', "zh");
            valuePairs.Add('з', "z");
            valuePairs.Add('и', "i");
            valuePairs.Add('й', "y");
            valuePairs.Add('к', "k");
            valuePairs.Add('л', "l");
            valuePairs.Add('м', "m");
            valuePairs.Add('н', "n");
            valuePairs.Add('о', "o");
            valuePairs.Add('п', "p");
            valuePairs.Add('р', "r");
            valuePairs.Add('с', "s");
            valuePairs.Add('т', "t");
            valuePairs.Add('у', "u");
            valuePairs.Add('ф', "f");
            valuePairs.Add('х', "kh");
            valuePairs.Add('ц', "ts");
            valuePairs.Add('ч', "ch");
            valuePairs.Add('ш', "sh");
            valuePairs.Add('щ', "shch");
            valuePairs.Add('ъ', "\"");
            valuePairs.Add('ы', "y");
            valuePairs.Add('ь', "'");
            valuePairs.Add('э', "e");
            valuePairs.Add('ю', "yu");
            valuePairs.Add('я', "ya");

            foreach (char c in cyrillic)
            {
                //Выполняется непосредственно транслитерация
                if (valuePairs.ContainsKey(c))
                {
                    result += valuePairs.Where(x => x.Key == c).First().Value;
                }

            }
            return result;
        }

        /// <summary>
        /// Метод для автоматической генерации пароля для первичного входа нового пользователя в систему
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            char[] alphabet1 = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                                            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
            char[] alphabet2 = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
            char[] alphabet3 = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', '*',
                                            '/', '_', '(', ')', '[', ']'};

            Random random = new Random();

            for (int i = 0; i < 8; i++)
            {
                int n = random.Next(1, 4);
                int n1;
                switch (n)
                {
                    case 1:
                        n1 = random.Next(0, alphabet1.Length);
                        passwordEncrypted += alphabet1[n1];
                        break;
                    case 2:
                        n1 = random.Next(0, alphabet2.Length);
                        passwordEncrypted += alphabet2[n1];
                        break;
                    case 3:
                        n1 = random.Next(0, alphabet3.Length);
                        passwordEncrypted += alphabet3[n1];
                        break;
                }
            }

            //Вычисляем хэш значение пароля для сохранения его в БД:
            byte[] original = ASCIIEncoding.ASCII.GetBytes(passwordEncrypted);
            //Вычисление хэша
            byte[] calculate = new MD5CryptoServiceProvider().ComputeHash(original);
            //перевод byte массива с хэшем в строковое представление
            string hashToString = "";
            for (int n = 0; n < calculate.Length; n++)
            {
                hashToString += calculate[n].ToString();
            }

            return hashToString;
        }
    }
}
