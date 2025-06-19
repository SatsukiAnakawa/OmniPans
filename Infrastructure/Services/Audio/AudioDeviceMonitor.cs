// Infrastructure/Services/Audio/AudioDeviceMonitor.cs
// オーディオデバイスの接続状態や変更を監視し、UI表示用デバイスリストを管理します。
namespace OmniPans.Infrastructure.Services.Audio;
[SupportedOSPlatform("windows")]
public class AudioDeviceMonitor : IAudioDeviceMonitor
{
    #region フィールド

    private readonly ILogger<AudioDeviceMonitor> _logger;
    private readonly ICoreAudioDeviceService _coreAudioDeviceService;
    private readonly IDispatcherService _dispatcherService;
    private readonly IDeviceFilter _deviceFilter;
    private readonly IManagedDeviceFactory _managedDeviceFactory;
    private readonly IDeviceFriendlyNameCache _nameCache;
    private readonly PanChangeNotifier _panChangeNotifier;
    private readonly ObservableCollection<IDisplayedDevice> _managedDevices = [];
    private bool _isDisposed;
    #endregion

    #region プロパティ

    public ReadOnlyObservableCollection<IDisplayedDevice> DisplayDevices { get; }

    #endregion

    #region コンストラクタ

    public AudioDeviceMonitor(
        ILogger<AudioDeviceMonitor> logger,
        ICoreAudioDeviceService coreAudioDeviceService,
        IDispatcherService dispatcherService,
        IDeviceFilter deviceFilter,
        IManagedDeviceFactory managedDeviceFactory,
        IDeviceFriendlyNameCache nameCache,
        PanChangeNotifier panChangeNotifier)
    {
        _logger = logger;
        _coreAudioDeviceService = coreAudioDeviceService;
        _dispatcherService = dispatcherService;
        _deviceFilter = deviceFilter;
        _managedDeviceFactory = managedDeviceFactory;
        _nameCache = nameCache;
        _panChangeNotifier = panChangeNotifier;
        DisplayDevices = new ReadOnlyObservableCollection<IDisplayedDevice>(_managedDevices);

        _coreAudioDeviceService.DeviceAdded += OnCoreDeviceAdded;
        _coreAudioDeviceService.DeviceRemoved += OnCoreDeviceRemoved;
        _coreAudioDeviceService.DeviceStateChanged += OnCoreDeviceStateChanged;
        _coreAudioDeviceService.DefaultDeviceChanged += OnCoreDefaultDeviceChanged;

        _dispatcherService.BeginInvoke(RefreshDeviceListInternal);
    }

    #endregion

    #region Public Methods

    // 最新のデバイスリストに更新します。
    public void RefreshDeviceList() => _dispatcherService.BeginInvoke(RefreshDeviceListInternal);
    // デバイスIDに基づいて、管理下のデバイスのフレンドリー名を取得します。
    public string? GetDeviceFriendlyNameById(string deviceId) => _managedDevices.FirstOrDefault(d => d.Id == deviceId)?.FriendlyName;
    #endregion

    #region Private Device Management Methods

    // システムに存在するデバイスに合わせて管理下のデバイスリストを更新します。
    private void RefreshDeviceListInternal()
    {
        try
        {
            _logger.LogDebug("デバイスリストの更新処理を開始...");
            List<MMDevice> currentSystemDevices;
            try
            {
                currentSystemDevices = _coreAudioDeviceService.GetActiveRenderDevices().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "アクティブなレンダリングデバイスの取得中にエラーが発生しました。");
                return;
            }

            var displayableDevices = _deviceFilter.GetDisplayableDevices(currentSystemDevices).ToList();
            var displayableDeviceIds = displayableDevices.Select(d => d.ID).ToHashSet();
            var managedDeviceIds = _managedDevices.Select(d => d.Id).ToHashSet();

            var deviceIdsToRemove = managedDeviceIds.Except(displayableDeviceIds).ToList();
            foreach (var deviceId in deviceIdsToRemove)
            {
                RemoveDeviceInternal(deviceId);
            }

            foreach (var device in displayableDevices)
            {
                if (!managedDeviceIds.Contains(device.ID))
                {
                    AddDeviceInternal(device);
                }
                else
                {
                    device.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "デバイスリストの更新処理中に予期せぬエラーが発生しました。");
        }
    }

    // デバイスIDからMMDeviceを取得し、管理下に追加します。
    private void AddDeviceInternal(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId) || _managedDevices.Any(d => d.Id == deviceId)) return;
        var mmDevice = _coreAudioDeviceService.GetDeviceById(deviceId);
        if (mmDevice is not null)
        {
            if (_deviceFilter.GetDisplayableDevices(new[] { mmDevice }).Any())
            {
                AddDeviceInternal(mmDevice);
            }
            else
            {
                mmDevice.Dispose();
            }
        }
    }

    // MMDeviceをラップして管理下に追加し、通知ハンドラを関連付けます。
    private void AddDeviceInternal(MMDevice device)
    {
        var displayDevice = _managedDeviceFactory.Create(device);
        _managedDevices.Add(displayDevice);
        _logger.LogInformation("デバイス '{FriendlyName}' をリストに追加しました。", displayDevice.FriendlyName);
    }

    // 指定されたデバイスを管理下から削除し、リソースを解放します。
    private void RemoveDeviceInternal(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId)) return;
        var deviceToRemove = _managedDevices.FirstOrDefault(d => d.Id == deviceId);
        if (deviceToRemove is not null)
        {
            _nameCache.Invalidate(deviceId);
            _logger.LogInformation("デバイス '{FriendlyName}' をリストから削除します。", deviceToRemove.FriendlyName);

            _managedDevices.Remove(deviceToRemove);
            _managedDeviceFactory.TearDown(deviceToRemove);
        }
    }

    // 全ての管理下デバイスをクリアします。
    private void ClearAllManagedDevices()
    {
        var deviceIds = _managedDevices.Select(d => d.Id).ToList();
        foreach (var deviceId in deviceIds)
        {
            RemoveDeviceInternal(deviceId);
        }

        if (_managedDevices.Any())
        {
            _logger.LogWarning("ClearAllManagedDevices後も管理下のデバイスが残っています。");
            _managedDevices.Clear();
        }
    }

    #endregion

    #region Event Handlers for CoreAudioDeviceService

    private void OnCoreDeviceAdded(object? sender, string deviceId) => _dispatcherService.BeginInvoke(() => AddDeviceInternal(deviceId));
    private void OnCoreDeviceRemoved(object? sender, string deviceId) => _dispatcherService.BeginInvoke(() => RemoveDeviceInternal(deviceId));

    private void OnCoreDeviceStateChanged(object? sender, DeviceStateChangedArgs args) => _dispatcherService.BeginInvoke(RefreshDeviceListInternal);
    private void OnCoreDefaultDeviceChanged(object? sender, DefaultDeviceChangedArgs args) => _dispatcherService.BeginInvoke(RefreshDeviceListInternal);

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (_coreAudioDeviceService is not null)
                {
                    _coreAudioDeviceService.DeviceAdded -= OnCoreDeviceAdded;
                    _coreAudioDeviceService.DeviceRemoved -= OnCoreDeviceRemoved;
                    _coreAudioDeviceService.DeviceStateChanged -= OnCoreDeviceStateChanged;
                    _coreAudioDeviceService.DefaultDeviceChanged -= OnCoreDefaultDeviceChanged;
                }
                _dispatcherService.Invoke(ClearAllManagedDevices);
                _panChangeNotifier?.Dispose();
            }
            _isDisposed = true;
        }
    }

    #endregion
}
