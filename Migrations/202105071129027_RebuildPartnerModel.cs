namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RebuildPartnerModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "Town", c => c.String());
            AddColumn("dbo.Partners", "Product", c => c.String());
            AddColumn("dbo.ServiceAndInsurances", "Partner_PartnerId", c => c.Int());
            CreateIndex("dbo.ServiceAndInsurances", "Partner_PartnerId");
            AddForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners", "PartnerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners");
            DropIndex("dbo.ServiceAndInsurances", new[] { "Partner_PartnerId" });
            DropColumn("dbo.ServiceAndInsurances", "Partner_PartnerId");
            DropColumn("dbo.Partners", "Product");
            DropColumn("dbo.Partners", "Town");
        }
    }
}
