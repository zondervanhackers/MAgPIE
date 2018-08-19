using System;
using System.Windows;
using System.Windows.Data;
using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf.Converters
{
    [ValueConversion(typeof(SqlServerAuthenticationMethod), typeof(Visibility))]
    public class SqlServerAuthenticationConverter : BaseConverter<SqlServerAuthenticationMethod, Visibility>
    {
        protected override Visibility Convert(SqlServerAuthenticationMethod value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == SqlServerAuthenticationMethod.SqlServer ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
