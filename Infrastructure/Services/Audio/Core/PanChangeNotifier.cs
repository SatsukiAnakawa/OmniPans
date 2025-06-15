
// Infrastructure/Services/Audio/Core/PanChangeNotifier.cs
// オーディオデバイスのパン変更を監視し、デバウンス処理後に通知メッセージを送信します。
namespace OmniPans.Infrastructure.Services.Audio;

[SupportedOSPlatform("windows")]
public sealed class PanChangeNotifier : IDisposable
{
    #region フィールド

    private readonly ILogger<PanChangeNotifier> _logger;
    private readonly ICoreAudioDeviceService _coreAudioDeviceService;
    private readonly IDispatcherService _dispatcherService;
    private readonly Subject<string> _propertyChangedSubject = new();
    private readonly IDisposable _propertyChangedSubscription;
    private bool _isDisposed;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// <see cref="PanChangeNotifier"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="logger">ロギングサービス。</param>
    /// <param name="coreAudioDeviceService">Core Audioデバイス操作サービス。</param>
    /// <param name="dispatcherService">UIスレッド操作サービス。</param>
    /// <param name="config">振る舞いに関する設定。</param>
    public PanChangeNotifier(
        ILogger<PanChangeNotifier> logger,
        ICoreAudioDeviceService coreAudioDeviceService,
        IDispatcherService dispatcherService,
        Core.Models.Configuration.BehaviorConfig config)
    {
        _logger = logger;
        _coreAudioDeviceService = coreAudioDeviceService;
        _dispatcherService = dispatcherService;

        _coreAudioDeviceService.DevicePropertyChanged += OnCoreDevicePropertyChanged;

        _propertyChangedSubscription = _propertyChangedSubject
            .GroupBy(deviceId => deviceId)
            .Select(group => group.Throttle(TimeSpan.FromMilliseconds(config.OsPanNotificationDebounceMs)))
            .Merge()
            .Subscribe(ProcessDebouncedPropertyChange);
    }

    #endregion

    #region Private Methods

    // CoreAudioからのプロパティ変更イベントをSubjectに流します。
    private void OnCoreDevicePropertyChanged(object? sender, DevicePropertyChangedArgs args)
    {
        _propertyChangedSubject.OnNext(args.DeviceId);
    }

    // デバウンス処理されたプロパティ変更イベントを処理し、パンの変更を通知します。
    private void ProcessDebouncedPropertyChange(string deviceId)
    {
        _dispatcherService.Invoke(() =>
        {
            using var mmDevice = _coreAudioDeviceService.GetDeviceById(deviceId);
            if (mmDevice?.AudioEndpointVolume?.Channels is not { Count: >= 2 } channels)
            {
                return;

            }

            try
            {
                var newPan = PanCalculator.ConvertScalarsToPan(channels[0].VolumeLevelScalar, channels[1].VolumeLevelScalar);
                WeakReferenceMessenger.Default.Send(new OsPanChangedMessage(deviceId, Math.Round(newPan)));
                _logger.LogDebug("デバイス {DeviceId} のパン変更を通知 (新しい値: {NewPan})。", deviceId, newPan);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "デバイス {DeviceId} のパン設定の読み取り中にエラーが発生しました。", deviceId);
            }
        });
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// このオブジェクトが使用するリソースを解放します。
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed) return;
        _coreAudioDeviceService.DevicePropertyChanged -= OnCoreDevicePropertyChanged;
        _propertyChangedSubscription?.Dispose();
        _propertyChangedSubject?.Dispose();
        _isDisposed = true;
    }

    #endregion
}