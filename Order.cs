using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime DateOfOrder { get; set; }
        public DateTime DateOfCreditIssued { get; set; }
        public string StatusOfOrder { get; set; }
        public int Payment { get; set; }
        public string Comment { get; set; }
        public bool RefusalRequired { get; set; }
        public string RequiestAmount { get; set; }
        public string AcceptableAmount { get; set; }
        public string TypeOfDocumentForCredit { get; set; }
        public string NumberOfDocumentForCredit { get; set; }
        public DateTime DateOfDocumentForCredit { get; set; }
        public string CityOfSale { get; set; }


        public Car Car { get; set; }
        public Client Client { get; set; }
        public Partner Dealer { get; set; }
        public Worker Worker { get; set; }
        public Tariff Tariff { get; set; }

        //Создание связи типа один ко многим с таблицей дополнительных услуг и страховок
        public ICollection<ServiceAndInsurance> ServicesAndInsurances { get; set; }

        public Order()
        {
            ServicesAndInsurances = new List<ServiceAndInsurance>();
        }

        public Order(Car car, Client client, Partner dealer, Worker worker, Tariff tariff, List<ServiceAndInsurance> list,
                     DateTime dateOfOrder, DateTime dateOfIssued, bool refusalRequired)
        {
            DateOfOrder = dateOfOrder;
            DateOfCreditIssued = dateOfIssued;
            StatusOfOrder = "Не рассматривалась";

            int sumOfCredit = car.Price - car.InitialPayment;
            foreach(var s in list) { if(s.OnCredit == true) sumOfCredit += (int)s.Price; }
            RequiestAmount = sumOfCredit.ToString();

            double monthlyPercent = tariff.PercentageRate / 100 / 12;
            Payment = (int)(sumOfCredit * (monthlyPercent + (monthlyPercent / (Math.Pow((1+monthlyPercent), tariff.Months) - 1))));

            Comment = "";
            RefusalRequired = refusalRequired;
            NumberOfDocumentForCredit = "";
            DateOfDocumentForCredit = new DateTime(1900, 1, 1);

            ServicesAndInsurances = new List<ServiceAndInsurance>();
        }
    }
}
