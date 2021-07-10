using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring3
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOFBirth { get; set; }
        public string Education { get; set; }
        public string SocialStatus { get; set; }
        public DateTime DateOfWedding { get; set; }
        public bool ClientIsMale { get; set; }


        public int PassportSeries { get; set; }
        public int PassportNumber { get; set; }
        public DateTime PassportDateOfIssue { get; set; }
        public string PassportDivision { get; set; }
        public int PassportCodeOfDivision { get; set; }

        public int DriverLicenseSeries { get; set; }
        public int DriverLicenseNumber { get; set; }
        public DateTime DriverLicenseDateofIssue { get; set; }

        public int ZipRegistration { get; set; }
        public string Subject_Registration { get; set; }
        public string District_Registration { get; set; }
        public string TypeOfPlaceOfResidence_Registration { get; set; }
        public string NameOfPlaceOfResidence_Registration { get; set; }
        public string Street_Registration { get; set; }
        public string NumberOfHouse_Registration { get; set; }
        public string NumberOfApartment_Registration { get; set; }
        public DateTime DateOfRegistartion { get; set; }

        public int Zip_Fact { get; set; }
        public string Subject_Fact { get; set; }
        public string District_Fact { get; set; }
        public string TypeOfPlaceOfResidence_Fact { get; set; }
        public string NameOfPlaceOfResidence_Fact { get; set; }
        public string Street_Fact { get; set; }
        public string NumberOfHouse_Fact { get; set; }
        public string NumberOfApartment_Fact { get; set; }

        public long MobilePhoneNumber { get; set; }
        public DateTime DateOfCall_mobilePhoneNumber { get; set; }
        public string Responder_mobilePhoneNumber { get; set; }
        public string ResultOfCall_mobilePhoneNumber { get; set; }

        public long WorkingPhoneNumber { get; set; }
        public DateTime DateOfCall_workingPhoneNumber { get; set; }
        public string Responder_workingPhoneNumber { get; set; }
        public string ResultOfCall_workingPhoneNumber { get; set; }

        public long SpousePhoneNumber { get; set; }
        public DateTime DateOfCall_spousePhoneNumber { get; set; }
        public string Responder_spousePhoneNumber { get; set; }
        public string ResultOfCall_spousePhoneNumber { get; set; }

        public long AdditionalPhoneNumber { get; set; }
        public DateTime DateOfCall_additionalPhoneNumber { get; set; }
        public string Responder_additionalPhoneNumber { get; set; }
        public string ResultOfCall_additionalPhoneNumber { get; set; }

        public string NameOfOrganization { get; set; }
        public long InnOfOrganization { get; set; }
        public string Post { get; set; }
        public string TypeOfPost { get; set; }
        public string Subject_Organization { get; set; }
        public string District_Organization { get; set; }
        public string TypeOfPlaceOfResidence_Organization { get; set; }
        public string NameOfPlaceOfResidence_Organization { get; set; }
        public string Street_Organization { get; set; }
        public string NumberOfHouse_Organization { get; set; }
        public string NumberOfApartment_Organization { get; set; }
        public int Experience_current { get; set; }
        public int Experience_general { get; set; }

        public int BasicIncome { get; set; }
        public int AdditionalIncome { get; set; }
        public bool IncomeIsChecked { get; set; }
        public string TypeOfIncomeConfirmation { get; set; }
        public int Expenses { get; set; }

        public bool Married { get; set; }
        public int YearsOfMarriage { get; set; }
        public int NumberOfDependents { get; set; }
    }
}
