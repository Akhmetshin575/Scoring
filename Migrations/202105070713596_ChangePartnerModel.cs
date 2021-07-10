namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePartnerModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners");
            DropIndex("dbo.ServiceAndInsurances", new[] { "Partner_PartnerId" });
            DropColumn("dbo.ServiceAndInsurances", "Partner_PartnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceAndInsurances", "Partner_PartnerId", c => c.Int());
            CreateIndex("dbo.ServiceAndInsurances", "Partner_PartnerId");
            AddForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners", "PartnerId");
        }
    }
}
