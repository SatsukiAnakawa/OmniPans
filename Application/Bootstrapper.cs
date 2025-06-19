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
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.AppName);
        string logFilePath = Path.Combine(appDataPath, "logs", "omnipans_log_.txt");

        var readerOptions = new ConfigurationReaderOptions();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration, readerOptions)
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
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

        var behaviorConfig = new Core.Models.Configuration.BehaviorConfig();
        Configuration.GetSection("Behavior").Bind(behaviorConfig);
        services.AddSingleton(behaviorConfig);

        services.AddSingleton<Core.Services.Common.ISystemClock, Infrastructure.Services.Common.SystemClock>();

        services.AddSingleton(Configuration);
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddAppLoggingServices();
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.AppName);
        string userSettingsPath = Path.Combine(appDataPath, AppConstants.UserSettingsFileName);
        try
        {
            Directory.CreateDirectory(appDataPath);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "AppDataフォルダの作成に失敗しました。");
        }

        services.AddAppUserSettingsServices(userSettingsPath);
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
