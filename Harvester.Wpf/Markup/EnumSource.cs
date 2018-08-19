using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using System.Diagnostics.Contracts;
using System.ComponentModel;

namespace ZondervanLibrary.Harvester.Wpf.Markup
{
    public class EnumSource : MarkupExtension
    {
        private readonly Type _enumType;

        public EnumSource(Type enumType)
        {
            Contract.Requires(enumType != null);

            _enumType = enumType;
        }

        private String GetDisplayName(object value)
        {
            String stringValue = value.ToString();
            DescriptionAttribute[] attributes = _enumType.GetField(stringValue).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                // Use provided description attribute.
                stringValue = attributes[0].Description;
            }
            else
            {
                // Look for a non upper case letter followed by an uppercase letter.
                Regex postfix = new Regex(@"([^A-Z])([A-Z])");
                stringValue = postfix.Replace(stringValue, m => $"{m.Groups[1].Value} {m.Groups[2].Value}");
            }

            return stringValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType).Cast<object>().Select(e => new { Value = e, DisplayName = GetDisplayName(e) });
        }
    }   
}
