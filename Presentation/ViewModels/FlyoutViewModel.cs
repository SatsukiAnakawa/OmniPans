// Presentation/ViewModels/FlyoutViewModel.cs
// フライアウトウィンドウのメインロジックと、表示されるデバイスVMのコレクションを管理します。
namespace OmniPans.Presentation.ViewModels;

/// <summary>
/// フライアウトウィンドウのメインロジックと、表示されるデバイスVMのコレクションを管理します。
/// </summary>
public partial class FlyoutViewModel : ObservableObject, IRecipient<HideDeviceRequestMessage>, IDisposable
{
    #region フィールド

    private readonly ILogger<FlyoutViewModel> _logger;
    private readonly IAudioDeviceMonitor _audioDeviceMonitor;
    private readonly IDeviceViewModelFactory _deviceViewModelFactory;
    private readonly IUserDevicePreferencesService _userDevicePreferencesService;
    private readonly IDispatcherService _dispatcherService;
    private bool _isDisposed;
    #endregion

    #region プロパティ

    /// <summary>
    /// フライアウトに表示されるデバイスのViewModelのコレクションを取得または設定します。
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<DeviceViewModel> _deviceViewModels = new();

    /// <summary>
    /// 表示可能なデバイスが1つ以上存在するかどうかを示す値を取得または設定します。
    /// </summary>
    [ObservableProperty]
    private bool _hasVisibleDevices;

    /// <summary>
    /// 表示可能なデバイスがない場合に表示するメッセージの可視性を取得します。
    /// </summary>
    public bool NoVisibleDevicesMessageIsVisible => !HasVisibleDevices;
    #endregion

    #region コンストラクタ

    /// <summary>
    /// <see cref="FlyoutViewModel"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    public FlyoutViewModel(
        ILogger<FlyoutViewModel> logger,
        IAudioDeviceMonitor audioDeviceMonitor,
        IUserDevicePreferencesService userDevicePreferencesService,
        IDeviceViewModelFactory deviceViewModelFactory,
        IDispatcherService dispatcherService)
    {
        _logger = logger;
        _audioDeviceMonitor = audioDeviceMonitor;
        _userDevicePreferencesService = userDevicePreferencesService;
        _deviceViewModelFactory = deviceViewModelFactory;
        _dispatcherService = dispatcherService;
        if (_audioDeviceMonitor.DisplayDevices is INotifyCollectionChanged notifyingCollection)
        {
            notifyingCollection.CollectionChanged += OnDisplayDevicesChanged;
        }

        WeakReferenceMessenger.Default.Register(this);
        UpdateDeviceViewModels(_audioDeviceMonitor.DisplayDevices);
    }

    #endregion

    #region Private Methods

    // HasVisibleDevicesプロパティ変更時に連動するプロパティの変更を通知します。
    partial void OnHasVisibleDevicesChanged(bool value) => OnPropertyChanged(nameof(NoVisibleDevicesMessageIsVisible));

    /// <summary>
    /// <see cref="HideDeviceRequestMessage"/> を受信してデバイスを非表示にします。
    /// </summary>
    /// <param name="message">受信したメッセージ。</param>
    public void Receive(HideDeviceRequestMessage message)
    {
        var deviceVmToHide = DeviceViewModels.FirstOrDefault(vm => vm.Id == message.DeviceId);
        if (deviceVmToHide is not null)
        {
            HideDeviceRequested(deviceVmToHide);
        }
    }

    // 表示デバイスコレクションの変更をUIスレッドで処理します。
    private void OnDisplayDevicesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _dispatcherService.Invoke(() =>
        {
            foreach (var vm in DeviceViewModels)
            {
                vm.Dispose();
            }

            UpdateDeviceViewModels(_audioDeviceMonitor.DisplayDevices);
        });
    }

    // 表示デバイスのリストからViewModelのコレクションを更新します。
    private void UpdateDeviceViewModels(ReadOnlyObservableCollection<IDisplayedDevice> displayDevices)
    {
        var newDeviceVms = displayDevices
            .Select(device => _deviceViewModelFactory.Create(device))
            .ToList();
        DeviceViewModels = new ObservableCollection<DeviceViewModel>(newDeviceVms);
        HasVisibleDevices = newDeviceVms.Any();
    }

    // 指定されたデバイスの非表示要求を処理します。
    private void HideDeviceRequested(DeviceViewModel deviceViewModel)
    {
        _logger.LogInformation("デバイス '{DeviceName}' (ID: {DeviceId}) の非表示リクエストを処理します。", deviceViewModel.FriendlyName, deviceViewModel.Id);
        _userDevicePreferencesService.SetUserHiddenPreference(deviceViewModel.Id, true);
        _audioDeviceMonitor.RefreshDeviceList();
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// このViewModelが使用するリソースを解放します。
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
            if (_audioDeviceMonitor.DisplayDevices is INotifyCollectionChanged notifyingCollection)
            {
                notifyingCollection.CollectionChanged -= OnDisplayDevicesChanged;
            }

            WeakReferenceMessenger.Default.UnregisterAll(this);
            foreach (var vm in DeviceViewModels)
            {
                vm.Dispose();
            }
        }

        _isDisposed = true;
    }

    #endregion
}