using System;
using System.ComponentModel.DataAnnotations;
using ZondervanLibrary.SharedLibrary.Equatable;

namespace ZondervanLibrary.PatronTranslator.Console.Patrons
{
    [Equatable(IsImmutable = true)]
    public partial class OclcPersonas : EquatableBase<OclcPersonas>
    { }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(PersonaMetadata))]
    public partial class Persona : EquatableBase<Persona>
    {
        internal sealed class PersonaMetadata
        {
            [EquatableProperty(Dependency = "dateOfBirthSpecified")]
            public DateTime dateOfBirth { get; set; }

            [EquatableProperty(Dependency = "oclcExpirationDateSpecified")]
            public DateTime oclcExpirationDate { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(CorrelationInfoMetadata))]
    public partial class CorrelationInfo : EquatableBase<CorrelationInfo>
    {
        internal sealed class CorrelationInfoMetadata
        {
            [EquatableProperty(Dependency = "sourceSystemField")]
            public DateTime sourceSystem { get; set; }

            [EquatableProperty(Dependency = "idAtSourceField")]
            public Boolean idAtSource { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(WsILLInfoMetadata))]
    public partial class WsILLInfo : EquatableBase<WsILLInfo>
    {
        internal sealed class WsILLInfoMetadata
        {
            [EquatableProperty(Dependency = "illIdField")]
            public string illId { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(NotificationDeliveryDestinationMetadata))]
    public partial class NotificationDeliveryDestination : EquatableBase<NotificationDeliveryDestination>
    {
        internal sealed class NotificationDeliveryDestinationMetadata
        {
            [EquatableProperty(Dependency = "deliveryServiceField")]
            public NotificationDeliveryDestinationDeliveryService deliveryService { get; set; }

            [EquatableProperty(Dependency = "destinationField")]
            public string destination { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    public partial class OldBarcode : EquatableBase<OldBarcode>
    { }

    [Equatable(IsImmutable = true)]
    public partial class KeyValuePair : EquatableBase<KeyValuePair>
    { }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(NoteMetadata))]
    public partial class Note : EquatableBase<Note>
    {
        internal sealed class NoteMetadata
        {
            [EquatableProperty(Dependency = "isPublicSpecified")]
            public Boolean isPublic { get; set; }

            [EquatableProperty(Dependency = "modifiedDateSpecified")]
            public DateTime modifiedDate { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    public partial class Relationship : EquatableBase<Relationship>
    { }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(RoleMetadata))]
    public partial class Role : EquatableBase<Role>
    {
        internal sealed class RoleMetadata
        {
            [EquatableProperty(Dependency = "isExplicitSpecified")]
            public Boolean isExplicit { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(EmailAddressMetadata))]
    public partial class EmailAddress : EquatableBase<EmailAddress>
    {
        internal sealed class EmailAddressMetadata
        {
            [EquatableProperty(Dependency = "isValidSpecified")]
            public Boolean isValid { get; set; }

            [EquatableProperty(Dependency = "verifiedSpecified")]
            public DateTime verified { get; set; }

            [EquatableProperty(Dependency = "isPrimarySpecified")]
            public Boolean isPrimary { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(PhoneMetadata))]
    public partial class Phone : EquatableBase<Phone>
    {
        internal sealed class PhoneMetadata
        {
            [EquatableProperty(Dependency = "isValidSpecified")]
            public Boolean isValid { get; set; }

            [EquatableProperty(Dependency = "verifiedSpecified")]
            public DateTime verified { get; set; }

            [EquatableProperty(Dependency = "isPrimarySpecified")]
            public Boolean isPrimary { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(PostalAddressMetadata))]
    public partial class PostalAddress : EquatableBase<PostalAddress>
    {
        internal sealed class PostalAddressMetadata
        {
            [EquatableProperty(Dependency = "isValidSpecified")]
            public Boolean isValid { get; set; }

            [EquatableProperty(Dependency = "verifiedSpecified")]
            public DateTime verified { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    public partial class ContactInfo : EquatableBase<ContactInfo>
    { }

    [Equatable(IsImmutable = true)]
    public partial class NotificationMethod : EquatableBase<NotificationMethod>
    { }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(PreferencesMetadata))]
    public partial class Preferences : EquatableBase<Preferences>
    {
        internal sealed class PreferencesMetadata
        {
            [EquatableProperty(Dependency = "noTelephoneCallsSpecified")]
            public Boolean noTelephoneCalls { get; set; }

            [EquatableProperty(Dependency = "useHighContrastDisplaySpecified")]
            public Boolean useHighContrastDisplay { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    [MetadataType(typeof(WmsCircPatronInfoMetadata))]
    public partial class WmsCircPatronInfo : EquatableBase<WmsCircPatronInfo>
    {
        internal sealed class WmsCircPatronInfoMetadata
        {
            [EquatableProperty(Dependency = "circRegistrationDateSpecified")]
            public DateTime circRegistrationDate { get; set; }

            [EquatableProperty(Dependency = "circExpirationDateSpecified")]
            public DateTime circExpirationDate { get; set; }

            [EquatableProperty(Dependency = "isCircBlockedSpecified")]
            public Boolean isCircBlocked { get; set; }

            [EquatableProperty(Dependency = "isCollectionExemptSpecified")]
            public Boolean isCollectionExempt { get; set; }

            [EquatableProperty(Dependency = "isFineExemptSpecified")]
            public Boolean isFineExempt { get; set; }

            [EquatableProperty(Dependency = "isVerifiedSpecified")]
            public Boolean isVerified { get; set; }

            [EquatableProperty(Dependency = "storeCheckoutHistorySpecified")]
            public Boolean storeCheckoutHistory { get; set; }
        }
    }

    [Equatable(IsImmutable = true)]
    public partial class NameInfo : EquatableBase<NameInfo>
    { }



    /*
    //public partial class Persona
    //{
    //    public override bool Equals(object obj)
    //    {
    //        Persona p = obj as Persona;
    //        if ((object)p == null)
    //        {
    //            return false;
    //        }

    //        if (this.dateOfBirthFieldSpecified != p.dateOfBirthFieldSpecified ||
    //            (this.dateOfBirthFieldSpecified == true && p.dateOfBirthFieldSpecified == true && this.dateOfBirthField != p.dateOfBirthField))
    //        {
    //            return false;
    //        }

    //        if (this.createdOnFieldSpecified != p.createdOnFieldSpecified ||
    //            (this.createdOnFieldSpecified == true && p.createdOnFieldSpecified == true && this.createdOnField != p.createdOnField))
    //        {
    //            return false;
    //        }

    //        if (this.lastUpdatedOnFieldSpecified != p.lastUpdatedOnFieldSpecified ||
    //            (this.lastUpdatedOnFieldSpecified == true && p.lastUpdatedOnFieldSpecified == true && this.lastUpdatedOnField != p.lastUpdatedOnField))
    //        {
    //            return false;
    //        }

    //        if (this.oclcExpirationDateFieldSpecified != p.oclcExpirationDateFieldSpecified ||
    //            (this.oclcExpirationDateFieldSpecified == true && p.oclcExpirationDateFieldSpecified == true && this.oclcExpirationDateField != p.oclcExpirationDateField))
    //        {
    //            return false;
    //        }

    //        if (this.nameInfoField != p.nameInfoField ||
    //            this.genderField != p.genderField ||
    //            this.wmsCircPatronInfoField != p.wmsCircPatronInfoField ||
    //            this.preferencesField != p.preferencesField ||
    //            this.oclcUserNameField != p.oclcUserNameField ||
    //            this.institutionIdField != p.institutionIdField ||
    //            this.optimisticLockIdField != p.optimisticLockIdField ||
    //            this.ppidField != p.ppidField ||
    //            this.relyingPartyField != p.relyingPartyField ||
    //            this.createdByField != p.createdByField ||
    //            this.lastUpdatedByField != p.lastUpdatedByField)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            if (this.correlationInfoField == null || p.correlationInfoField == null)
    //            {
    //                if (this.correlationInfoField != null || p.correlationInfoField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.correlationInfoField.OrderBy(c => c.id).SequenceEqual(p.correlationInfoField.OrderBy(c => c.id)))
    //                    return false;
    //            }

    //            if (this.contactInfoField == null || p.contactInfoField == null)
    //            {
    //                if (this.contactInfoField != null || p.contactInfoField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                // Compare EmailAddress and PhysicalLocation and Phone seperately for ranking purposes.
    //                IEnumerable<ContactInfo> list1 = this.contactInfoField.Where(c => c.Item is EmailAddress).OrderBy(c => ((EmailAddress)c.Item).emailAddress);
    //                IEnumerable<ContactInfo> list2 = p.contactInfoField.Where(c => c.Item is EmailAddress).OrderBy(c => ((EmailAddress)c.Item).emailAddress);

    //                if (!list1.SequenceEqual(list2))
    //                    return false;

    //                list1 = this.contactInfoField.Where(c => c.Item is PhysicalLocation).OrderBy(c => ((PhysicalLocation)c.Item).postalAddress.streetAddressLine1);
    //                list2 = p.contactInfoField.Where(c => c.Item is PhysicalLocation).OrderBy(c => ((PhysicalLocation)c.Item).postalAddress.streetAddressLine1);

    //                if (!list1.SequenceEqual(list2))
    //                    return false;

    //                list1 = this.contactInfoField.Where(c => c.Item is Phone).OrderBy(c => ((Phone)c.Item).number);
    //                list2 = p.contactInfoField.Where(c => c.Item is Phone).OrderBy(c => ((Phone)c.Item).number);

    //                if (!list1.SequenceEqual(list2))
    //                    return false;
    //            }

    //            if (this.roleField == null || p.roleField == null)
    //            {
    //                if (this.roleField != null || p.roleField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.roleField.OrderBy(r => r.id).SequenceEqual(p.roleField.OrderBy(r => r.id)))
    //                    return false;
    //            }

    //            if (this.relationshipField == null || p.relationshipField == null)
    //            {
    //                if (this.relationshipField != null || p.relationshipField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.relationshipField.OrderBy(r => r.id).SequenceEqual(p.relationshipField.OrderBy(r => r.id)))
    //                    return false;
    //            }

    //            if (this.noteField == null || p.noteField == null)
    //            {
    //                if (this.noteField != null || p.noteField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.noteField.OrderBy(n => n.id).SequenceEqual(p.noteField.OrderBy(n => n.id)))
    //                    return false;
    //            }

    //            if (this.additionalInfoField == null || p.additionalInfoField == null)
    //            {
    //                if (this.additionalInfoField != null || p.additionalInfoField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.additionalInfoField.OrderBy(a => a.id).SequenceEqual(p.additionalInfoField.OrderBy(a => a.id)))
    //                    return false;
    //            }

    //            if (this.cancelledBarcodeField == null || p.cancelledBarcodeField == null)
    //            {
    //                if (this.cancelledBarcodeField != null || p.cancelledBarcodeField != null)
    //                    return false;
    //            }
    //            else
    //            {
    //                if (!this.cancelledBarcodeField.OrderBy(c => c.barcode).SequenceEqual(p.cancelledBarcodeField.OrderBy(c => c.barcode)))
    //                    return false;
    //            }
    //        }

    //        return true;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }
    //}

    //public partial class CorrelationInfo
    //{
    //    public override bool Equals(object obj)
    //    {
    //        CorrelationInfo i = obj as CorrelationInfo;
    //        if ((object)i == null)
    //        {
    //            return false;
    //        }

    //        if (this.isBlockedFieldSpecified != i.isBlockedFieldSpecified ||
    //            (this.isBlockedFieldSpecified == true && i.isBlockedFieldSpecified == true && this.isBlockedField != i.isBlockedField))
    //        {
    //            return false;
    //        }

    //        if (this.lastLoginDateFieldSpecified != i.lastLoginDateFieldSpecified ||
    //            (this.lastLoginDateFieldSpecified == true && i.lastLoginDateFieldSpecified == true && this.lastLoginDateField != i.lastLoginDateField))
    //        {
    //            return false;
    //        }

    //        return this.sourceSystemField == i.sourceSystemField &&
    //               this.idAtSourceField == i.idAtSourceField &&
    //               this.idField == i.idField;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }

    //    public static bool operator ==(CorrelationInfo a, CorrelationInfo b)
    //    {
    //        if (System.Object.ReferenceEquals(a, b))
    //        {
    //            return true;
    //        }

    //        if ((object)a == null)
    //        {
    //            return (object)b == null;
    //        }
    //        else
    //        {
    //            return a.Equals(b);
    //        }
    //    }

    //    public static bool operator !=(CorrelationInfo a, CorrelationInfo b)
    //    {
    //        return !(a == b);
    //    }
    //}

    //public partial class OldBarcode
    //{
    //    public override bool Equals(object obj)
    //    {
    //        OldBarcode b = obj as OldBarcode;
    //        if ((object)b == null)
    //        {
    //            return false;
    //        }

    //        return this.barcodeField == b.barcodeField &&
    //               this.cancelledOnField == b.cancelledOnField;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }

    //    public static bool operator ==(OldBarcode a, OldBarcode b)
    //    {
    //        if (System.Object.ReferenceEquals(a, b))
    //        {
    //            return true;
    //        }

    //        if ((object)a == null)
    //        {
    //            return (object)b == null;
    //        }
    //        else
    //        {
    //            return a.Equals(b);
    //        }
    //    }

    //    public static bool operator !=(OldBarcode a, OldBarcode b)
    //    {
    //        return !(a == b);
    //    }
    //}

    public partial class KeyValuePair
    {
        public override bool Equals(object obj)
        {
            KeyValuePair k = obj as KeyValuePair;
            if ((object)k == null)
            {
                return false;
            }

            return this.businessContextField == k.businessContextField &&
                   this.keyField == k.keyField &&
                   this.valueField == k.valueField &&
                   this.idField == k.idField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(KeyValuePair a, KeyValuePair b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(KeyValuePair a, KeyValuePair b)
        {
            return !(a == b);
        }
    }

    public partial class Note
    {
        public override bool Equals(object obj)
        {
            Note n = obj as Note;
            if ((object)n == null)
            {
                return false;
            }

            if (this.isPublicFieldSpecified != n.isPublicFieldSpecified ||
                (this.isPublicFieldSpecified == true && n.isPublicFieldSpecified == true && this.isPublicField != n.isPublicField))
            {
                return false;
            }

            if (this.modifiedDateFieldSpecified != n.modifiedDateFieldSpecified ||
                (this.modifiedDateFieldSpecified == true && n.modifiedDateFieldSpecified == true && this.modifiedDateField != n.modifiedDateField))
            {
                return false;
            }

            return this.businessContextField == n.businessContextField &&
                   this.authorIdField == n.authorIdField &&
                   this.flagField == n.flagField &&
                   this.textField == n.textField &&
                   this.idField == n.idField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Note a, Note b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(Note a, Note b)
        {
            return !(a == b);
        }
    }

    public partial class Relationship
    {
        public override bool Equals(object obj)
        {
            Relationship r = obj as Relationship;
            if ((object)r == null)
            {
                return false;
            }

            return this.relationshipTypeField == r.relationshipTypeField &&
                   this.targetUserIdField == r.targetUserIdField &&
                   this.idField == r.idField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Relationship a, Relationship b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(Relationship a, Relationship b)
        {
            return !(a == b);
        }
    }

    public partial class Role
    {
        public override bool Equals(object obj)
        {
            Role r = obj as Role;
            if ((object)r == null)
            {
                return false;
            }

            if (this.isExplicitFieldSpecified != r.isExplicitFieldSpecified ||
                (this.isExplicitFieldSpecified == true && r.isExplicitFieldSpecified == true && this.isExplicitField != r.isExplicitField))
            {
                return false;
            }

            return this.authorityIdField == r.authorityIdField &&
                   this.authorityIdTypeField == r.authorityIdTypeField &&
                   this.roleNameField == r.roleNameField &&
                   this.idField == r.idField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Role a, Role b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }
    }

    public partial class EmailAddress
    {
        public override bool Equals(object obj)
        {
            EmailAddress a = obj as EmailAddress;
            if ((object)a == null)
            {
                return false;
            }

            if (this.isValidFieldSpecified != a.isValidFieldSpecified ||
                (this.isValidFieldSpecified == true && a.isValidFieldSpecified == true && this.isValidField != a.isValidField))
            {
                return false;
            }

            if (this.verifiedFieldSpecified != a.verifiedFieldSpecified ||
                (this.verifiedFieldSpecified == true && a.verifiedFieldSpecified == true && this.verifiedField != a.verifiedField))
            {
                return false;
            }

            if (this.isPrimaryFieldSpecified != a.isPrimaryFieldSpecified ||
                (this.isPrimaryFieldSpecified == true && a.isPrimaryFieldSpecified == true && this.isPrimaryField != a.isPrimaryField))
            {
                return false;
            }

            return this.emailAddressField == a.emailAddressField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(EmailAddress a, EmailAddress b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(EmailAddress a, EmailAddress b)
        {
            return !(a == b);
        }
    }

    public partial class Phone
    {
        public override bool Equals(object obj)
        {
            Phone p = obj as Phone;
            if ((object)p == null)
            {
                return false;
            }

            if (this.isValidFieldSpecified != p.isValidFieldSpecified ||
                (this.isValidFieldSpecified == true && p.isValidFieldSpecified == true && this.isValidField != p.isValidField))
            {
                return false;
            }

            if (this.verifiedFieldSpecified != p.verifiedFieldSpecified ||
                (this.verifiedFieldSpecified == true && p.verifiedFieldSpecified == true && this.verifiedField != p.verifiedField))
            {
                return false;
            }

            if (this.isPrimaryFieldSpecified != p.isPrimaryFieldSpecified ||
                (this.isPrimaryFieldSpecified == true && p.isPrimaryFieldSpecified == true && this.isPrimaryField != p.isPrimaryField))
            {
                return false;
            }

            return this.numberField == p.numberField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Phone a, Phone b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(Phone a, Phone b)
        {
            return !(a == b);
        }
    }

    public partial class PostalAddress
    {
        public override bool Equals(object obj)
        {
            PostalAddress p = obj as PostalAddress;
            if ((object)p == null)
            {
                return false;
            }

            if (this.isValidFieldSpecified != p.isValidFieldSpecified ||
                (this.isValidFieldSpecified == true && p.isValidFieldSpecified == true && this.isValidField != p.isValidField))
            {
                return false;
            }

            if (this.verifiedFieldSpecified != p.verifiedFieldSpecified ||
                (this.verifiedFieldSpecified == true && p.verifiedFieldSpecified == true && this.verifiedField != p.verifiedField))
            {
                return false;
            }

            return this.streetAddressLine1Field == p.streetAddressLine1Field &&
                   this.streetAddressLine2Field == p.streetAddressLine2Field &&
                   this.cityOrLocalityField == p.cityOrLocalityField &&
                   this.stateOrProvinceField == p.stateOrProvinceField &&
                   this.postalCodeField == p.postalCodeField &&
                   this.countryField == p.countryField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(PostalAddress a, PostalAddress b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(PostalAddress a, PostalAddress b)
        {
            return !(a == b);
        }
    }

    public partial class PhysicalLocation
    {
        public override bool Equals(object obj)
        {
            PhysicalLocation l = obj as PhysicalLocation;
            if ((object)l == null)
            {
                return false;
            }

            if (this.isPermanentFieldSpecified != l.isPermanentFieldSpecified ||
                (this.isPermanentFieldSpecified == true && l.isPermanentFieldSpecified == true && this.isPermanentField != l.isPermanentField))
            {
                return false;
            }

            if (this.isPrimaryFieldSpecified != l.isPrimaryFieldSpecified ||
                (this.isPrimaryFieldSpecified == true && l.isPrimaryFieldSpecified == true && this.isPrimaryField != l.isPrimaryField))
            {
                return false;
            }

            if (this.validFromFieldSpecified != l.validFromFieldSpecified ||
                (this.validFromFieldSpecified == true && l.validFromFieldSpecified == true && this.validFromField != l.validFromField))
            {
                return false;
            }

            if (this.validToFieldSpecified != l.validToFieldSpecified ||
                (this.validToFieldSpecified == true && l.validToFieldSpecified == true && this.validToField != l.validToField))
            {
                return false;
            }

            if (this.postalAddressField == l.postalAddressField &&
                this.labelField == l.labelField)
            {
                if (this.phoneField == null || l.phoneField == null)
                {
                    if (this.phoneField != null || l.phoneField != null)
                        return false;
                }
                else
                {
                    if (!this.phoneField.OrderBy(p => p.number).SequenceEqual(l.phoneField.OrderBy(p => p.number)))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(PhysicalLocation a, PhysicalLocation b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(PhysicalLocation a, PhysicalLocation b)
        {
            return !(a == b);
        }
    }

    public partial class ContactInfo
    {
        public override bool Equals(object obj)
        {
            ContactInfo c = obj as ContactInfo;
            if ((object)c == null)
            {
                return false;
            }

            if ((this.itemField as EmailAddress) != (c.itemField as EmailAddress))
            {
                return false;
            }

            if ((this.itemField as PhysicalLocation) != (c.itemField as PhysicalLocation))
            {
                return false;
            }

            if ((this.itemField as Phone) != (c.itemField as Phone))
            {
                return false;
            }

            return this.idField == c.idField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ContactInfo a, ContactInfo b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(ContactInfo a, ContactInfo b)
        {
            return !(a == b);
        }
    }

    public partial class NotificationMethod
    {
        public override bool Equals(object obj)
        {
            NotificationMethod n = obj as NotificationMethod;
            if ((object)n == null)
            {
                return false;
            }

            return this.labelField == n.labelField &&
                   this.businessContextField == n.businessContextField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(NotificationMethod a, NotificationMethod b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(NotificationMethod a, NotificationMethod b)
        {
            return !(a == b);
        }
    }

    public partial class Preferences
    {
        public override bool Equals(object obj)
        {
            Preferences p = obj as Preferences;
            if ((object)p == null)
            {
                return false;
            }

            if (this.noTelephoneCallsFieldSpecified != p.noTelephoneCallsFieldSpecified ||
                (this.noTelephoneCallsFieldSpecified == true && p.noTelephoneCallsFieldSpecified == true && this.noTelephoneCallsField != p.noTelephoneCallsField))
            {
                return false;
            }

            if (this.useHighContrastDisplayFieldSpecified != p.useHighContrastDisplayFieldSpecified ||
                (this.useHighContrastDisplayFieldSpecified == true && p.useHighContrastDisplayFieldSpecified == true && this.useHighContrastDisplayField != p.useHighContrastDisplayField))
            {
                return false;
            }

            return this.localeField == p.localeField &&
                   this.preferredNotificationMethodField == p.preferredNotificationMethodField &&
                   this.timezoneField == p.timezoneField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Preferences a, Preferences b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(Preferences a, Preferences b)
        {
            return !(a == b);
        }
    }

    public partial class WmsCircPatronInfo
    {
        public override bool Equals(object obj)
        {
            WmsCircPatronInfo w = obj as WmsCircPatronInfo;
            if ((object)w == null)
            {
                return false;
            }

            if (this.circRegistrationDateFieldSpecified != w.circRegistrationDateFieldSpecified ||
                (this.circRegistrationDateFieldSpecified == true && w.circRegistrationDateFieldSpecified == true && this.circRegistrationDateField != w.circRegistrationDateField))
            {
                return false;
            }

            if (this.circExpirationDateFieldSpecified != w.circExpirationDateFieldSpecified ||
                (this.circExpirationDateFieldSpecified == true && w.circExpirationDateFieldSpecified == true && this.circExpirationDateField != w.circExpirationDateField))
            {
                return false;
            }

            if (this.isCircBlockedFieldSpecified != w.isCircBlockedFieldSpecified ||
                (this.isCircBlockedFieldSpecified == true && w.isCircBlockedFieldSpecified == true && this.isCircBlockedField != w.isCircBlockedField))
            {
                return false;
            }

            if (this.isCollectionExemptFieldSpecified != w.isCollectionExemptFieldSpecified ||
                (this.isCollectionExemptFieldSpecified == true && w.isCollectionExemptFieldSpecified == true && this.isCollectionExemptField != w.isCollectionExemptField))
            {
                return false;
            }

            if (this.isFineExemptFieldSpecified != w.isFineExemptFieldSpecified ||
                (this.isFineExemptFieldSpecified == true && w.isFineExemptFieldSpecified == true && this.isFineExemptField != w.isFineExemptField))
            {
                return false;
            }

            if (this.isVerifiedFieldSpecified != w.isVerifiedFieldSpecified ||
                (this.isVerifiedFieldSpecified == true && w.isVerifiedFieldSpecified == true && this.isVerifiedField != w.isVerifiedField))
            {
                return false;
            }

            if (this.storeCheckoutHistoryFieldSpecified != w.storeCheckoutHistoryFieldSpecified ||
                (this.storeCheckoutHistoryFieldSpecified == true && w.storeCheckoutHistoryFieldSpecified == true && this.storeCheckoutHistoryField != w.storeCheckoutHistoryField))
            {
                return false;
            }

            if (this.waivedAmountFieldSpecified != w.waivedAmountFieldSpecified ||
                (this.waivedAmountFieldSpecified == true && w.waivedAmountFieldSpecified == true && this.waivedAmountField != w.waivedAmountField))
            {
                return false;
            }

            return this.barcodeField == w.barcodeField &&
                   this.barcodeStatusField == w.barcodeStatusField &&
                   this.pinField == w.pinField &&
                   this.borrowerCategoryField == w.borrowerCategoryField &&
                   this.homeBranchField == w.homeBranchField &&
                   this.claimedLostCountField == w.claimedLostCountField &&
                   this.claimedReturnedCountField == w.claimedReturnedCountField &&
                   this.claimedNeverHadCountField == w.claimedNeverHadCountField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(WmsCircPatronInfo a, WmsCircPatronInfo b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(WmsCircPatronInfo a, WmsCircPatronInfo b)
        {
            return !(a == b);
        }
    }

    public partial class NameInfo
    {
        public override bool Equals(object obj)
        {
            NameInfo n = obj as NameInfo;
            if ((object)n == null)
            {
                return false;
            }

            if (this.prefixField == n.prefixField &&
                   this.givenNameField == n.givenNameField &&
                   this.middleNameField == n.middleNameField &&
                   this.familyNameField == n.familyNameField &&
                   this.suffixField == n.suffixField &&
                   this.fullNameField == n.fullNameField &&
                   this.nicknameField == n.nicknameField)
            {
                if (this.otherNameInfoField == null || n.otherNameInfoField == null)
                {
                    if (this.otherNameInfoField != null || n.otherNameInfoField != null)
                        return false;
                }
                else
                {
                    if (!this.otherNameInfoField.OrderBy(c => c).SequenceEqual(n.otherNameInfoField.OrderBy(c => c)))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(NameInfo a, NameInfo b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null)
            {
                return (object)b == null;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(NameInfo a, NameInfo b)
        {
            return !(a == b);
        }
    }*/
}
