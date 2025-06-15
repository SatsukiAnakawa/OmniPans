// Presentation/Controls/InteractiveSlider/InteractiveSlider.Visuals.cs
// InteractiveSliderコントロールの見た目の更新に関するロジックです。
namespace OmniPans.Presentation.Controls;

public partial class InteractiveSlider
{
    #region 非公開メソッド (見た目更新ヘルパー)

    // 数値書式指定子を文字列から抽出します。
    private static string ExtractNumericFormatSpecifier(string? formatString, string defaultSpecifier = "F0")
    {
        if (string.IsNullOrEmpty(formatString) || formatString == "{0}") return defaultSpecifier;
        int colonIndex = formatString.IndexOf(':');
        if (colonIndex >= 0 && formatString.EndsWith('}') && formatString.Length > colonIndex + 2)
        {
            return formatString.Substring(colonIndex + 1, formatString.Length - 1 - (colonIndex + 1));
        }
        return defaultSpecifier;
    }

    // 書式指定された値のテキストを更新します。
    private void UpdateFormattedValueTextInternal()
    {
        if (Calculator is PanSliderCalculator)
        {
            string specifier = ExtractNumericFormatSpecifier(ValueStringFormat);
            string valStr = Value.ToString(specifier, CultureInfo.InvariantCulture);
            string absValStr = Math.Abs(Value).ToString(specifier, CultureInfo.InvariantCulture);
            FormattedValueText = Value.CompareTo(0.0) switch
            {
                > 0 => $"{valStr}R",
                < 0 => $"L{absValStr}",
                _ => valStr
            };
        }
        else
        {
            try
            {
                FormattedValueText = string.Format(CultureInfo.InvariantCulture, string.IsNullOrEmpty(ValueStringFormat) ? "{0}" : ValueStringFormat, Value);
            }
            catch (FormatException)
            {
                FormattedValueText = Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
    #endregion

    #region 保護された仮想メソッド (見た目更新)

    // スライダーの視覚的な外観（トラックとThumbの位置）を更新します。
    protected virtual void UpdateSliderVisuals()
    {
        if (_trackValueElement is null || _thumbElement is null || _sliderContainerElement is null || Maximum <= Minimum)
        {
            if (_trackValueElement is not null) _trackValueElement.Width = 0;
            if (_thumbElement is not null) _thumbElement.Margin = new Thickness(0);
            return;
        }

        if (_sliderContainerElement.ActualWidth <= 0)
        {
            return;
        }

        var calculator = Calculator;
        var metrics = new SliderMetrics(
            _sliderContainerElement.ActualWidth,
            _thumbElement.ActualWidth > 0 ? _thumbElement.ActualWidth : ThumbWidth,
            Value,
            Minimum,
            Maximum
        );
        var visuals = calculator.Calculate(metrics);

        _trackValueElement.HorizontalAlignment = visuals.TrackValueHorizontalAlignment;
        _trackValueElement.Margin = visuals.TrackValueMargin;
        _trackValueElement.Width = visuals.TrackValueWidth;
        _thumbElement.Margin = new Thickness(visuals.ThumbLeftMargin, 0, 0, 0);
    }

    // コントロールの視覚的な状態（通常、マウスオーバーなど）を更新します。
    protected virtual void UpdateVisualState(bool useTransitions = true)
    {
        string commonState = GetCommonVisualState();
        VisualStateManager.GoToState(this, commonState, useTransitions);

        string focusState = IsKeyboardFocused ? "Focused" : "Unfocused";
        VisualStateManager.GoToState(this, focusState, useTransitions);
    }

    // 現在のコモン状態（通常、マウスオーバー、無効）を取得します。
    private string GetCommonVisualState()
    {
        return (IsEnabled, IsMouseOver) switch
        {
            (false, _) => "Disabled",
            (true, true) => "MouseOver",
            _ => "Normal"
        };
    }

    #endregion
}