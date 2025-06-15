// Application/App.xaml.cs
// アプリケーションのエントリーポイント。ライフサイクルイベントとグローバルな例外処理を管理します。
namespace OmniPans;
public partial class App : System.Windows.Application
{
    #region フィールド・プロパティ

    private Bootstrapper? _bootstrapper;

    internal Microsoft.Extensions.Logging.ILogger? AppLogger { get; set; }
    internal IUnhandledExceptionUIService? UnhandledExceptionUIService { get; set; }

    #endregion

    #region アプリケーションライフサイクル

    // アプリケーションの起動時に実行される処理です。
    protected override void OnStartup(StartupEventArgs e)
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        base.OnStartup(e);

        try
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "ブートストラッパーの初期化中に致命的なエラーが発生しました。");
            MessageBox.Show(
                $"アプリケーションの起動に必要なコンポーネントを初期化できませんでした。\nエラー: {ex.Message}",
                "致命的な起動エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    // アプリケーションの終了時に実行される処理です。
    protected override void OnExit(ExitEventArgs e)
    {
        AppLogger?.LogInformation("アプリケーション終了処理開始...");
        this.DispatcherUnhandledException -= App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        AppLogger?.LogInformation("グローバル例外ハンドラ登録解除。");

        _bootstrapper?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    #endregion

    #region 例外ハンドラ

    // UIスレッドで発生した未処理の例外を処理します。
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        UnhandledExceptionUIService?.HandleUiThreadException(e.Exception);
        e.Handled = true;
    }

    // バックグラウンドスレッドで発生した未処理の例外を処理します。
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        const string message = "ハンドルされていない重大なエラーが発生しました (非UIスread)。";
        if (e.ExceptionObject is Exception ex)
        {
            AppLogger?.LogCritical(ex, "{Message} IsTerminating: {IsTerminating}", message, e.IsTerminating);
        }
        else
        {
            AppLogger?.LogCritical("{Message} ExceptionObject: {ExceptionObject}, IsTerminating: {IsTerminating}", message, e.ExceptionObject, e.IsTerminating);
        }

        try
        {
            Log.CloseAndFlush();
        }
        catch (Exception flushEx)
        {
            AppLogger?.LogError(flushEx, "Serilogのフラッシュ失敗 (CurrentDomain_UnhandledException)。");
        }
    }

    #endregion
}