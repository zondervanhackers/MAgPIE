using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Fields
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DelimitedFieldAttribute : Attribute, IFieldAttribute
    {
        public Boolean IsRequired { get; set; }

        public String NullPattern { get; set; }

        public String ValidationPattern { get; set; }

        public String[] RemoveCharacters { get; set; }
    }
}
