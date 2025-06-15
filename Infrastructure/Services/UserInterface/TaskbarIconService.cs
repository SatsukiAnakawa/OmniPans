// Infrastructure/Services/UserInterface/TaskbarIconService.cs
// タスクバーアイコンの表示とインタラクションを管理します。
namespace OmniPans.Infrastructure.Services.UserInterface;

using System.Windows.Controls;

/// <summary>
/// タスクバーアイコンの表示とインタラクションを管理する実装クラスです。
/// </summary>
[SupportedOSPlatform("windows")]
public class TaskbarIconService(
    ILogger<TaskbarIconService> logger,
    IFlyoutService flyoutService,
    IServiceProvider serviceProvider) : ITaskbarIconService
{
    #region フィールド

    private TaskbarIcon? _taskbarIcon;
    private ITrayMenuViewModel? _trayMenuViewModel;
    private bool _isDisposed;

    #endregion

    #region Public Methods

    /// <summary>
    /// タスクバーアイコンを初期化し、表示します。
    /// </summary>
    public void Initialize()
    {
        _trayMenuViewModel = serviceProvider.GetRequiredService<ITrayMenuViewModel>();
        var contextMenu = (ContextMenu)System.Windows.Application.Current.FindResource(AppConstants.TrayContextMenuResourceKey);

        _taskbarIcon = new TaskbarIcon
        {
            IconSource = new BitmapImage(new Uri(AppConstants.AppIconUri)),
            ToolTipText = AppConstants.AppName,
            DataContext = _trayMenuViewModel,
            ContextMenu = contextMenu,
            MenuActivation = PopupActivationMode.RightClick
        };
        _taskbarIcon.TrayLeftMouseDown += OnTrayLeftMouseDown;
        _taskbarIcon.TrayContextMenuOpen += OnTrayContextMenuOpen;

        logger.LogInformation("TaskbarIconService 初期化、TaskbarIcon設定完了。");
    }

    #endregion

    #region Private Methods

    // タスクバーアイコンの左クリックイベントを処理し、フライアウトを開きます。
    private void OnTrayLeftMouseDown(object sender, RoutedEventArgs e)
    {
        flyoutService.ToggleFlyoutWindow();
    }

    // コンテキストメニューが開かれる直前のイベントで、メニュー項目を最新の状態に更新します。
    private void OnTrayContextMenuOpen(object sender, RoutedEventArgs e)
    {
        _trayMenuViewModel?.LoadMenuItems();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// このサービスが使用するリソースを解放します。
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
        if (_isDisposed)
        {
            return;
        }

        if (disposing && _taskbarIcon is not null)
        {
            _taskbarIcon.TrayLeftMouseDown -= OnTrayLeftMouseDown;
            _taskbarIcon.TrayContextMenuOpen -= OnTrayContextMenuOpen;
            _taskbarIcon.Dispose();
            _taskbarIcon = null;
            logger.LogInformation("TaskbarIconService破棄、TaskbarIconリソース解放完了。");
        }

        _isDisposed = true;
    }

    #endregion
}