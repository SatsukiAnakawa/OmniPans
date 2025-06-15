// Infrastructure/Services/Audio/Core/DisplayedDevice.cs
// MMDeviceをラップし、UI表示用のデバイス情報と基本的な操作を提供します。
namespace OmniPans.Infrastructure.Services.Factories;

public class DisplayedDevice : IDisplayedDevice
{
    #region フィールド

    private readonly MMDevice _mmDevice;
    private readonly ILogger<DisplayedDevice> _logger;
    private IOsNotificationHandler? _attachedHandler;
    private bool _isDisposed;

    #endregion

    #region プロパティ・イベント

    public event OsVolumeNotificationDelegate? OsVolumeNotificationReceived;
    public string Id => _mmDevice.ID;
    public string FriendlyName { get; }
    public bool CanPan => _mmDevice.AudioEndpointVolume?.Channels.Count >= 2;
    #endregion

    #region コンストラクタ

    public DisplayedDevice(MMDevice mmDevice, string friendlyName, ILogger<DisplayedDevice> logger)
    {
        ArgumentNullException.ThrowIfNull(mmDevice);
        ArgumentNullException.ThrowIfNull(logger);

        _mmDevice = mmDevice;
        _logger = logger;
        FriendlyName = friendlyName;
        try
        {
            if (_mmDevice.AudioEndpointVolume is not null)
            {
                _mmDevice.AudioEndpointVolume.OnVolumeNotification += OnNaudioVolumeNotification;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "デバイス {DeviceId} のイベント購読中にエラーが発生しました。", mmDevice.ID);
        }
    }

    #endregion

    #region Public Methods for internal use

    /// <summary>
    /// このデバイスインスタンスに通知ハンドラをアタッチします。
    /// </summary>
    /// <param name="handler">アタッチする通知ハンドラ。</param>
    internal void AttachNotificationHandler(IOsNotificationHandler handler)
    {
        _attachedHandler = handler;
    }

    #endregion

    #region IDisplayedDevice Implementation

    // 現在のマスター音量を0.0から1.0の範囲のスカラー値で取得します。
    public float GetMasterVolumeScalar()
    {
        try
        {
            return _mmDevice.AudioEndpointVolume?.MasterVolumeLevelScalar ?? 0.0f;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "デバイス {DeviceId} のマスター音量取得に失敗しました。", Id);
            return 0.0f;
        }
    }

    // 左右チャンネルの音量を個別のスカラー値で設定します。
    public void SetStereoVolumeScalars(float leftScalar, float rightScalar)
    {
        var channels = _mmDevice.AudioEndpointVolume?.Channels;
        if (channels is not { Count: >= 2 }) return;
        try
        {
            channels[0].VolumeLevelScalar = leftScalar;
            channels[1].VolumeLevelScalar = rightScalar;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "デバイス {DeviceId} のステレオ音量設定に失敗しました。", Id);
        }
    }

    // モノラルデバイスの音量を単一のスカラー値で設定します。
    public void SetMonoVolumeScalar(float masterScalar)
    {
        var channels = _mmDevice.AudioEndpointVolume?.Channels;
        if (channels is not { Count: 1 }) return;

        try
        {
            channels[0].VolumeLevelScalar = masterScalar;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "デバイス {DeviceId} のモノラル音量設定に失敗しました。", Id);
        }
    }

    #endregion

    #region Private Methods

    // NAudioからのネイティブな音量通知をハンドルします。
    private void OnNaudioVolumeNotification(AudioVolumeNotificationData data)
    {
        OsVolumeNotificationReceived?.Invoke(data.MasterVolume);
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            if (_mmDevice.AudioEndpointVolume is not null)
            {
                _mmDevice.AudioEndpointVolume.OnVolumeNotification -= OnNaudioVolumeNotification;
            }

            // アタッチされたハンドラのイベント購読を解除し、破棄する
            if (_attachedHandler is not null)
            {
                this.OsVolumeNotificationReceived -= _attachedHandler.HandleOsVolumeNotification;
                _attachedHandler.Dispose();
                _attachedHandler = null;
            }

            // 責務として、常に内部のMMDeviceを破棄する
            _mmDevice.Dispose();
        }
        _isDisposed = true;
    }

    #endregion
}