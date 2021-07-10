namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteFieldTypeOfDocumentForCreditFromOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "TypeOfDocumentForCredit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "TypeOfDocumentForCredit", c => c.String());
        }
    }
}
