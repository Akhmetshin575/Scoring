using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class Tariff
    {
        public int TariffId { get; set; }
        public string Name { get; set; }
        public int Months { get; set; }
        public double PercentageRate { get; set; }
        public bool CarInsuranceRequired { get; set; }
        public bool TariffForNewCar { get; set; }
        public int PercentOfMinimalInitaialPayment { get; set; }
        public int PercentOfMaximumInitaialPayment { get; set; }
        public string FullName { get; set; }

        public Tariff() {}

        public Tariff(string name, int months, double percent, bool insurance, bool condition, int min, int max)
        {
            Name = name;
            Months = months;
            PercentageRate = percent;
            CarInsuranceRequired = insurance;
            TariffForNewCar = condition;
            PercentOfMinimalInitaialPayment = min;
            PercentOfMaximumInitaialPayment = max;

            //Собираем полное название тарифа:
            FullName += Name + $" ПВ от {PercentOfMinimalInitaialPayment} до {PercentOfMaximumInitaialPayment}";
            if (CarInsuranceRequired == false) FullName += " без КАСКО";
        }
    }
}
