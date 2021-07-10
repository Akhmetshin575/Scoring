namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFieldAcceptableAmountToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AcceptableAmount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "AcceptableAmount");
        }
    }
}
