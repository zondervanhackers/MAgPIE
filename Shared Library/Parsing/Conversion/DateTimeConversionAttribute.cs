using System;
using System.Globalization;

namespace ZondervanLibrary.SharedLibrary.Parsing.Conversion
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateTimeConversionAttribute : Attribute, IConversionAttribute<DateTime>
    {
        private readonly String[] _formats;

        public DateTimeConversionAttribute(params String[] format)
        {
            _formats = format.Length == 0 ? 
                throw new ArgumentException("No formats provided") : 
                format;
        }

        public DateTime Convert(String input)
        {
            try
            {
                return DateTime.ParseExact(input, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
