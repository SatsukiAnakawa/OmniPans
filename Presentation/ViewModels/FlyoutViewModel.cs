// Presentation/ViewModels/FlyoutViewModel.cs
// フライアウトウィンドウのメインロジックと、表示されるデバイスVMのコレクションを管理します。
namespace OmniPans.Presentation.ViewModels;

public partial class FlyoutViewModel : ObservableObject, IRecipient<HideDeviceRequestMessage>, IDisposable
{
    #region フィールド

    private readonly ILogger<FlyoutViewModel> _logger;
    private readonly IAudioDeviceMonitor _audioDeviceMonitor;
    private readonly IDeviceViewModelFactory _deviceViewModelFactory;
    private readonly IUserDevicePreferencesService _userDevicePreferencesService;
    private readonly IDispatcherService _dispatcherService;
    private readonly IMessenger _messenger;
    private bool _isDisposed;

    #endregion

    #region プロパティ

    [ObservableProperty]
    private ObservableCollection<DeviceViewModel> _deviceViewModels = new();

    [ObservableProperty]
    private bool _hasVisibleDevices;

    public bool NoVisibleDevicesMessageIsVisible => !HasVisibleDevices;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// FlyoutViewModel クラスの新しいインスタンスを初期化します。
    /// </summary>
    public FlyoutViewModel(
        ILogger<FlyoutViewModel> logger,
        IAudioDeviceMonitor audioDeviceMonitor,
        IUserDevicePreferencesService userDevicePreferencesService,
        IDeviceViewModelFactory deviceViewModelFactory,
        IDispatcherService dispatcherService,
        IMessenger messenger)
    {
        _logger = logger;
        _audioDeviceMonitor = audioDeviceMonitor;
        _userDevicePreferencesService = userDevicePreferencesService;
        _deviceViewModelFactory = deviceViewModelFactory;
        _dispatcherService = dispatcherService;
        _messenger = messenger;

        if (_audioDeviceMonitor.DisplayDevices is INotifyCollectionChanged notifyingCollection)
        {
            notifyingCollection.CollectionChanged += OnDisplayDevicesChanged;
        }

        _messenger.Register(this);
        UpdateDeviceViewModels(_audioDeviceMonitor.DisplayDevices);
    }

    #endregion

    #region メッセージ受信 (IMessenger)

    /// <summary>
    /// 指定されたデバイスを非表示にする要求メッセージを受信して処理します。
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

    #endregion

    #region コレクション変更ハンドラ

    /// <summary>
    /// 表示対象デバイスのコレクション変更を検知し、ViewModelコレクションを効率的に同期します。
    /// </summary>
    private void OnDisplayDevicesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _dispatcherService.Invoke(() =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null)
                    {
                        foreach (IDisplayedDevice newDevice in e.NewItems)
                        {
                            var newVm = _deviceViewModelFactory.Create(newDevice);
                            DeviceViewModels.Insert(e.NewStartingIndex, newVm);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        foreach (IDisplayedDevice oldDevice in e.OldItems)
                        {
                            var vmToRemove = DeviceViewModels.FirstOrDefault(vm => vm.Id == oldDevice.Id);
                            if (vmToRemove is not null)
                            {
                                vmToRemove.Dispose();
                                DeviceViewModels.Remove(vmToRemove);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var vm in DeviceViewModels)
                    {
                        vm.Dispose();
                    }
                    DeviceViewModels.Clear();
                    UpdateDeviceViewModels(_audioDeviceMonitor.DisplayDevices);
                    break;
            }

            HasVisibleDevices = DeviceViewModels.Any();
        });
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// HasVisibleDevicesプロパティ変更時に、関連プロパティの変更を通知します。
    /// </summary>
    partial void OnHasVisibleDevicesChanged(bool value) => OnPropertyChanged(nameof(NoVisibleDevicesMessageIsVisible));

    /// <summary>
    /// 表示デバイスのリストからViewModelのコレクションを再構築します。
    /// </summary>
    /// <param name="displayDevices">再構築の基となるデバイスのコレクション。</param>
    private void UpdateDeviceViewModels(ReadOnlyObservableCollection<IDisplayedDevice> displayDevices)
    {
        var newDeviceVms = displayDevices
            .Select(device => _deviceViewModelFactory.Create(device))
            .ToList();

        DeviceViewModels = new ObservableCollection<DeviceViewModel>(newDeviceVms);
        HasVisibleDevices = newDeviceVms.Any();
    }

    /// <summary>
    /// 指定されたデバイスの非表示要求を処理します。
    /// </summary>
    /// <param name="deviceViewModel">非表示にするデバイスのViewModel。</param>
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
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            if (_audioDeviceMonitor.DisplayDevices is INotifyCollectionChanged notifyingCollection)
            {
                notifyingCollection.CollectionChanged -= OnDisplayDevicesChanged;
            }

            _messenger.UnregisterAll(this);

            foreach (var vm in DeviceViewModels)
            {
                vm.Dispose();
            }
        }

        _isDisposed = true;
    }

    #endregion
}
