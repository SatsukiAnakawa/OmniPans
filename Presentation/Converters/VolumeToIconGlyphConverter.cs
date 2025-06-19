// Presentation/Converters/VolumeToIconGlyphConverter.cs
// double型の音量値を、対応するアイコンのGlyph文字列に変換します。
namespace OmniPans.Presentation.Converters;

using OmniPans.Presentation.Constants;
using System.Windows.Data;

public class VolumeToIconGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double volume)
        {
            return IconGlyphs.VolumeLevel3;
        }

        if (Math.Abs(volume) < double.Epsilon)
        {
            return IconGlyphs.VolumeMute;
        }
        else if (volume < AppConstants.VolumeThresholdLow)
        {
            return IconGlyphs.VolumeLevel1;
        }
        else if (volume < AppConstants.VolumeThresholdMid)
        {
            return IconGlyphs.VolumeLevel2;
        }
        else
        {
            return IconGlyphs.VolumeLevel3;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
