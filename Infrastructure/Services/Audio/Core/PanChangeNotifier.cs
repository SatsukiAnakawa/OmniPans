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
    private readonly IMessenger _messenger;
    private readonly Subject<string> _propertyChangedSubject = new();
    private readonly IDisposable _propertyChangedSubscription;
    private bool _isDisposed;

    #endregion

    #region コンストラクタ

    // PanChangeNotifier クラスの新しいインスタンスを初期化します。
    public PanChangeNotifier(
        ILogger<PanChangeNotifier> logger,
        ICoreAudioDeviceService coreAudioDeviceService,
        IDispatcherService dispatcherService,
        IMessenger messenger,
        Core.Models.Configuration.BehaviorConfig config)
    {
        _logger = logger;
        _coreAudioDeviceService = coreAudioDeviceService;
        _dispatcherService = dispatcherService;
        _messenger = messenger;

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
            try
            {
                using var mmDevice = _coreAudioDeviceService.GetDeviceById(deviceId);
                if (mmDevice is null)
                {
                    return;
                }

                if (mmDevice.AudioEndpointVolume?.Channels is not { Count: >= 2 } channels)
                {
                    return;
                }

                var newPan = PanCalculator.ConvertScalarsToPan(channels[0].VolumeLevelScalar, channels[1].VolumeLevelScalar);
                _messenger.Send(new OsPanChangedMessage(deviceId, Math.Round(newPan)));
                _logger.LogDebug("デバイス {DeviceId} のパン変更を通知 (新しい値: {NewPan})。", deviceId, newPan);
            }
            catch (Exception ex) when (ex is System.Runtime.InteropServices.COMException or System.IO.FileNotFoundException)
            {
                _logger.LogWarning(ex, "デバイス {DeviceId} のパン状態取得中にエラーが発生しました。デバイスが無効になった可能性があります。", deviceId);
            }
        });
    }

    #endregion

    #region IDisposable Implementation

    // このオブジェクトが使用するリソースを解放します。
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
