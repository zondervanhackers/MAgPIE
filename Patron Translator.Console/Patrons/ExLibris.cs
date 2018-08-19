using FileHelpers;

namespace ZondervanLibrary.PatronTranslator.Console.Patrons
{
    [FixedLengthRecord(FixedMode.AllowVariableLength)]
    public class Patron
    {
        public Patron()
        {
        }

        [FieldFixedLength(3)]                   //3
        public string Field1Thru4;

        [FieldFixedLength(14)]                  //17
        public string Barcode;                  //field 5

        [FieldFixedLength(116)]                 //133
        public string Field6Thru7;

        [FieldFixedLength(200)]                 //333
        public string Name;                     //field 8

        [FieldFixedLength(8)]                   //341
        public string DateOfBirth;              //field 9

        [FieldFixedLength(864)]                 //1205
        public string Field10Thru58;

        [FieldFixedLength(50)]                  //1255
        public string FullNameOption1;          //field 59

        [FieldFixedLength(50)]                  //1305
        public string AddressLine1Field60;      //field 60

        [FieldFixedLength(50)]                  //1355
        public string AddressLine2Field61;      //field 61

        [FieldFixedLength(50)]                  //1405
        public string AddressLine3Field62;      //field 62

        [FieldFixedLength(50)]                  //1455
        public string AddressLine4Field63;      //field 63

        [FieldFixedLength(9)]                   //1464
        public string ZipCodeField64;           //field 64

        [FieldFixedLength(1)]                   //1465
        public string Field65;                  //field 65

        [FieldFixedLength(30)]                  //1495
        public string PhoneField66;             //Field 66

        [FieldFixedLength(90)]                  //1585
        public string Field67Thr69;

        [FieldFixedLength(60)]                  //1645
        public string EmailAddressField70;      //Field 70 

        [FieldFixedLength(60)]                  //1705
        public string Field71Thru79;

        [FieldFixedLength(50)]                  //1755
        public string FullNameOption2;          //field 79

        [FieldFixedLength(50)]                  //1805
        public string AddressLine1Field80;      //field 80

        [FieldFixedLength(50)]                  //1855
        public string AddressLine2Field81;      //field 81

        [FieldFixedLength(50)]                  //1905
        public string AddressLine3Field82;      //field 82

        [FieldFixedLength(50)]                  //1955
        public string AddressLine4Field83;      //field 83  

        [FieldFixedLength(9)]                   //1964
        public string ZipCodeField84;           //field 84

        [FieldFixedLength(1)]                   //1965
        public string Field85;                  //field 85

        [FieldFixedLength(30)]                  //1995
        public string PhoneField86;             //field 86

        [FieldFixedLength(90)]                  //2085
        public string Field87Thru89;

        [FieldFixedLength(60)]                  //2145
        public string EmailAddressField90;      //Field 90 

        [FieldFixedLength(8)]                   //2153
        public string circRegistrationDate;     //Field 91 

        [FieldFixedLength(8)]                   //2161
        public string circExpirationDate;       //Field 92 

        [FieldFixedLength(47)]                  //2208
        public string Field93Thru98;

        [FieldFixedLength(2)]                   //2210
        public string borrowerCategory;         //field 99

        [FieldOptional]
        [FieldFixedLength(390)]                   //2600 (390)
        public string Fields100Thru109;          //

        [FieldOptional]
        [FieldFixedLength(60)]                  //
        public string id;                       //
    }
}
