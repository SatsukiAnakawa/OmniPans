// Presentation/InteractionHandlers/OsNotificationHandler.cs
// OSからの音量変更通知をハンドリングし、UI更新のためのメッセージを送信します。
namespace OmniPans.Presentation.InteractionHandlers;

public class OsNotificationHandler : IOsNotificationHandler
{
    #region フィールド

    private readonly string _deviceId;
    private readonly IUserInteractionTracker _userInteractionTracker;
    private readonly IDisposable _subscription;
    private readonly Subject<float> _notificationSubject = new();
    private readonly int _gracePeriodMs;
    private bool _isDisposed;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// <see cref="OsNotificationHandler"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="deviceId">このハンドラーが対象とするデバイスのID。</param>
    /// <param name="dispatcherService">UIスレッド操作サービス。</param>
    /// <param name="userInteractionTracker">ユーザー操作追跡サービス。</param>
    /// <param name="debounceTimeMs">デバウンス時間（ミリ秒）。</param>
    /// <param name="gracePeriodMs">ユーザー操作の猶予期間（ミリ秒）。</param>
    public OsNotificationHandler(
        string deviceId,
        IDispatcherService dispatcherService,
        IUserInteractionTracker userInteractionTracker,
        int debounceTimeMs,
        int gracePeriodMs)
    {
        _deviceId = deviceId;
        _userInteractionTracker = userInteractionTracker;
        _gracePeriodMs = gracePeriodMs;

        _subscription = _notificationSubject
            .Throttle(TimeSpan.FromMilliseconds(debounceTimeMs))
            .ObserveOn(new DispatcherScheduler(dispatcherService.Dispatcher))
            .Subscribe(ProcessDebouncedNotification);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// OSからの音量変更通知を処理します。ユーザー自身の操作直後の通知は無視します。
    /// </summary>
    /// <param name="newVolumeScalar">OSから通知された新しい音量のスカラー値 (0.0-1.0)。</param>
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
        WeakReferenceMessenger.Default.Send(new OsVolumeChangedMessage(_deviceId, newVolume));
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// このオブジェクトが使用するリソースを解放します。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// マネージドリソースおよびアンマネージドリソースを解放します。
    /// </summary>
    /// <param name="disposing">マネージドリソースを解放する場合は <c>true</c>、それ以外は <c>false</c>。</param>
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