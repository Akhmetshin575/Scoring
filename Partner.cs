using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class Partner
    {
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public string TypeOfCounterParty { get; set; }
        public string Bic { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Rs { get; set; }
        public string Ks { get; set; }
        public string Bank { get; set; }
        public string Town { get; set; }
        public string Product { get; set; }

        //Создание связи типа многие ко многим с таблицей городов присутствия. См.комментарий в классе CityOfPresence
        public ICollection<City> Cities { get; set; }
        public ICollection<ServiceAndInsurance> ServicesAndInsurances { get; set; }

        public Partner()
        {
            Cities = new List<City>();
            ServicesAndInsurances = new List<ServiceAndInsurance>();
        }

        public Partner(string name, string type, string bic, string inn, string rs, string ks, string bank, string kpp)
        {
            Name = name.ToUpper();
            TypeOfCounterParty = type;
            Bic = bic;
            Inn = inn;
            Rs = rs;
            Ks = ks;
            Bank = bank;
            Kpp = kpp;
            Cities = new List<City>();
            ServicesAndInsurances = new List<ServiceAndInsurance>();
        }

        public Partner(int partnerId, string name, string typeOfCounterParty, string products, string cities)
        {
            PartnerId = partnerId;
            Name = name;
            TypeOfCounterParty = typeOfCounterParty;
            Town = cities;
            Cities = new List<City>();
            Product = products;
            ServicesAndInsurances = new List<ServiceAndInsurance>();
        }
    }
}
