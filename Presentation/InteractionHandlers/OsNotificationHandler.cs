// Presentation/InteractionHandlers/OsNotificationHandler.cs
// OSからの音量変更通知をハンドリングし、UI更新のためのメッセージを送信します。
namespace OmniPans.Presentation.InteractionHandlers;

public class OsNotificationHandler : IOsNotificationHandler
{
    #region フィールド

    private readonly string _deviceId;
    private readonly IUserInteractionTracker _userInteractionTracker;
    private readonly IMessenger _messenger;
    private readonly IDisposable _subscription;
    private readonly Subject<float> _notificationSubject = new();
    private readonly int _gracePeriodMs;
    private bool _isDisposed;

    #endregion

    #region コンストラクタ

    // OsNotificationHandler クラスの新しいインスタンスを初期化します。
    public OsNotificationHandler(
        string deviceId,
        IDispatcherService dispatcherService,
        IUserInteractionTracker userInteractionTracker,
        IMessenger messenger,
        int debounceTimeMs,
        int gracePeriodMs)
    {
        _deviceId = deviceId;
        _userInteractionTracker = userInteractionTracker;
        _messenger = messenger;
        _gracePeriodMs = gracePeriodMs;

        _subscription = _notificationSubject
            .Throttle(TimeSpan.FromMilliseconds(debounceTimeMs))
            .ObserveOn(new DispatcherScheduler(dispatcherService.Dispatcher))
            .Subscribe(ProcessDebouncedNotification);
    }

    #endregion

    #region Public Methods

    // OSからの音量変更通知を処理します。ユーザー自身の操作直後の通知は無視します。
    public void HandleOsVolumeNotification(float newVolumeScalar)
    {
        long elapsedMilliseconds = (long)(DateTime.UtcNow - _userInteractionTracker.GetLastUserInteractionTime(_deviceId)).TotalMilliseconds;
        if (elapsedMilliseconds < _gracePeriodMs)
        {
            return;
        }
        _notificationSubject.OnNext(newVolumeScalar);
    }

    #endregion

    #region Private Methods

    // デバウンス処理された後の通知を処理します。
    private void ProcessDebouncedNotification(float newVolumeScalar)
    {
        double newVolume = Math.Round(newVolumeScalar * 100.0);
        _messenger.Send(new OsVolumeChangedMessage(_deviceId, newVolume));
    }

    #endregion

    #region IDisposable

    // このオブジェクトが使用するリソースを解放します。
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // マネージドリソースおよびアンマネージドリソースを解放します。
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _subscription?.Dispose();
            _notificationSubject?.Dispose();
        }

        _isDisposed = true;
    }

    #endregion
}
