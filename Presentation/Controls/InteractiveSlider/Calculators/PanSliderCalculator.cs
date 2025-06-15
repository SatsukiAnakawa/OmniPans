// Presentation/Controls/InteractiveSlider/Calculators/PanSliderCalculator.cs
// パン（左右バランス）用途のInteractiveSliderの見た目を計算するクラスです。
namespace OmniPans.Presentation.Controls;

public class PanSliderCalculator : ISliderVisualsCalculator
{
    // パン（左右バランス）スライダーの見た目を計算します。
    public SliderVisuals Calculate(SliderMetrics metrics)
    {
        double trackHalfWidth = metrics.TrackActualWidth / 2.0;
        double trackValueWidth;
        Thickness trackValueMargin;
        HorizontalAlignment trackValueAlignment;

        if (metrics.Value.CompareTo(0.0) >= 0) // 中央または右
        {
            trackValueAlignment = HorizontalAlignment.Left;
            trackValueMargin = new Thickness(trackHalfWidth, 0, 0, 0);
            double valueRatio = (metrics.Maximum.CompareTo(0.0) == 0) ? 0 : metrics.Value / metrics.Maximum;
            trackValueWidth = Math.Clamp(valueRatio * trackHalfWidth, 0, trackHalfWidth);
        }
        else // 左
        {
            trackValueAlignment = HorizontalAlignment.Right;
            trackValueMargin = new Thickness(0, 0, trackHalfWidth, 0);
            double valueRatio = (metrics.Minimum.CompareTo(0.0) == 0) ? 0 : metrics.Value / metrics.Minimum;
            trackValueWidth = Math.Clamp(valueRatio * trackHalfWidth, 0, trackHalfWidth);
        }

        double range = metrics.Maximum - metrics.Minimum;
        double percentage = (range > double.Epsilon) ? (metrics.Value - metrics.Minimum) / range : 0;
        double thumbTargetCenterPosition = Math.Clamp(percentage, 0.0, 1.0) * metrics.TrackActualWidth;
        double thumbLeftMargin = thumbTargetCenterPosition - (metrics.ThumbActualWidth / 2.0);
        thumbLeftMargin = Math.Clamp(thumbLeftMargin, 0, metrics.TrackActualWidth - metrics.ThumbActualWidth);

        return new SliderVisuals(
            trackValueWidth,
            trackValueMargin,
            trackValueAlignment,
            thumbLeftMargin
        );
    }
}