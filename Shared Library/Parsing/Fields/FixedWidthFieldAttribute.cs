using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Fields
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FixedWidthFieldAttribute : Attribute, IFieldAttribute
    {
        public FixedWidthFieldAttribute(Int32 fieldSize)
        {
            FieldSize = fieldSize;
        }

        public Int32 FieldSize { get; }

        public Boolean IsRequired { get; set; }

        public String NullPattern { get; set; }

        public String ValidationPattern { get; set; }
        public String[] RemoveCharacters { get; set; }
    }
}
