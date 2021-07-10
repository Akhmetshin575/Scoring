namespace Scoring3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        CarId = c.Int(nullable: false, identity: true),
                        Brand = c.String(),
                        Model = c.String(),
                        NameFromDocument = c.String(),
                        TypeOfDocument = c.String(),
                        DocumentSeries = c.String(),
                        DocumentNumber = c.String(),
                        DocumentDateOfIssue = c.DateTime(nullable: false),
                        NewCar = c.Boolean(nullable: false),
                        YearOfRelease = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        InitialPayment = c.Int(nullable: false),
                        Vin = c.String(),
                        Uveos = c.String(),
                        MotorNumber = c.String(),
                        BodyNumber = c.String(),
                        Powerful = c.Double(nullable: false),
                        TypeOfMotor = c.String(),
                        EcologicalClass = c.Int(nullable: false),
                        PermittedWeight = c.Int(nullable: false),
                        Color = c.String(),
                    })
                .PrimaryKey(t => t.CarId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        PartnerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TypeOfCounterParty = c.String(),
                        Bic = c.String(),
                        Inn = c.String(),
                        Kpp = c.String(),
                        Rs = c.String(),
                        Ks = c.String(),
                        Bank = c.String(),
                    })
                .PrimaryKey(t => t.PartnerId);
            
            CreateTable(
                "dbo.ServiceAndInsurances",
                c => new
                    {
                        ServiceAndInsuranceId = c.Int(nullable: false, identity: true),
                        TypeOfAdditionalProduct = c.String(),
                        NameOfProduct = c.String(),
                        InvoiceNumber = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        OnCredit = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        DurationMonths = c.Int(nullable: false),
                        Series = c.String(),
                        Number = c.String(),
                        InsuredSum = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        WorkerId = c.Int(),
                        OrderId = c.Int(),
                        Partner_PartnerId = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceAndInsuranceId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.Workers", t => t.WorkerId)
                .ForeignKey("dbo.Partners", t => t.Partner_PartnerId)
                .Index(t => t.WorkerId)
                .Index(t => t.OrderId)
                .Index(t => t.Partner_PartnerId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        DateOfOrder = c.DateTime(nullable: false),
                        DateOfCreditIssued = c.DateTime(nullable: false),
                        StatusOfOrder = c.String(),
                        Payment = c.Int(nullable: false),
                        Comment = c.String(),
                        RefusalRequired = c.Boolean(nullable: false),
                        TypeOfDocumentForCredit = c.String(),
                        NumberOfDocumentForCredit = c.String(),
                        DateOfDocumentForCredit = c.DateTime(nullable: false),
                        CarId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CounterPartyId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        TariffId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.Workers",
                c => new
                    {
                        WorkerId = c.Int(nullable: false, identity: true),
                        Surname = c.String(),
                        Name = c.String(),
                        Patronymic = c.String(),
                        Post = c.String(),
                        Phone = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        FirstEntry = c.Boolean(nullable: false),
                        Town = c.String(),
                    })
                .PrimaryKey(t => t.WorkerId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        Surname = c.String(),
                        Name = c.String(),
                        Patronymic = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        PlaceOFBirth = c.String(),
                        Education = c.String(),
                        SocialStatus = c.String(),
                        PassportSeries = c.Int(nullable: false),
                        PassportNumber = c.Int(nullable: false),
                        PassportDateOfIssue = c.DateTime(nullable: false),
                        PassportDivision = c.String(),
                        PassportCodeOfDivision = c.Int(nullable: false),
                        DriverLicenseSeries = c.Int(nullable: false),
                        DriverLicenseNumber = c.Int(nullable: false),
                        DriverLicenseDateofIssue = c.DateTime(nullable: false),
                        ZipRegistration = c.Int(nullable: false),
                        Subject_Registration = c.String(),
                        District_Registration = c.String(),
                        TypeOfPlaceOfResidence_Registration = c.String(),
                        NameOfPlaceOfResidence_Registration = c.String(),
                        Street_Registration = c.String(),
                        NumberOfHouse_Registration = c.String(),
                        NumberOfApartment_Registration = c.String(),
                        DateOfRegistartion = c.DateTime(nullable: false),
                        Zip_Fact = c.Int(nullable: false),
                        Subject_Fact = c.String(),
                        District_Fact = c.String(),
                        TypeOfPlaceOfResidence_Fact = c.String(),
                        NameOfPlaceOfResidence_Fact = c.String(),
                        Street_Fact = c.String(),
                        NumberOfHouse_Fact = c.String(),
                        NumberOfApartment_Fact = c.String(),
                        MobilePhoneNumber = c.Long(nullable: false),
                        DateOfCall_mobilePhoneNumber = c.DateTime(nullable: false),
                        Responder_mobilePhoneNumber = c.String(),
                        ResultOfCall_mobilePhoneNumber = c.String(),
                        WorkingPhoneNumber = c.Long(nullable: false),
                        DateOfCall_workingPhoneNumber = c.DateTime(nullable: false),
                        Responder_workingPhoneNumber = c.String(),
                        ResultOfCall_workingPhoneNumber = c.String(),
                        SpousePhoneNumber = c.Long(nullable: false),
                        DateOfCall_spousePhoneNumber = c.DateTime(nullable: false),
                        Responder_spousePhoneNumber = c.String(),
                        ResultOfCall_spousePhoneNumber = c.String(),
                        AdditionalPhoneNumber = c.Long(nullable: false),
                        DateOfCall_additionalPhoneNumber = c.DateTime(nullable: false),
                        Responder_additionalPhoneNumber = c.String(),
                        ResultOfCall_additionalPhoneNumber = c.String(),
                        NameOfOrganization = c.String(),
                        InnOfOrganization = c.Long(nullable: false),
                        Post = c.String(),
                        TypeOfPost = c.String(),
                        Subject_Organization = c.String(),
                        District_Organization = c.String(),
                        TypeOfPlaceOfResidence_Organization = c.String(),
                        NameOfPlaceOfResidence_Organization = c.String(),
                        Street_Organization = c.String(),
                        NumberOfHouse_Organization = c.String(),
                        NumberOfApartment_Organization = c.String(),
                        Experience_current = c.Int(nullable: false),
                        Experience_general = c.Int(nullable: false),
                        BasicIncome = c.Int(nullable: false),
                        AdditionalIncome = c.Int(nullable: false),
                        IncomeIsChecked = c.Boolean(nullable: false),
                        TypeOfIncomeConfirmation = c.String(),
                        Expenses = c.Int(nullable: false),
                        Married = c.Boolean(nullable: false),
                        YearsOfMarriage = c.Int(nullable: false),
                        NumberOfDependents = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Tariffs",
                c => new
                    {
                        TariffId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Months = c.Int(nullable: false),
                        PercentageRate = c.Double(nullable: false),
                        CarInsuranceRequired = c.Boolean(nullable: false),
                        TariffForNewCar = c.Boolean(nullable: false),
                        PercentOfMinimalInitaialPayment = c.Int(nullable: false),
                        PercentOfMaximumInitaialPayment = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TariffId);
            
            CreateTable(
                "dbo.PartnerCities",
                c => new
                    {
                        Partner_PartnerId = c.Int(nullable: false),
                        City_CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Partner_PartnerId, t.City_CityId })
                .ForeignKey("dbo.Partners", t => t.Partner_PartnerId, cascadeDelete: true)
                .ForeignKey("dbo.Cities", t => t.City_CityId, cascadeDelete: true)
                .Index(t => t.Partner_PartnerId)
                .Index(t => t.City_CityId);
            
            CreateTable(
                "dbo.WorkerCities",
                c => new
                    {
                        Worker_WorkerId = c.Int(nullable: false),
                        City_CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Worker_WorkerId, t.City_CityId })
                .ForeignKey("dbo.Workers", t => t.Worker_WorkerId, cascadeDelete: true)
                .ForeignKey("dbo.Cities", t => t.City_CityId, cascadeDelete: true)
                .Index(t => t.Worker_WorkerId)
                .Index(t => t.City_CityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceAndInsurances", "Partner_PartnerId", "dbo.Partners");
            DropForeignKey("dbo.ServiceAndInsurances", "WorkerId", "dbo.Workers");
            DropForeignKey("dbo.WorkerCities", "City_CityId", "dbo.Cities");
            DropForeignKey("dbo.WorkerCities", "Worker_WorkerId", "dbo.Workers");
            DropForeignKey("dbo.ServiceAndInsurances", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.PartnerCities", "City_CityId", "dbo.Cities");
            DropForeignKey("dbo.PartnerCities", "Partner_PartnerId", "dbo.Partners");
            DropIndex("dbo.WorkerCities", new[] { "City_CityId" });
            DropIndex("dbo.WorkerCities", new[] { "Worker_WorkerId" });
            DropIndex("dbo.PartnerCities", new[] { "City_CityId" });
            DropIndex("dbo.PartnerCities", new[] { "Partner_PartnerId" });
            DropIndex("dbo.ServiceAndInsurances", new[] { "Partner_PartnerId" });
            DropIndex("dbo.ServiceAndInsurances", new[] { "OrderId" });
            DropIndex("dbo.ServiceAndInsurances", new[] { "WorkerId" });
            DropTable("dbo.WorkerCities");
            DropTable("dbo.PartnerCities");
            DropTable("dbo.Tariffs");
            DropTable("dbo.Clients");
            DropTable("dbo.Workers");
            DropTable("dbo.Orders");
            DropTable("dbo.ServiceAndInsurances");
            DropTable("dbo.Partners");
            DropTable("dbo.Cities");
            DropTable("dbo.Cars");
        }
    }
}
