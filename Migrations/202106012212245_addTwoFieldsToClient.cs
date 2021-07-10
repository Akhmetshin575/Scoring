namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTwoFieldsToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "DateOfWedding", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "ClientIsMale", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "ClientIsMale");
            DropColumn("dbo.Clients", "DateOfWedding");
        }
    }
}
