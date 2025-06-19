// Presentation/ViewModels/DeviceViewModel.cs
// 単一のオーディオデバイスに関するUIロジックと状態を管理します。
namespace OmniPans.Presentation.ViewModels;

public partial class DeviceViewModel : ObservableObject, IDisposable,
    IRecipient<OsVolumeChangedMessage>,
    IRecipient<OsPanChangedMessage>
{
    #region フィールド

    private readonly IAudioEndpointController _audioEndpointController;
    private readonly IUserDevicePreferencesService _userDevicePreferencesService;
    private readonly IUserInteractionTracker _userInteractionTracker;
    private readonly IMessenger _messenger;
    private readonly ILogger<DeviceViewModel> _logger;
    private double _volumeBeforeMute;
    // ミュート前の音量を記憶
    private bool _isUpdatingFromOs;
    private bool _isTogglingPanReset; // アイコンクリックによるリセット操作中かどうかのフラグ
    private bool _isDisposed;

    #endregion

    #region プロパティ

    [ObservableProperty]
    private IDisplayedDevice _device;

    [ObservableProperty]
    private double _volume;

    [ObservableProperty]
    private double _pan;

    public string Id => Device.Id;
    public string FriendlyName => Device.FriendlyName;
    public bool IsPanControlAvailable => Device.CanPan;

    #endregion

    #region コンストラクタ

    // DeviceViewModel クラスの新しいインスタンスを初期化します。
    public DeviceViewModel(
        IDisplayedDevice device,
        IAudioEndpointController audioEndpointController,
        IUserDevicePreferencesService userDevicePreferencesService,
        IUserInteractionTracker userInteractionTracker,
        IMessenger messenger,
        ILogger<DeviceViewModel> logger)
    {
        _device = device;
        _audioEndpointController = audioEndpointController;
        _userDevicePreferencesService = userDevicePreferencesService;
        _userInteractionTracker = userInteractionTracker;
        _messenger = messenger;
        _logger = logger;

        var settings = _userDevicePreferencesService.GetSyncedSettings(device.Id);
        _volume = settings.Volume;
        _pan = settings.Pan;

        _volumeBeforeMute = Math.Abs(_volume) > double.Epsilon ? _volume : Core.Models.DeviceSettings.DefaultVolume;

        _messenger.Register<OsVolumeChangedMessage>(this);
        _messenger.Register<OsPanChangedMessage>(this);
    }

    #endregion

    #region メッセージ受信

    // OSからの音量変更通知メッセージを受信し、UIの状態を更新します。
    public void Receive(OsVolumeChangedMessage message)
    {
        if (_isDisposed || message.DeviceId != Id) return;
        SyncStateFromOS(message.NewVolume, Pan);
    }

    // OSからのパン変更通知メッセージを受信し、UIの状態を更新します。
    public void Receive(OsPanChangedMessage message)
    {
        if (_isDisposed || message.DeviceId != Id) return;
        SyncStateFromOS(Volume, message.NewPan);
    }

    #endregion

    #region コマンド

    // このデバイスをUI上から非表示にするよう要求します。
    [RelayCommand]
    private void HideDevice()
    {
        _messenger.Send(new HideDeviceRequestMessage(Id));
    }

    // 音量のミュート状態を切り替えます。
    [RelayCommand]
    private void ToggleMute()
    {
        if (Math.Abs(Volume) > double.Epsilon)
        {
            Volume = 0;
        }
        else
        {
            Volume = _volumeBeforeMute;
        }
    }

    // パンの値をリセット（0に設定）または元の値に戻します。
    [RelayCommand]
    private async Task TogglePanResetAsync()
    {
        if (_isTogglingPanReset) return;

        try
        {
            _isTogglingPanReset = true;
            var currentSettings = _userDevicePreferencesService.GetDeviceSettings(Id);

            if (Math.Abs(Pan) > double.Epsilon)
            {
                var newSettings = currentSettings with { Pan = 0, PanBeforeReset = this.Pan };
                _userDevicePreferencesService.UpdateDeviceSetting(Id, newSettings);
                await _audioEndpointController.ApplyPanAsync(Id, 0, default);
                this.Pan = 0;
            }
            else
            {
                var panToRestore = currentSettings.PanBeforeReset;
                if (Math.Abs(panToRestore) > double.Epsilon)
                {
                    _userDevicePreferencesService.UpdateDeviceSetting(Id, currentSettings with { Pan = panToRestore });
                    await _audioEndpointController.ApplyPanAsync(Id, panToRestore, default);
                    this.Pan = panToRestore;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "デバイス '{DeviceName}' (ID: {DeviceId}) のパンのリセット操作中にエラーが発生しました。", FriendlyName, Id);
        }
        finally
        {
            _isTogglingPanReset = false;
        }
    }

    // UIからの音量変更をデバイスに適用し、設定を保存します。
    [RelayCommand]
    private async Task ApplyVolumeChangeAsync(CancellationToken cancellationToken)
    {
        var roundedVolume = (int)Math.Round(Volume);
        var currentSettings = _userDevicePreferencesService.GetDeviceSettings(Id);
        if (roundedVolume == (int)Math.Round(currentSettings.Volume)) return;

        _userInteractionTracker.RecordUserInteraction(Id);
        await _audioEndpointController.ApplyVolumeAsync(Id, roundedVolume, Pan, cancellationToken).ConfigureAwait(false);
        _userDevicePreferencesService.UpdateDeviceSetting(Id, currentSettings with { Volume = roundedVolume });
    }

    // UIからのパン変更をデバイスに適用し、設定を保存します。
    [RelayCommand]
    private async Task ApplyPanChangeAsync(CancellationToken cancellationToken)
    {
        var roundedPan = (int)Math.Round(Pan);
        var currentSettings = _userDevicePreferencesService.GetDeviceSettings(Id);

        _userInteractionTracker.RecordUserInteraction(Id);
        await _audioEndpointController.ApplyPanAsync(Id, roundedPan, cancellationToken).ConfigureAwait(false);
        _userDevicePreferencesService.UpdateDeviceSetting(Id, currentSettings with { Pan = roundedPan, PanBeforeReset = roundedPan });
    }

    #endregion

    #region プロパティ変更ハンドラ

    // 音量プロパティの変更を検知し、適用コマンドを実行します。
    partial void OnVolumeChanged(double value)
    {
        if (Math.Abs(value) > double.Epsilon)
        {
            _volumeBeforeMute = value;
        }

        if (!_isUpdatingFromOs)
        {
            if (ApplyVolumeChangeCommand.IsRunning)
            {
                ApplyVolumeChangeCommand.Cancel();
            }
            ApplyVolumeChangeCommand.Execute(null);
        }
    }

    // パンプロパティの変更を検知し、適用コマンドを実行します。
    partial void OnPanChanged(double value)
    {
        if (_isTogglingPanReset) return;
        if (!_isUpdatingFromOs)
        {
            if (ApplyPanChangeCommand.IsRunning)
            {
                ApplyPanChangeCommand.Cancel();
            }
            ApplyPanChangeCommand.Execute(null);
        }
    }

    #endregion

    #region 公開メソッド

    // OSから読み取った最新の状態でViewModelを安全に更新します。
    public void SyncStateFromOS(double osVolume, double osPan)
    {
        try
        {
            _isUpdatingFromOs = true;
            Volume = osVolume;

            if (IsPanControlAvailable)
            {
                Pan = osPan;
            }
        }
        finally
        {
            _isUpdatingFromOs = false;
        }
    }

    #endregion

    #region IDisposable

    // このViewModelが使用するリソースを解放します。
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
            if (ApplyVolumeChangeCommand.IsRunning) ApplyVolumeChangeCommand.Cancel();
            if (ApplyPanChangeCommand.IsRunning) ApplyPanChangeCommand.Cancel();
            _messenger.UnregisterAll(this);
        }
        _isDisposed = true;
    }

    #endregion
}
