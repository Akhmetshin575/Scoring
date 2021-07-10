namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteWorkerIdFieldsFromServiceAndInsurances : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceAndInsurances", "WorkerId", "dbo.Workers");
            DropIndex("dbo.ServiceAndInsurances", new[] { "WorkerId" });
            DropColumn("dbo.ServiceAndInsurances", "WorkerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceAndInsurances", "WorkerId", c => c.Int());
            CreateIndex("dbo.ServiceAndInsurances", "WorkerId");
            AddForeignKey("dbo.ServiceAndInsurances", "WorkerId", "dbo.Workers", "WorkerId");
        }
    }
}
