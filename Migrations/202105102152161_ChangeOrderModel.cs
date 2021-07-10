namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "Order_OrderId", c => c.Int());
            AddColumn("dbo.Orders", "Car_CarId", c => c.Int());
            AddColumn("dbo.Orders", "Client_ClientId", c => c.Int());
            AddColumn("dbo.Orders", "Tariff_TariffId", c => c.Int());
            AddColumn("dbo.Orders", "Worker_WorkerId", c => c.Int());
            CreateIndex("dbo.Partners", "Order_OrderId");
            CreateIndex("dbo.Orders", "Car_CarId");
            CreateIndex("dbo.Orders", "Client_ClientId");
            CreateIndex("dbo.Orders", "Tariff_TariffId");
            CreateIndex("dbo.Orders", "Worker_WorkerId");
            AddForeignKey("dbo.Orders", "Car_CarId", "dbo.Cars", "CarId");
            AddForeignKey("dbo.Orders", "Client_ClientId", "dbo.Clients", "ClientId");
            AddForeignKey("dbo.Partners", "Order_OrderId", "dbo.Orders", "OrderId");
            AddForeignKey("dbo.Orders", "Tariff_TariffId", "dbo.Tariffs", "TariffId");
            AddForeignKey("dbo.Orders", "Worker_WorkerId", "dbo.Workers", "WorkerId");
            DropColumn("dbo.Orders", "CarId");
            DropColumn("dbo.Orders", "ClientId");
            DropColumn("dbo.Orders", "CounterPartyId");
            DropColumn("dbo.Orders", "EmployeeId");
            DropColumn("dbo.Orders", "TariffId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "TariffId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "CounterPartyId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "ClientId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "CarId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "Worker_WorkerId", "dbo.Workers");
            DropForeignKey("dbo.Orders", "Tariff_TariffId", "dbo.Tariffs");
            DropForeignKey("dbo.Partners", "Order_OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Client_ClientId", "dbo.Clients");
            DropForeignKey("dbo.Orders", "Car_CarId", "dbo.Cars");
            DropIndex("dbo.Orders", new[] { "Worker_WorkerId" });
            DropIndex("dbo.Orders", new[] { "Tariff_TariffId" });
            DropIndex("dbo.Orders", new[] { "Client_ClientId" });
            DropIndex("dbo.Orders", new[] { "Car_CarId" });
            DropIndex("dbo.Partners", new[] { "Order_OrderId" });
            DropColumn("dbo.Orders", "Worker_WorkerId");
            DropColumn("dbo.Orders", "Tariff_TariffId");
            DropColumn("dbo.Orders", "Client_ClientId");
            DropColumn("dbo.Orders", "Car_CarId");
            DropColumn("dbo.Partners", "Order_OrderId");
        }
    }
}
