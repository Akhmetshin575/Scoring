namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTariffModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tariffs", "FullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tariffs", "FullName");
        }
    }
}
