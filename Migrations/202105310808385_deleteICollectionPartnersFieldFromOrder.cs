namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteICollectionPartnersFieldFromOrder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Partners", "Order_OrderId", "dbo.Orders");
            DropIndex("dbo.Partners", new[] { "Order_OrderId" });
            DropColumn("dbo.Partners", "Order_OrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Partners", "Order_OrderId", c => c.Int());
            CreateIndex("dbo.Partners", "Order_OrderId");
            AddForeignKey("dbo.Partners", "Order_OrderId", "dbo.Orders", "OrderId");
        }
    }
}
