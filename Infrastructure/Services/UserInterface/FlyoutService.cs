// Infrastructure/Services/UserInterface/FlyoutService.cs
// UIのフライアウトウィンドウの表示・非表示とライフサイクルを管理します。
namespace OmniPans.Infrastructure.Services.UserInterface;
/// <summary>
/// UIのフライアウトウィンドウの表示・非表示とライフサイクルを管理する実装クラスです。
/// </summary>
[SupportedOSPlatform("windows")]
public class FlyoutService(
    ILogger<FlyoutService> logger,
    Func<FlyoutViewModel> flyoutViewModelFactory,
    IWindowPositioner windowPositioner) : IFlyoutService
{
    #region フィールド

    private FlyoutWindow? _flyoutWindow;
    private double? _currentFlyoutLeft;
    private bool _isDisposed;

    #endregion

    #region Public Methods

    /// <summary>
    /// フライアウトウィンドウの表示状態を切り替えます。表示されていなければ表示し、表示されていればアクティブにします。
    /// </summary>
    public void ToggleFlyoutWindow()
    {
        if (_isDisposed)
        {
            logger.LogWarning("破棄されたFlyoutServiceが呼び出されました。");
            return;
        }

        if (_flyoutWindow is not null && _flyoutWindow.IsVisible)
        {
            _flyoutWindow.Activate();
            return;
        }

        try
        {
            var viewModel = flyoutViewModelFactory();
            _flyoutWindow = new FlyoutWindow { DataContext = viewModel };

            _flyoutWindow.MaxHeight = SystemParameters.WorkArea.Height - 10;

            _flyoutWindow.Closed += OnFlyoutWindow_Closed;
            _flyoutWindow.SizeChanged += OnFlyoutWindow_SizeChanged;

            _flyoutWindow.Opacity = 0;
            _flyoutWindow.Show();
            _flyoutWindow.UpdateLayout();
            _currentFlyoutLeft = windowPositioner.PositionFlyout(_flyoutWindow);

            _flyoutWindow.Opacity = 1;
            _flyoutWindow.Activate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FlyoutWindowの作成または表示中にエラーが発生しました。");
            CleanupCurrentWindow();
        }
    }

    #endregion

    #region Private Methods & Event Handlers

    // ウィンドウが閉じた時のクリーンアップ処理を呼び出します。
    private void OnFlyoutWindow_Closed(object? sender, EventArgs e)
    {
        logger.LogInformation("FlyoutWindowが閉じられました。リソースをクリーンアップします。");
        CleanupCurrentWindow();
    }

    // ウィンドウのサイズが変更された時にY座標を再計算します。
    private void OnFlyoutWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_flyoutWindow is null || !_flyoutWindow.IsVisible || _currentFlyoutLeft is null || !e.HeightChanged)
        {
            return;
        }
        windowPositioner.RepositionFlyoutY(_flyoutWindow, _currentFlyoutLeft.Value);
    }

    // 現在表示中のウィンドウリソースを解放します。
    private void CleanupCurrentWindow()
    {
        if (_flyoutWindow is null) return;
        _flyoutWindow.Closed -= OnFlyoutWindow_Closed;
        _flyoutWindow.SizeChanged -= OnFlyoutWindow_SizeChanged;

        if (_flyoutWindow.DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        _flyoutWindow = null;
        _currentFlyoutLeft = null;
    }

    #endregion

    #region IDisposable

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
        if (_isDisposed) return;
        if (disposing)
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                _flyoutWindow?.Close();
                CleanupCurrentWindow();
            });
            logger.LogInformation("FlyoutService の破棄処理が完了しました。");
        }
        _isDisposed = true;
    }

    #endregion
}
