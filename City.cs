using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }

        //создание связи типа многие ко многим:
        //1. С таблицей работников. Т.е. к одному городу может быть привязано несколько работников.
        //Но и каждый работник может иметь доступ в программе более чем к одному городу.
        //2. С таблицей контрагентов. Т.е. в одном городе как правило работает множество контрагентов.
        //Но и каждый контрагент работает (обычно) более чем в одном городе.
        public ICollection<Worker> Workers { get; set; }
        public ICollection<Partner> Partners { get; set; }
        public City()
        {
            Workers = new List<Worker>();
            Partners = new List<Partner>();
        }

        public City(string city)
        {
            Name = city;
            Workers = new List<Worker>();
            Partners = new List<Partner>();
        }

        public override string ToString()
        {
            string temp = Name + ", ";
            return temp;
        }
    }
}
