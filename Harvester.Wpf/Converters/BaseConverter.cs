using System;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;

namespace ZondervanLibrary.Harvester.Wpf.Converters
{
    public abstract class BaseConverter<TFrom, TTo> : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TFrom)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo)value, targetType, parameter, culture);
        }

        protected virtual TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected virtual TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
