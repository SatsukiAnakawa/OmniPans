// Presentation/Controls/InteractiveSlider/Calculators/ISliderVisualsCalculator.cs
// InteractiveSliderの見た目（トラックとThumbの位置・サイズ）を計算するロジックのインターフェースです。
namespace OmniPans.Presentation.Controls;

// スライダーの構成要素のプロパティを計算するための入力情報を提供します。
public record SliderMetrics(
    double TrackActualWidth,
    double ThumbActualWidth,
    double Value,
    double Minimum,
    double Maximum
);

// 計算されたスライダーの見た目の結果を表します。
public record SliderVisuals(
    double TrackValueWidth,
    Thickness TrackValueMargin,
    HorizontalAlignment TrackValueHorizontalAlignment,
    double ThumbLeftMargin
);

public interface ISliderVisualsCalculator
{
    // スライダーの現在の状態に基づいて、トラックとThumbの視覚的なプロパティを計算します。
    SliderVisuals Calculate(SliderMetrics metrics);
}