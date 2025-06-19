// Presentation/Converters/PanToOpacityConverter.cs
// パンの値と方向パラメータに基づき、アイコンの不透明度を計算します。
using System.Windows.Data;

namespace OmniPans.Presentation.Converters;

public class PanToOpacityConverter : IMultiValueConverter
{
    private const double MinOpacity = 0.3;
    private const double MaxOpacity = 1.0;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not double pan || values[1] is not string direction)
        {
            return MaxOpacity;
        }

        if (pan < 0) // Panが左側
        {
            return direction == "Left"
                ? MaxOpacity
                : MinOpacity + (MaxOpacity - MinOpacity) * (pan + 100.0) / 100.0;
        }
        else // Panが中央または右側
        {
            return direction == "Right"
                ? MaxOpacity
                : MinOpacity + (MaxOpacity - MinOpacity) * (100.0 - pan) / 100.0;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
