using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Scoring3
{
    class UserContext3 : DbContext
    {
        //наследуем конструктор базового класса. "DbConnection" - имя строки подключения к БД
        public UserContext3() : base("DbConnection3") { }

        //Определяем сущности (таблицы в БД)
        public DbSet<Car> Cars { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ServiceAndInsurance> ServicesAndInsurances { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
    }
}
