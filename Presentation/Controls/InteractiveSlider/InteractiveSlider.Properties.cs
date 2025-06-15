// Presentation/Controls/InteractiveSlider/InteractiveSlider.Properties.cs
// InteractiveSliderコントロールの依存関係プロパティを定義します。
namespace OmniPans.Presentation.Controls;

public partial class InteractiveSlider
{
    #region 依存関係プロパティ (値と範囲)

    /// <summary>
    /// スライダーの現在の値を示す Value 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                           OnValuePropertyChanged_CallUpdates, CoerceValue));

    /// <summary>
    /// スライダーの現在の値を取得または設定します。
    /// </summary>
    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    /// <summary>
    /// スライダーの最小許容値を示す Minimum 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, OnMinMaxChanged_CallUpdates, CoerceMinimum));

    /// <summary>
    /// スライダーの最小許容値を取得または設定します。
    /// </summary>
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }

    /// <summary>
    /// スライダーの最大許容値を示す Maximum 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.None, OnMinMaxChanged_CallUpdates, CoerceMaximum));

    /// <summary>
    /// スライダーの最大許容値を取得または設定します。
    /// </summary>
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }

    #endregion

    #region 依存関係プロパティ (ラベルとテキスト表示)

    private static readonly DependencyPropertyKey FormattedValueTextPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(FormattedValueText), typeof(string), typeof(InteractiveSlider), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// 書式設定された現在の値を表す文字列を示す読み取り専用の FormattedValueText 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty FormattedValueTextProperty = FormattedValueTextPropertyKey.DependencyProperty;

    /// <summary>
    /// 書式設定された現在の値を表す文字列を取得します。
    /// </summary>
    public string FormattedValueText { get => (string)GetValue(FormattedValueTextProperty); private set => SetValue(FormattedValueTextPropertyKey, value); }

    /// <summary>
    /// 値を文字列に書式設定する際に使用するフォーマット文字列を示す ValueStringFormat 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ValueStringFormatProperty =
        DependencyProperty.Register(nameof(ValueStringFormat), typeof(string), typeof(InteractiveSlider),
            new PropertyMetadata("{0:F0}", OnValuePropertyChanged_CallUpdates));

    /// <summary>
    /// 値を文字列に書式設定する際に使用するフォーマット文字列を取得または設定します。
    /// </summary>
    public string ValueStringFormat { get => (string)GetValue(ValueStringFormatProperty); set => SetValue(ValueStringFormatProperty, value); }

    /// <summary>
    /// 値を表示するテキストブロックの幅を示す ValueTextWidth 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ValueTextWidthProperty =
        DependencyProperty.Register(nameof(ValueTextWidth), typeof(double), typeof(InteractiveSlider), new PropertyMetadata(30.0));

    /// <summary>
    /// 値を表示するテキストブロックの幅を取得または設定します。
    /// </summary>
    public double ValueTextWidth { get => (double)GetValue(ValueTextWidthProperty); set => SetValue(ValueTextWidthProperty, value); }

    /// <summary>
    /// 値を表示するテキストのフォントサイズを示す ValueTextFontSize 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ValueTextFontSizeProperty =
        DependencyProperty.Register(nameof(ValueTextFontSize), typeof(double), typeof(InteractiveSlider), new PropertyMetadata(13.0));

    /// <summary>
    /// 値を表示するテキストのフォントサイズを取得または設定します。
    /// </summary>
    public double ValueTextFontSize { get => (double)GetValue(ValueTextFontSizeProperty); set => SetValue(ValueTextFontSizeProperty, value); }

    #endregion

    #region 依存関係プロパティ (トラックの見た目)

    /// <summary>
    /// スライダートラックの背景ブラシを示す TrackBackgroundBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty TrackBackgroundBrushProperty =
        DependencyProperty.Register(nameof(TrackBackgroundBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// スライダートラックの背景ブラシを取得または設定します。
    /// </summary>
    public System.Windows.Media.Brush TrackBackgroundBrush { get => (Brush)GetValue(TrackBackgroundBrushProperty); set => SetValue(TrackBackgroundBrushProperty, value); }

    /// <summary>
    /// スライダーの値部分を示すトラックのブラシを示す TrackValueBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty TrackValueBrushProperty =
        DependencyProperty.Register(nameof(TrackValueBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// スライダーの値部分を示すトラックのブラシを取得または設定します。
    /// </summary>
    public Brush TrackValueBrush { get => (Brush)GetValue(TrackValueBrushProperty); set => SetValue(TrackValueBrushProperty, value); }

    /// <summary>
    /// スライダートラックの高さを示す TrackHeight 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty TrackHeightProperty =
        DependencyProperty.Register(nameof(TrackHeight), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, OnVisualRelatedPropertyChanged));

    /// <summary>
    /// スライダートラックの高さを取得または設定します。
    /// </summary>
    public double TrackHeight { get => (double)GetValue(TrackHeightProperty); set => SetValue(TrackHeightProperty, value); }

    #endregion

    #region 依存関係プロパティ (Thumbの見た目)

    /// <summary>
    /// Thumb（つまみ）の背景ブラシを示す ThumbBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbBrushProperty =
        DependencyProperty.Register(nameof(ThumbBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Thumb（つまみ）の背景ブラシを取得または設定します。
    /// </summary>
    public Brush ThumbBrush { get => (Brush)GetValue(ThumbBrushProperty); set => SetValue(ThumbBrushProperty, value); }

    /// <summary>
    /// Thumb（つまみ）の境界線ブラシを示す ThumbBorderBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbBorderBrushProperty =
        DependencyProperty.Register(nameof(ThumbBorderBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkSlateGray), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Thumb（つまみ）の境界線ブラシを取得または設定します。
    /// </summary>
    public Brush ThumbBorderBrush { get => (Brush)GetValue(ThumbBorderBrushProperty); set => SetValue(ThumbBorderBrushProperty, value); }

    /// <summary>
    /// マウスオーバー時のThumb（つまみ）の背景ブラシを示す ThumbMouseOverBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbMouseOverBrushProperty =
        DependencyProperty.Register(nameof(ThumbMouseOverBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// マウスオーバー時のThumb（つまみ）の背景ブラシを取得または設定します。
    /// </summary>
    public Brush ThumbMouseOverBrush { get => (Brush)GetValue(ThumbMouseOverBrushProperty); set => SetValue(ThumbMouseOverBrushProperty, value); }

    /// <summary>
    /// マウスオーバー時のThumb（つまみ）の境界線ブラシを示す ThumbMouseOverBorderBrush 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbMouseOverBorderBrushProperty =
        DependencyProperty.Register(nameof(ThumbMouseOverBorderBrush), typeof(Brush), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(Brushes.SlateGray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// マウスオーバー時のThumb（つまみ）の境界線ブラシを取得または設定します。
    /// </summary>
    public Brush ThumbMouseOverBorderBrush { get => (Brush)GetValue(ThumbMouseOverBorderBrushProperty); set => SetValue(ThumbMouseOverBorderBrushProperty, value); }

    /// <summary>
    /// Thumb（つまみ）の幅を示す ThumbWidth 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbWidthProperty =
        DependencyProperty.Register(nameof(ThumbWidth), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, OnVisualRelatedPropertyChanged));

    /// <summary>
    /// Thumb（つまみ）の幅を取得または設定します。
    /// </summary>
    public double ThumbWidth { get => (double)GetValue(ThumbWidthProperty); set => SetValue(ThumbWidthProperty, value); }

    /// <summary>
    /// Thumb（つまみ）の高さを示す ThumbHeight 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ThumbHeightProperty =
        DependencyProperty.Register(nameof(ThumbHeight), typeof(double), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(18.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, OnVisualRelatedPropertyChanged));

    /// <summary>
    /// Thumb（つまみ）の高さを取得または設定します。
    /// </summary>
    public double ThumbHeight { get => (double)GetValue(ThumbHeightProperty); set => SetValue(ThumbHeightProperty, value); }

    #endregion

    #region 依存関係プロパティ (インタラクション)

    /// <summary>
    /// ダブルクリックで値をリセットする機能が有効かどうかを示す IsDoubleClickToResetEnabled 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty IsDoubleClickToResetEnabledProperty =
        DependencyProperty.Register(nameof(IsDoubleClickToResetEnabled), typeof(bool), typeof(InteractiveSlider), new PropertyMetadata(false));

    /// <summary>
    /// ダブルクリックで値をリセットする機能が有効かどうかを取得または設定します。
    /// </summary>
    public bool IsDoubleClickToResetEnabled { get => (bool)GetValue(IsDoubleClickToResetEnabledProperty); set => SetValue(IsDoubleClickToResetEnabledProperty, value); }

    /// <summary>
    /// ダブルクリック時にリセットされる値を示す ResetValue 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty ResetValueProperty =
        DependencyProperty.Register(nameof(ResetValue), typeof(double), typeof(InteractiveSlider), new PropertyMetadata(0.0));

    /// <summary>
    /// ダブルクリック時にリセットされる値を取得または設定します。
    /// </summary>
    public double ResetValue { get => (double)GetValue(ResetValueProperty); set => SetValue(ResetValueProperty, value); }

    /// <summary>
    /// キーボードの矢印キーで値を変更する際のステップ量を示す KeyboardStep 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty KeyboardStepProperty =
        DependencyProperty.Register(nameof(KeyboardStep), typeof(double), typeof(InteractiveSlider), new PropertyMetadata(1.0));

    /// <summary>
    /// キーボードの矢印キーで値を変更する際のステップ量を取得または設定します。
    /// </summary>
    public double KeyboardStep { get => (double)GetValue(KeyboardStepProperty); set => SetValue(KeyboardStepProperty, value); }

    /// <summary>
    /// マウスホイールで値を変更する際のステップ量を示す MouseWheelStep 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty MouseWheelStepProperty =
        DependencyProperty.Register(nameof(MouseWheelStep), typeof(double), typeof(InteractiveSlider), new PropertyMetadata(2.0));

    /// <summary>
    /// マウスホイールで値を変更する際のステップ量を取得または設定します。
    /// </summary>
    public double MouseWheelStep { get => (double)GetValue(MouseWheelStepProperty); set => SetValue(MouseWheelStepProperty, value); }

    #endregion

    #region 依存関係プロパティ (振る舞い)

    /// <summary>
    /// スライダーの視覚的な計算ロジックを定義する計算機クラスを示す Calculator 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty CalculatorProperty =
        DependencyProperty.Register(nameof(Calculator), typeof(ISliderVisualsCalculator), typeof(InteractiveSlider),
            new FrameworkPropertyMetadata(new NormalSliderCalculator(), OnVisualRelatedPropertyChanged));

    /// <summary>
    /// スライダーの視覚的な計算ロジックを定義する計算機クラスを取得または設定します。
    /// </summary>
    public ISliderVisualsCalculator Calculator { get => (ISliderVisualsCalculator)GetValue(CalculatorProperty); set => SetValue(CalculatorProperty, value); }

    /// <summary>
    /// スライダー部分のコンテナの幅を示す SliderContainerWidth 依存関係プロパティ。
    /// </summary>
    public static readonly DependencyProperty SliderContainerWidthProperty =
        DependencyProperty.Register(
            name: nameof(SliderContainerWidth),
            propertyType: typeof(double),
            ownerType: typeof(InteractiveSlider),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: 230.0,
                flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
        );

    /// <summary>
    /// スライダー部分のコンテナの幅を取得または設定します。
    /// </summary>
    public double SliderContainerWidth { get => (double)GetValue(SliderContainerWidthProperty); set => SetValue(SliderContainerWidthProperty, value); }

    #endregion

    #region プロパティ変更コールバック

    private static void OnValuePropertyChanged_CallUpdates(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is InteractiveSlider slider)
        {
            slider.UpdateSliderVisuals();
            slider.UpdateFormattedValueTextInternal();

            if (e.Property == ValueProperty)
            {
                var args = new RoutedPropertyChangedEventArgs<double>(
                    (double)e.OldValue,
                    (double)e.NewValue,
                    ValueChangedEvent);
                slider.RaiseEvent(args);
            }
        }
    }

    private static void OnMinMaxChanged_CallUpdates(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is InteractiveSlider slider)
        {
            slider.CoerceValue(ValueProperty);
            slider.UpdateSliderVisuals();
            slider.UpdateFormattedValueTextInternal();
        }
    }

    private static void OnVisualRelatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is InteractiveSlider slider)
        {
            slider.UpdateSliderVisuals();
        }
    }

    #endregion

    #region Coerceコールバック (値の強制)

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        if (d is InteractiveSlider s && baseValue is double v)
        {
            return Math.Clamp(v, s.Minimum, s.Maximum);
        }
        return baseValue;
    }

    private static object CoerceMinimum(DependencyObject d, object baseValue)
    {
        if (d is InteractiveSlider s && baseValue is double v)
        {
            return Math.Min(v, s.Maximum);
        }
        return baseValue;
    }

    private static object CoerceMaximum(DependencyObject d, object baseValue)
    {
        if (d is InteractiveSlider s && baseValue is double v)
        {
            return Math.Max(v, s.Minimum);
        }
        return baseValue;
    }

    #endregion
}