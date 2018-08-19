using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ZondervanLibrary.PatronTranslator.Console.Patrons
{
    public class PatronToPersonaConverter : IConverter<Patron, Persona>
    {
        private readonly String[] _stateCodes;
        private DateTime _exportedDateTime;

        public PatronToPersonaConverter(DateTime exportedDateTime)
        {
            _stateCodes = new[] { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "DC", "FL", "GA", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };
            _exportedDateTime = exportedDateTime;
        }

        public Persona Convert(Patron patron)
        {
            if (patron == null)
                throw new ArgumentNullException(nameof(patron));

            Persona persona = new Persona
            {
                institutionId = "1001",
                correlationInfo = ParseCorrelationInfo(patron),
                wmsCircPatronInfo = ParsePatronInfo(patron),
                nameInfo = ParseNameInfo(patron),
                contactInfo = ParseContactInfo(patron)
            };

            DateTime? expirationDate = ParseCirculationDate(patron.circExpirationDate);

            if (expirationDate.HasValue)
            {
                persona.oclcExpirationDate = expirationDate.Value;
                persona.oclcExpirationDateSpecified = true;
            }
            else
            {
                persona.oclcExpirationDateSpecified = false;
            }

            return persona;
        }

        public Patron ConvertBack(Persona persona)
        {
            throw new NotImplementedException();
        }

        private CorrelationInfo[] ParseCorrelationInfo(Patron patron)
        {
            if (!String.IsNullOrWhiteSpace(patron.id))
            {
                CorrelationInfo info = new CorrelationInfo
                {
                    sourceSystem = "urn:mace:oclc:idm:tayloruniversity:cas",
                    idAtSource = patron.id.Trim()
                };

                return new[] { info };
            }
            else
            {
                return null;
            }
        }

        private WmsCircPatronInfo ParsePatronInfo(Patron patron)
        {
            WmsCircPatronInfo patronInfo = new WmsCircPatronInfo
            {// Hard coded values
                homeBranch = "141904",
                isFineExemptSpecified = true,
                isFineExempt = false,
                isVerifiedSpecified = true,
                isVerified = true,
                storeCheckoutHistorySpecified = true,
                storeCheckoutHistory = true,
                barcode = patron.Barcode,
                borrowerCategory = ParseBorrowerCategory(patron)
            };

            DateTime? registrationDate = ParseCirculationDate(patron.circRegistrationDate);
            if (registrationDate.HasValue)
            {
                patronInfo.circRegistrationDate = registrationDate.Value;
                patronInfo.circRegistrationDateSpecified = true;
            }
            else
            {
                patronInfo.circRegistrationDateSpecified = false;
            }

            return patronInfo;
        }

        private String ParseBorrowerCategory(Patron patron)
        {
            String category = "ITU Community";
            switch (patron.borrowerCategory)
            {
                case "01":
                    category = "ITU Undergraduate Student";
                    break;
                case "12":
                    category = "ITU Graduate Student";
                    break;
                case "02":
                    category = "ITU Faculty";
                    break;
                case "13":
                    category = "ITU Adjunct Faculty";
                    break;
                case "03":
                case "10":
                    category = "ITU Staff";
                    break;
            }

            return category;
        }

        private NameInfo ParseNameInfo(Patron patron)
        {
            if (String.IsNullOrWhiteSpace(patron.Name))
            {
                return null;
            }

            NameInfo nameInfo = new NameInfo();

            String[] namePieces = patron.Name.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            String firstAndMiddle;

            nameInfo.familyName = namePieces[0].Trim();

            if (namePieces.Length == 3)
            {
                nameInfo.suffix = namePieces[1].Trim();
                firstAndMiddle = namePieces[2];
            }
            else
            {
                firstAndMiddle = namePieces[1];
            }

            String[] firstAndMiddlePieces = firstAndMiddle.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (firstAndMiddlePieces.Length == 1)
            {
                nameInfo.givenName = firstAndMiddlePieces[0].Trim();
            }
            else
            {
                nameInfo.givenName = firstAndMiddlePieces[0];
                nameInfo.middleName = String.Join(" ", firstAndMiddlePieces.Skip(1)).Trim();
            }

            return nameInfo;
        }

        private ContactInfo[] ParseContactInfo(Patron patron)
        {
            //Email Address
            ContactInfo homeContactInfo = new ContactInfo() { label = "Home" };
            ContactInfo taylorContactInfo = new ContactInfo() { label = "Taylor" };

            EmailAddress taylorEmailAddress = ParseEmailAddress(patron.EmailAddressField90, true);

            // Ignore secondary email address if duplicate.
            if (patron.EmailAddressField70 != patron.EmailAddressField90)
            {
                EmailAddress homeEmailAddress = ParseEmailAddress(patron.EmailAddressField70, false);

                if (homeEmailAddress != null)
                {
                    if (taylorEmailAddress == null)
                    {
                        // If Taylor email not present, make the home email primary
                        homeEmailAddress.isPrimary = true;
                        homeEmailAddress.isPrimarySpecified = true;
                    }
                    else
                    {
                        taylorEmailAddress.isPrimary = true;
                        taylorEmailAddress.isPrimarySpecified = true;
                    }

                    homeContactInfo.Items = new Object[] { homeEmailAddress };
                }
            }

            if (taylorEmailAddress != null)
            {
                taylorContactInfo.Items = new Object[] { taylorEmailAddress };
            }

            //Postal Phone
            object[] taylorInfo = ParsePostalOrPhoneAddress(patron.AddressLine1Field80, patron.AddressLine2Field81, patron.AddressLine3Field82, patron.AddressLine4Field83, patron.ZipCodeField84, patron.PhoneField86, patron.borrowerCategory, true);

            if (taylorInfo != null)
            {
                taylorContactInfo.Items = taylorContactInfo.Items?.Concat(taylorInfo).ToArray() ?? taylorInfo;
            }

            object[] homeInfo = ParsePostalOrPhoneAddress(patron.AddressLine1Field60, patron.AddressLine2Field61, patron.AddressLine3Field62, patron.AddressLine4Field63, patron.ZipCodeField64, patron.PhoneField66, patron.borrowerCategory, false);

            if (homeInfo != null)
            {
                // Check to see whether the postal addresses are duplicates... if they are we only need to send one.
                if (taylorInfo == null)
                {
                    homeContactInfo.Items = homeContactInfo.Items.Concat(homeInfo).ToArray();
                }
                else
                {
                    List<Object> taylorPostalAddresses = taylorInfo.Where(x => x.GetType() == typeof(PostalAddress)).ToList();
                    List<Object> homeInfoPostalAddresses = homeInfo.Where(x => x.GetType() == typeof(PostalAddress)).ToList();
                    if (taylorPostalAddresses.Count == 1 && homeInfoPostalAddresses.Count == 1)
                    {
                        PostalAddress taylorPostalAddress = (PostalAddress)taylorPostalAddresses.First();
                        PostalAddress homeInfoPostalAddress = (PostalAddress)homeInfoPostalAddresses.First();
                        if (taylorPostalAddress.streetAddressLine1 != homeInfoPostalAddress.streetAddressLine1 || 
                            taylorPostalAddress.postalCode != homeInfoPostalAddress.postalCode ||
                            taylorPostalAddress.stateOrProvince != homeInfoPostalAddress.stateOrProvince ||
                            taylorPostalAddress.streetAddressLine2 != homeInfoPostalAddress.streetAddressLine2 ||
                            taylorPostalAddress.cityOrLocality != homeInfoPostalAddress.cityOrLocality ||
                            taylorPostalAddress.country != homeInfoPostalAddress.country)
                        {
                            homeContactInfo.Items = homeContactInfo.Items?.Concat(homeInfo).ToArray() ?? homeInfo;
                        }
                    }
                    else if (taylorPostalAddresses.Count > 1 || homeInfoPostalAddresses.Count > 1)
                    {
                        System.Console.WriteLine("More than one address");
                    }
                    else
                    {
                        homeContactInfo.Items = homeContactInfo.Items?.Concat(homeInfo).ToArray() ?? homeInfo;
                    }
                }
            }
            if (homeContactInfo.Items == null || !homeContactInfo.Items.Any())
            {
                return new[] { taylorContactInfo };
            }

            return new[] { taylorContactInfo, homeContactInfo };
        }

        private EmailAddress ParseEmailAddress(String emailAddress, Boolean isTaylorAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress))
            {
                return null;
            }

            return new EmailAddress() { emailAddress = emailAddress.Trim(), isPrimary = isTaylorAddress, isPrimarySpecified = true };
        }

        private object[] ParsePostalOrPhoneAddress(String line1, String line2, String line3, String line4, String zipCode, String phoneNumber, String categoryNumber, Boolean isTaylorAddress)
        {
            Phone phone = null;

            if (!String.IsNullOrWhiteSpace(phoneNumber))
            {
                phone = new Phone()
                {
                    isPrimary = isTaylorAddress,
                    isPrimarySpecified = true,
                    number = phoneNumber.Trim()
                };
            }

            if (!String.IsNullOrWhiteSpace(line1) || !String.IsNullOrWhiteSpace(line2) || !String.IsNullOrWhiteSpace(line3) || !String.IsNullOrWhiteSpace(line4) || !String.IsNullOrWhiteSpace(zipCode))
            {
                // Part of the physical Address is specified.
                String[] cityAndStatePieces;
                PostalAddress postalAddress = new PostalAddress();

                if (!String.IsNullOrWhiteSpace(line4))
                {
                    postalAddress.streetAddressLine1 = $"{line1.Trim()}; {line2.Trim()}";
                    postalAddress.streetAddressLine2 = line3.Trim();
                    cityAndStatePieces = line4.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (!String.IsNullOrWhiteSpace(line3))
                {
                    postalAddress.streetAddressLine1 = line1.Trim();
                    postalAddress.streetAddressLine2 = line2.Trim();
                    cityAndStatePieces = line3.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    postalAddress.streetAddressLine1 = line1.Trim();
                    postalAddress.streetAddressLine2 = String.Empty;
                    cityAndStatePieces = line2.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                // If streetAddressLine2 is empty would should set to null
                postalAddress.streetAddressLine2 = (String.IsNullOrWhiteSpace(postalAddress.streetAddressLine2)) ? null : postalAddress.streetAddressLine2;

                postalAddress.cityOrLocality = (String.IsNullOrWhiteSpace(cityAndStatePieces[0])) ? "N/A" : cityAndStatePieces[0].Trim();

                if (cityAndStatePieces.Length > 1)
                {
                    postalAddress.stateOrProvince = cityAndStatePieces[1].Trim();

                    if (_stateCodes.Contains(postalAddress.stateOrProvince))
                    {
                        // If address contains state code then set country to United States
                        postalAddress.country = "United States";
                    }
                }
                else
                {
                    postalAddress.stateOrProvince = "N/A";
                }

                postalAddress.postalCode = (String.IsNullOrWhiteSpace(zipCode)) ? "N/A" : zipCode.Trim();
          
                postalAddress.isPermanent = !isTaylorAddress;
                postalAddress.isPermanentSpecified = true;
                postalAddress.isPrimary = isTaylorAddress;
                postalAddress.isPrimarySpecified = true;

                if (isTaylorAddress && (categoryNumber == "01" || categoryNumber == "12"))
                {
                    postalAddress.validFromSpecified = true;
                    postalAddress.validToSpecified = true;

                    if (_exportedDateTime.Month >= 2 && _exportedDateTime.Month <= 5)
                    {
                        postalAddress.validFrom = new DateTime(_exportedDateTime.Year, 2, 1);
                        postalAddress.validTo = new DateTime(_exportedDateTime.Year, 5, 31);
                    }
                    else if (_exportedDateTime.Month >= 6 && _exportedDateTime.Month <= 7)
                    {
                        postalAddress.validFrom = new DateTime(_exportedDateTime.Year, 6, 1);
                        postalAddress.validTo = new DateTime(_exportedDateTime.Year, 7, 31);
                    }
                    else if (_exportedDateTime.Month >= 8 && _exportedDateTime.Month <= 11 || (_exportedDateTime.Month == 12 && _exportedDateTime.Day <= 24))
                    {
                        postalAddress.validFrom = new DateTime(_exportedDateTime.Year, 8, 1);
                        postalAddress.validTo = new DateTime(_exportedDateTime.Year, 12, 24);
                    }
                    else if (_exportedDateTime.Month == 12)
                    {
                        postalAddress.validFrom = new DateTime(_exportedDateTime.Year, 12, 25);
                        postalAddress.validTo = new DateTime(_exportedDateTime.Year + 1, 1, 31);
                    }
                    else if (_exportedDateTime.Month == 1)
                    {
                        postalAddress.validFrom = new DateTime(_exportedDateTime.Year - 1, 12, 25);
                        postalAddress.validTo = new DateTime(_exportedDateTime.Year, 1, 31);
                    }
                }
                else
                {
                    postalAddress.validFromSpecified = false;
                    postalAddress.validToSpecified = false;
                }

                if (phone != null)
                {
                    return new object[] { phone, postalAddress };
                }
                else
                {
                    return  new object[] { postalAddress };
                }
            }
            else
            {
                // If no physical address specified then return phone number
                if (phone != null)
                {
                    return new object[] { phone };
                }
                else
                {
                    return null;
                }
            }
        }

        private DateTime? ParseCirculationDate(String circulationDate)
        {
            if (String.IsNullOrWhiteSpace(circulationDate))
            {
                return null;
            }

            try
            {
                return DateTime.ParseExact(circulationDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                // If the date was in an invalid format return null
                return null;
            }
        }
    }
}
