namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTypePartnerNameDealerFieldToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Dealer_PartnerId", c => c.Int());
            CreateIndex("dbo.Orders", "Dealer_PartnerId");
            AddForeignKey("dbo.Orders", "Dealer_PartnerId", "dbo.Partners", "PartnerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "Dealer_PartnerId", "dbo.Partners");
            DropIndex("dbo.Orders", new[] { "Dealer_PartnerId" });
            DropColumn("dbo.Orders", "Dealer_PartnerId");
        }
    }
}
