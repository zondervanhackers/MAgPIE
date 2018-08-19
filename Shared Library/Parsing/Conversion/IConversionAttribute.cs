using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Conversion
{
    public interface IConversionAttribute<TField>
    {
        TField Convert(String input);
    }
}
