namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManyToManyRelationsBetweenPartnersAndServicesAndIncurances : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners");
            DropIndex("dbo.ServiceAndInsurances", new[] { "Partner_PartnerId" });
            CreateTable(
                "dbo.ServiceAndInsurancePartners",
                c => new
                    {
                        ServiceAndInsurance_ServiceAndInsuranceId = c.Int(nullable: false),
                        Partner_PartnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceAndInsurance_ServiceAndInsuranceId, t.Partner_PartnerId })
                .ForeignKey("dbo.ServiceAndInsurances", t => t.ServiceAndInsurance_ServiceAndInsuranceId, cascadeDelete: true)
                .ForeignKey("dbo.Partners", t => t.Partner_PartnerId, cascadeDelete: true)
                .Index(t => t.ServiceAndInsurance_ServiceAndInsuranceId)
                .Index(t => t.Partner_PartnerId);
            
            DropColumn("dbo.ServiceAndInsurances", "Partner_PartnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceAndInsurances", "Partner_PartnerId", c => c.Int());
            DropForeignKey("dbo.ServiceAndInsurancePartners", "Partner_PartnerId", "dbo.Partners");
            DropForeignKey("dbo.ServiceAndInsurancePartners", "ServiceAndInsurance_ServiceAndInsuranceId", "dbo.ServiceAndInsurances");
            DropIndex("dbo.ServiceAndInsurancePartners", new[] { "Partner_PartnerId" });
            DropIndex("dbo.ServiceAndInsurancePartners", new[] { "ServiceAndInsurance_ServiceAndInsuranceId" });
            DropTable("dbo.ServiceAndInsurancePartners");
            CreateIndex("dbo.ServiceAndInsurances", "Partner_PartnerId");
            AddForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners", "PartnerId");
        }
    }
}
