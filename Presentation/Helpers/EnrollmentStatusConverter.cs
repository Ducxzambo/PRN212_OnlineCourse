using System.Globalization;
using System.Windows.Data;

namespace Presentation.Helpers;

public class EnrollmentStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int status)
            return EnrollmentStatusOptions.All.FirstOrDefault(o => o.Value == status)?.Text ?? status.ToString();

        return value?.ToString() ?? "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

