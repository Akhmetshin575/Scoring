using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    class PartnerCity
    {
        public int PartnerCityId { get; set; }
        public Partner Partner { get; set; }
        public City City { get; set; }

        public PartnerCity() { }
        public PartnerCity(Partner partner, City city)
        {
            this.Partner = partner;
            this.City = city;
        }
    }
}
