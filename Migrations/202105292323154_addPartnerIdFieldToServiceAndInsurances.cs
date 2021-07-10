namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPartnerIdFieldToServiceAndInsurances : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAndInsurances", "PartnerId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAndInsurances", "PartnerId");
        }
    }
}
