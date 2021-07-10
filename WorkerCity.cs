using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    class WorkerCity
    {
        public int WorkerCityId { get; set; }
        public Worker Worker { get; set; }
        public City City { get; set; }

        public WorkerCity() { }

        public WorkerCity(Worker worker, City city)
        {
            this.Worker = worker;
            this.City = city;
        }
    }
}
