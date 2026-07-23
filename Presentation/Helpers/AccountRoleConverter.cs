using System.Globalization;
using System.Windows.Data;

namespace Presentation.Helpers;

public class AccountRoleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int role)
        {
            return role switch
            {
                1 => "Admin",
                2 => "Student",
                _ => "Instructor"
            };
        }

        return value?.ToString() ?? "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

