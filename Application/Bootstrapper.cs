// Application/Bootstrapper.cs
// アプリケーションの起動プロセス、設定読み込み、DIコンテナの構築を担当します。
namespace OmniPans.Application;
public class Bootstrapper : IDisposable
{
    #region フィールド・プロパティ

    private ServiceProvider? _serviceProvider;
    private bool _isDisposed;
    public IServiceProvider? ServiceProvider => _serviceProvider;
    public IConfiguration? Configuration { get; private set; }

    #endregion

    #region Public Methods

    // アプリケーションの起動とサービスの設定を行います。
    public void Run()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
        Log.Information("OmniPans アプリケーション起動処理開始...");
        System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var hostWindowService = _serviceProvider.GetRequiredService<IApplicationHostWindowService>();
        hostWindowService.CreateAndSetMainWindow();

        InitializeGlobalServices();

        var appInitializer = _serviceProvider.GetRequiredService<IApplicationInitializer>();
        appInitializer.Initialize();
        Log.Information("OmniPans アプリケーション起動成功。");
    }

    #endregion

    #region Private Methods

    // 各種サービスをDIコンテナに登録します。
    private void ConfigureServices(IServiceCollection services)
    {
        if (Configuration is null)
        {
            throw new InvalidOperationException("Configuration is not initialized.");
        }

        // BehaviorConfigをDIコンテナに登録
        var behaviorConfig = new Core.Models.Configuration.BehaviorConfig();
        Configuration.GetSection("Behavior").Bind(behaviorConfig);
        services.AddSingleton(behaviorConfig);

        // ISystemClockをここで最初に登録する
        services.AddSingleton<Core.Services.Common.ISystemClock, Infrastructure.Services.Common.SystemClock>();

        services.AddSingleton(Configuration);
        services.AddAppLoggingServices();

        // 依存関係の土台から順に登録するように順番を整理
        services.AddAppUserSettingsServices(AppConstants.UserSettingsFileName);
        services.AddAppApplicationServices();
        services.AddAppAudioServices();
    }

    // グローバルに利用するサービスをAppインスタンスに設定します。
    private void InitializeGlobalServices()
    {
        if (System.Windows.Application.Current is App app && _serviceProvider is not null)
        {
            app.AppLogger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(AppConstants.MainLoggerName);
            app.UnhandledExceptionUIService = _serviceProvider.GetService<IUnhandledExceptionUIService>();
            app.AppLogger?.LogInformation("グローバルサービスをAppインスタンスに設定しました。");
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
            (_serviceProvider as IDisposable)?.Dispose();
        }

        _isDisposed = true;
    }

    #endregion
}