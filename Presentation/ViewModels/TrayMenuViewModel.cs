// Presentation/ViewModels/TrayMenuViewModel.cs
// トレイアイコンのコンテキストメニューのロジックと状態を管理します。
namespace OmniPans.Presentation.ViewModels;

/// <summary>
/// タスクトレイアイコンのコンテキストメニューのロジックと状態を管理します。
/// </summary>
[SupportedOSPlatform("windows")]
public partial class TrayMenuViewModel(
    IStartupService startupService,
    IUserDevicePreferencesService userDevicePreferencesService,
    IAudioDeviceMonitor audioDeviceMonitor,
    IApplicationLifecycleService applicationLifecycleService,
    IDeviceFilter deviceFilter) : ObservableObject, ITrayMenuViewModel
{
    #region プロパティ

    /// <summary>
    /// アプリケーションがWindowsのスタートアップに登録されているかどうかを示す値を取得または設定します。
    /// </summary>
    [ObservableProperty]
    private bool _isStartupEnabled;

    /// <summary>
    /// 非表示に設定されているデバイスを表すメニュー項目のコレクションを取得または設定します。
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<IMenuItemViewModel> _hiddenDeviceMenuItems = [];

    /// <summary>
    /// 非表示のデバイスメニュー項目が1つ以上存在するかどうかを示す値を取得または設定します。
    /// </summary>
    [ObservableProperty]
    private bool _hasHiddenDeviceItems;

    #endregion

    #region コマンド

    /// <summary>
    /// アプリケーションのスタートアップ登録状態を切り替えます。
    /// </summary>
    [RelayCommand]
    private void ToggleStartup(bool isEnabled)
    {
        startupService.SetEnabled(isEnabled);
    }

    /// <summary>
    /// 指定されたデバイスを再表示（非表示設定を解除）します。
    /// </summary>
    [RelayCommand]
    private void UnhideDevice(string? deviceId)
    {
        if (string.IsNullOrEmpty(deviceId)) return;
        userDevicePreferencesService.SetUserHiddenPreference(deviceId, false);
        audioDeviceMonitor.RefreshDeviceList();
    }

    /// <summary>
    /// アプリケーションを非同期で終了します。
    /// </summary>
    [RelayCommand]
    private async Task ExitApplicationAsync()
    {
        await applicationLifecycleService.ExitApplicationAsync();
    }

    #endregion

    #region publicメソッド

    /// <summary>
    /// コンテキストメニューが開かれる直前にメニュー項目を最新の状態に更新します。
    /// </summary>
    public void LoadMenuItems()
    {
        IsStartupEnabled = startupService.IsEnabled();
        var hiddenDevices = deviceFilter.GetHiddenDeviceInfos().ToList();

        var menuItems = new List<IMenuItemViewModel>();
        if (hiddenDevices.Any())
        {
            menuItems.Add(new HeaderMenuItemViewModel("クリックでデバイスを再表示:"));
            menuItems.AddRange(hiddenDevices.Select(d => new HiddenDeviceMenuItemViewModel(d.Id, d.FriendlyName)));
        }

        HiddenDeviceMenuItems = new ObservableCollection<IMenuItemViewModel>(menuItems);
        HasHiddenDeviceItems = hiddenDevices.Any();
    }

    #endregion
}