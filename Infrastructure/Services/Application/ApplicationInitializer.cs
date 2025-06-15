// Infrastructure/Services/Application/ApplicationInitializer.cs
// アプリケーションの主要なサービスを初期化する実装クラスです。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// アプリケーションの主要なサービスを初期化する実装クラスです。
/// </summary>
[SupportedOSPlatform("windows")]
public class ApplicationInitializer(
    ILogger<ApplicationInitializer> logger,
    ITaskbarIconService taskbarIconService,
    ICriticalErrorHandler criticalErrorHandler) : IApplicationInitializer
{
    #region Public Methods

    /// <summary>
    /// アプリケーションのサービスを初期化します。
    /// </summary>
    public void Initialize()
    {
        try
        {
            logger.LogInformation("TaskbarIconServiceの初期化を開始します。");
            taskbarIconService.Initialize();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "TaskbarIconServiceの初期化中に致命的なエラーが発生しました。");
            criticalErrorHandler.Handle(ex, "タスクバーアイコンの初期化に失敗しました。");
        }
    }

    #endregion
}