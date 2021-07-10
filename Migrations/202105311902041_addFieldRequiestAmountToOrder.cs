namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFieldRequiestAmountToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RequiestAmount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "RequiestAmount");
        }
    }
}
