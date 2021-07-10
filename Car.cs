using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string NameFromDocument { get; set; }

        public string TypeOfDocument { get; set; }
        public string DocumentSeries { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDateOfIssue { get; set; }

        public bool NewCar { get; set; }
        public int YearOfRelease { get; set; }
        public int Price { get; set; }
        public int InitialPayment { get; set; }

        public string Vin { get; set; }
        public string Uveos { get; set; }
        public string MotorNumber { get; set; }
        public string BodyNumber { get; set; }
        public double Powerful { get; set; }
        public string TypeOfMotor { get; set; }
        public int EcologicalClass { get; set; }
        public int PermittedWeight { get; set; }
        public string Color { get; set; }
    }
}
