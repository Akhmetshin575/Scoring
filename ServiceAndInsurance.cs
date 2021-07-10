using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class ServiceAndInsurance
    {
        public int ServiceAndInsuranceId { get; set; }
        public string TypeOfAdditionalProduct { get; set; }
        public string NameOfProduct { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool OnCredit { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationMonths { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }
        public int InsuredSum { get; set; }
        public double Price { get; set; }
        public int PartnerId { get; set; }

        //Создание связи типа один ко многим с таблицей заявок. Т.е. одна заявка может включать любое количество услуг и/или страховок
        public int? OrderId { get; set; }
        public Order Order { get; set; }

        //Создание связи типа многие ко многим с таблицей партнеров. Т.е. один и тот же вид страховок/услуг может предоставляться произвольным количеством пратнеров, равно как и каждый партнер может предоставлять произвольное количество страховок и/или услуг
        public ICollection<Partner> Partners { get; set; }


        public ServiceAndInsurance()
        {
            Partners = new List<Partner>();
        }

        //Конструктор для создания Услуг
        public ServiceAndInsurance( string name, string invoiceNumber, string invoiceDate, bool onCredit, string price, int partnerId )
        {
            TypeOfAdditionalProduct = "Услуга";
            NameOfProduct = name;
            InvoiceNumber = invoiceNumber;
            InvoiceDate = Convert.ToDateTime(invoiceDate);
            OnCredit = onCredit;
            StartDate = DateTime.Now;
            DurationMonths = 0;
            InsuredSum = 0;
            Price = Convert.ToDouble(price);
            PartnerId = partnerId;
        }

        //Конструктор для создания Страховок
        public ServiceAndInsurance(string name, string dateOfPolicyRegistration, bool onCredit, string dateOfBegin,
                                   string duration, string series, string number, string insuredSum, string price,
                                   int partnerId)
        {
            TypeOfAdditionalProduct = "Страхование";
            NameOfProduct = name;
            InvoiceDate = Convert.ToDateTime(dateOfPolicyRegistration);
            OnCredit = onCredit;
            StartDate = Convert.ToDateTime(dateOfBegin);
            DurationMonths = Convert.ToInt32(duration);
            Series = series;
            Number = number;
            InsuredSum = Convert.ToInt32(insuredSum);
            Price = Convert.ToDouble(price);
            PartnerId = partnerId;
        }

        public override string ToString()
        {
            string temp = NameOfProduct + ", ";
            return temp;
        }
    }
}
