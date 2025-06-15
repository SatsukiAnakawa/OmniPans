// Presentation/Controls/InteractiveSlider/Calculators/NormalSliderCalculator.cs
// 通常の（左から右への）InteractiveSliderの見た目を計算するクラスです。
namespace OmniPans.Presentation.Controls;

public class NormalSliderCalculator : ISliderVisualsCalculator
{
    // 通常のスライダーの見た目を計算します。
    public SliderVisuals Calculate(SliderMetrics metrics)
    {
        double range = metrics.Maximum - metrics.Minimum;
        double percentage = (range > double.Epsilon) ? (metrics.Value - metrics.Minimum) / range : 0;
        double trackValueWidth = Math.Clamp(percentage * metrics.TrackActualWidth, 0, metrics.TrackActualWidth);
        double thumbTargetCenterPosition = Math.Clamp(percentage, 0.0, 1.0) * metrics.TrackActualWidth;
        double thumbLeftMargin = thumbTargetCenterPosition - (metrics.ThumbActualWidth / 2.0);
        thumbLeftMargin = Math.Clamp(thumbLeftMargin, 0, metrics.TrackActualWidth - metrics.ThumbActualWidth);

        return new SliderVisuals(
            trackValueWidth,
            new Thickness(0),
            HorizontalAlignment.Left,
            thumbLeftMargin
        );
    }
}