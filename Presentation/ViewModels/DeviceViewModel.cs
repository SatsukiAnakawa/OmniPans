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
    private bool _isUpdatingFromOs;
    private bool _isDisposed;
    #endregion

    #region プロパティ

    /// <summary>
    /// このViewModelが表現するデバイスのモデルを取得します。
    /// </summary>
    [ObservableProperty]
    private IDisplayedDevice _device;

    /// <summary>
    /// デバイスの現在の音量 (0-100) を取得または設定します。
    /// UIからの変更は、デバウンス処理された後、非同期でOSに適用されます。
    /// </summary>
    [ObservableProperty]
    private double _volume;

    /// <summary>
    /// デバイスの現在のパン（左右バランス, -100-100）を取得または設定します。
    /// UIからの変更は、デバウンス処理された後、非同期でOSに適用されます。
    /// </summary>
    [ObservableProperty]
    private double _pan;

    /// <summary>
    /// デバイスの一意なIDを取得します。
    /// </summary>
    public string Id => Device.Id;

    /// <summary>
    /// デバイスの表示名を取得します。
    /// </summary>
    public string FriendlyName => Device.FriendlyName;

    /// <summary>
    /// デバイスがパンコントロールをサポートしているかどうかを示す値を取得します。
    /// </summary>
    public bool IsPanControlAvailable => Device.CanPan;
    #endregion

    #region コンストラクタ

    /// <summary>
    /// <see cref="DeviceViewModel"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="device">関連付けるデバイスモデル。</param>
    /// <param name="audioEndpointController">オーディオ設定を適用するためのコントローラー。</param>
    /// <param name="userDevicePreferencesService">ユーザー設定を管理するサービス。</param>
    /// <param name="userInteractionTracker">ユーザー操作を追跡するサービス。</param>
    public DeviceViewModel(
        IDisplayedDevice device,
        IAudioEndpointController audioEndpointController,
        IUserDevicePreferencesService userDevicePreferencesService,
        IUserInteractionTracker userInteractionTracker)
    {
        _device = device;
        _audioEndpointController = audioEndpointController;
        _userDevicePreferencesService = userDevicePreferencesService;
        _userInteractionTracker = userInteractionTracker;

        // OSの最新設定と同期してから値を取得する
        var settings = _userDevicePreferencesService.GetSyncedSettings(device.Id);
        _volume = settings.Volume;
        _pan = settings.Pan;

        WeakReferenceMessenger.Default.Register<OsVolumeChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<OsPanChangedMessage>(this);
    }

    #endregion

    #region メッセージ受信

    /// <summary>
    /// OSからの音量変更通知メッセージを受信し、UIの状態を更新します。
    /// </summary>
    /// <param name="message">受信したメッセージ。</param>
    public void Receive(OsVolumeChangedMessage message)
    {
        if (_isDisposed || message.DeviceId != Id) return;
        SyncStateFromOS(message.NewVolume, Pan);
    }

    /// <summary>
    /// OSからのパン変更通知メッセージを受信し、UIの状態を更新します。
    /// </summary>
    /// <param name="message">受信したメッセージ。</param>
    public void Receive(OsPanChangedMessage message)
    {
        if (_isDisposed || message.DeviceId != Id) return;
        SyncStateFromOS(Volume, message.NewPan);
    }

    #endregion

    #region コマンド

    /// <summary>
    /// このデバイスをUI上から非表示にするよう要求します。
    /// </summary>
    [RelayCommand]
    private void HideDevice()
    {
        WeakReferenceMessenger.Default.Send(new HideDeviceRequestMessage(Id));
    }

    /// <summary>
    /// UIからの音量変更をデバイスに適用し、設定を保存します。
    /// </summary>
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

    /// <summary>
    /// UIからのパン変更をデバイスに適用し、設定を保存します。
    /// </summary>
    [RelayCommand]
    private async Task ApplyPanChangeAsync(CancellationToken cancellationToken)
    {
        var roundedPan = (int)Math.Round(Pan);
        var currentSettings = _userDevicePreferencesService.GetDeviceSettings(Id);
        if (roundedPan == (int)Math.Round(currentSettings.Pan)) return;

        _userInteractionTracker.RecordUserInteraction(Id);
        await _audioEndpointController.ApplyPanAsync(Id, roundedPan, cancellationToken).ConfigureAwait(false);
        _userDevicePreferencesService.UpdateDeviceSetting(Id, currentSettings with { Pan = roundedPan });
    }

    #endregion

    #region プロパティ変更ハンドラ

    // 音量プロパティの変更を検知し、適用コマンドを実行します。
    partial void OnVolumeChanged(double value)
    {
        if (_isUpdatingFromOs) return;
        if (ApplyVolumeChangeCommand.IsRunning)
        {
            ApplyVolumeChangeCommand.Cancel();
        }
        ApplyVolumeChangeCommand.Execute(null);
    }

    // パンプロパティの変更を検知し、適用コマンドを実行します。
    partial void OnPanChanged(double value)
    {
        if (_isUpdatingFromOs) return;
        if (ApplyPanChangeCommand.IsRunning)
        {
            ApplyPanChangeCommand.Cancel();
        }
        ApplyPanChangeCommand.Execute(null);
    }

    #endregion

    #region 公開メソッド

    /// <summary>
    /// OSから読み取った最新の状態でViewModelを安全に更新します。
    /// </summary>
    /// <param name="osVolume">OSから取得した最新の音量。</param>
    /// <param name="osPan">OSから取得した最新のパン。</param>
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
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
        _isDisposed = true;
    }

    #endregion
}