// Presentation/Converters/DoubleGreaterThanZeroToVisibilityConverter.cs
// double値が0より大きい場合にVisibleを返すコンバーターです。
namespace OmniPans.Presentation.Converters;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

public class DoubleGreaterThanZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return Math.Abs(d) > double.Epsilon ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
