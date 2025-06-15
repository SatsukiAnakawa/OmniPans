// Presentation/Controls/InteractiveSlider/InteractiveSlider.cs
// マウスやキーボードで操作可能なカスタムスライダーコントロールのメイン定義部分です。
namespace OmniPans.Presentation.Controls;

using System.Windows.Controls;

[TemplatePart(Name = PartSliderContainer, Type = typeof(Grid))]
[TemplatePart(Name = PartTrackBackground, Type = typeof(Border))]
[TemplatePart(Name = PartTrackValue, Type = typeof(Border))]
[TemplatePart(Name = PartThumb, Type = typeof(Border))]
public partial class InteractiveSlider : Control
{
    #region ルーテッドイベント

    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            name: nameof(ValueChanged),
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedPropertyChangedEventHandler<double>),
            ownerType: typeof(InteractiveSlider));

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add { AddHandler(ValueChangedEvent, value); }
        remove { RemoveHandler(ValueChangedEvent, value); }
    }

    #endregion

    #region 定数 (テンプレートパーツ名)

    private const string PartSliderContainer = "PART_SliderContainer";
    private const string PartTrackBackground = "PART_TrackBackground";
    private const string PartTrackValue = "PART_TrackValue";
    private const string PartThumb = "PART_Thumb";

    #endregion

    #region フィールド (テンプレートパーツ参照)

    private Grid? _sliderContainerElement;
    private Border? _trackValueElement;
    private Border? _thumbElement;

    #endregion

    #region 静的コンストラクタ

    static InteractiveSlider()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InteractiveSlider), new FrameworkPropertyMetadata(typeof(InteractiveSlider)));
    }

    #endregion

    #region オーバーライド (コントロールライフサイクル)

    // コントロールのテンプレートが適用されたときに呼び出されます。
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (_sliderContainerElement is not null)
        {
            _sliderContainerElement.SizeChanged -= OnSliderContainerSizeChanged;
        }

        _sliderContainerElement = GetTemplateChild(PartSliderContainer) as Grid;
        _trackValueElement = GetTemplateChild(PartTrackValue) as Border;
        _thumbElement = GetTemplateChild(PartThumb) as Border;

        if (_sliderContainerElement is not null)
        {
            _sliderContainerElement.SizeChanged += OnSliderContainerSizeChanged;
        }

        UpdateSliderVisuals();
        UpdateFormattedValueTextInternal();
        UpdateVisualState(false);
    }

    // 依存関係プロパティの値が変更されたときに呼び出されます。
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsEnabledProperty)
        {
            UpdateVisualState();
        }
    }

    #endregion

    #region イベントハンドラ

    // スライダーコンテナのサイズが変更されたときにスライダーの見た目を更新します。
    private void OnSliderContainerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateSliderVisuals();
    }

    #endregion
}