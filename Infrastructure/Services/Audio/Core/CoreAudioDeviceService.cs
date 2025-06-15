// Infrastructure/Services/Audio/Core/CoreAudioDeviceService.cs
// NAudioライブラリをラップし、Core Audio APIへのアクセスを提供します。
namespace OmniPans.Infrastructure.Services.Audio;

/// <summary>
/// NAudioライブラリをラップし、Core Audio APIへのアクセスを提供する実装クラスです。
/// </summary>
[SupportedOSPlatform("windows")]
public class CoreAudioDeviceService : ICoreAudioDeviceService, IMMNotificationClient
{
    #region フィールド

    private readonly ILogger<CoreAudioDeviceService> _logger;
    private readonly MMDeviceEnumerator? _deviceEnumerator;
    private readonly bool _notificationCallbackRegistered = false;
    private bool _isDisposed;
    #endregion

    #region イベント

    /// <summary>
    /// 新しいオーディオデバイスがシステムに追加されたときに発生します。
    /// </summary>
    public event EventHandler<string>? DeviceAdded;

    /// <summary>
    /// オーディオデバイスがシステムから削除されたときに発生します。
    /// </summary>
    public event EventHandler<string>? DeviceRemoved;

    /// <summary>
    /// デバイスの状態（有効、無効など）が変更されたときに発生します。
    /// </summary>
    public event EventHandler<DeviceStateChangedArgs>? DeviceStateChanged;

    /// <summary>
    /// デフォルトのオーディオデバイスが変更されたときに発生します。
    /// </summary>
    public event EventHandler<DefaultDeviceChangedArgs>? DefaultDeviceChanged;

    /// <summary>
    /// デバイスのプロパティ（名前など）が変更されたときに発生します。
    /// </summary>
    public event EventHandler<DevicePropertyChangedArgs>? DevicePropertyChanged;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// <see cref="CoreAudioDeviceService"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="logger">ロギングサービス。</param>
    public CoreAudioDeviceService(ILogger<CoreAudioDeviceService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        try
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            _notificationCallbackRegistered = true;
            _logger.LogInformation("CoreAudioDeviceService 初期化、通知コールバック登録完了。");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MMDeviceEnumerator の初期化またはコールバック登録失敗。");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 現在アクティブな音声出力デバイスの一覧を取得します。
    /// </summary>
    /// <returns>アクティブな <see cref="MMDevice"/> のコレクション。</returns>
    public IEnumerable<MMDevice> GetActiveRenderDevices()
    {
        if (_deviceEnumerator is null)
        {
            _logger.LogWarning("GetActiveRenderDevices呼び出し時、MMDeviceEnumerator未初期化。");
            return Enumerable.Empty<MMDevice>();
        }
        try
        {
            var devices = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
            _logger.LogDebug("アクティブなレンダリングデバイス {DeviceCount} 件取得。", devices.Count);
            return devices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "アクティブなレンダリングデバイスの列挙中にエラー発生。");
            return Enumerable.Empty<MMDevice>();
        }
    }

    /// <summary>
    /// デバイスIDを指定して、対応する <see cref="MMDevice"/> オブジェクトを取得します。
    /// </summary>
    /// <param name="deviceId">取得するデバイスのID。</param>
    /// <returns>見つかった <see cref="MMDevice"/> オブジェクト。見つからない場合は null。</returns>
    public MMDevice? GetDeviceById(string deviceId)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        if (_deviceEnumerator is null)
        {
            _logger.LogWarning("GetDeviceById呼び出し時、MMDeviceEnumerator未初期化。");
            return null;
        }
        try
        {
            return _deviceEnumerator.GetDevice(deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ID '{DeviceId}' のデバイス取得中にエラー発生。", deviceId);
            return null;
        }
    }

    #endregion

    #region IMMNotificationClient Implementation

    void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState) => DeviceStateChanged?.Invoke(this, new(deviceId, newState));
    void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId) => DeviceAdded?.Invoke(this, pwstrDeviceId);
    void IMMNotificationClient.OnDeviceRemoved(string deviceId) => DeviceRemoved?.Invoke(this, deviceId);
    void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) => DefaultDeviceChanged?.Invoke(this, new(flow, role, defaultDeviceId));
    void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) => DevicePropertyChanged?.Invoke(this, new(pwstrDeviceId, key));

    #endregion

    #region IDisposable Implementation

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
            if (_deviceEnumerator is not null && _notificationCallbackRegistered)
            {
                _deviceEnumerator.UnregisterEndpointNotificationCallback(this);
            }
        }
        _isDisposed = true;
    }

    #endregion
}