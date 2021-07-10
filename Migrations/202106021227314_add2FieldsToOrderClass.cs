namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add2FieldsToOrderClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TypeOfDocumentForCredit", c => c.String());
            AddColumn("dbo.Orders", "CityOfSale", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CityOfSale");
            DropColumn("dbo.Orders", "TypeOfDocumentForCredit");
        }
    }
}
