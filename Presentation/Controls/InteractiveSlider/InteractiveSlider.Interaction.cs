// Presentation/Controls/InteractiveSlider/InteractiveSlider.Interaction.cs
// InteractiveSliderコントロールのユーザー操作（マウス、キーボード）に関するロジックです。
namespace OmniPans.Presentation.Controls;

public partial class InteractiveSlider
{
    #region フィールド

    private bool _isDragging;

    #endregion

    #region オーバーライド (ユーザーインタラクション)

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        UpdateVisualState();
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        UpdateVisualState();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (!IsEnabled || e.ButtonState != MouseButtonState.Pressed) return;

        Focus();

        if (IsDoubleClickToResetEnabled && e.ClickCount == 2)
        {
            Value = ResetValue;
            e.Handled = true;
            UpdateVisualState();
            return;
        }

        if (CaptureMouse())
        {
            _isDragging = true;
            if (_sliderContainerElement is not null)
            {
                Value = CalculateValueFromMousePosition(e.GetPosition(_sliderContainerElement).X);
            }
        }
        UpdateVisualState();
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (IsMouseCaptured && _isDragging)
        {
            ReleaseMouseCapture();
        }
        _isDragging = false;
        UpdateVisualState();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging && _sliderContainerElement is not null)
        {
            double preciseValue = CalculateValueFromMousePosition(e.GetPosition(_sliderContainerElement).X);
            Value = Math.Round(preciseValue, MidpointRounding.AwayFromZero);
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        if (!IsEnabled || e.Delta == 0) return;

        double step = e.Delta > 0 ? MouseWheelStep : -MouseWheelStep;
        Value = Math.Clamp(Value + step, Minimum, Maximum);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!IsEnabled) return;

        double step = KeyboardStep;
        var (newValue, handled) = e.Key switch
        {
            Key.Left or Key.Down => (Value - step, true),
            Key.Right or Key.Up => (Value + step, true),
            Key.Home => (Minimum, true),
            Key.End => (Maximum, true),
            _ => (Value, false)
        };
        if (handled)
        {
            Value = Math.Clamp(newValue, Minimum, Maximum);
            e.Handled = true;
        }
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        UpdateVisualState();
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        UpdateVisualState();
    }

    #endregion

    #region 非公開ヘルパー

    // マウスカーソルのX座標からスライダーの値を計算します。
    private double CalculateValueFromMousePosition(double mouseX)
    {
        if (_sliderContainerElement is null || _sliderContainerElement.ActualWidth <= 0 || Maximum <= Minimum)
        {
            return Value;
        }

        double ratio = Math.Clamp(mouseX / _sliderContainerElement.ActualWidth, 0.0, 1.0);
        double newValue = Minimum + (ratio * (Maximum - Minimum));
        return Math.Clamp(newValue, Minimum, Maximum);
    }

    #endregion
}