using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Fields
{
    public interface IFieldAttribute
    {
        Boolean IsRequired { get; set; }

        String NullPattern { get; set; }

        String ValidationPattern { get; set; }

        String[] RemoveCharacters { get; set; }
    }
}
