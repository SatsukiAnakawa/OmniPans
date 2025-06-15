// Infrastructure/Services/Application/ApplicationLifecycleService.cs
// アプリケーションのライフサイクルイベントを処理します。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// アプリケーションのライフサイクルイベントを処理する実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
/// <param name="userDevicePreferencesService">ユーザー設定を管理するサービス。</param>
public class ApplicationLifecycleService(
    ILogger<ApplicationLifecycleService> logger,
    IUserDevicePreferencesService userDevicePreferencesService) : IApplicationLifecycleService
{
    /// <summary>
    /// アプリケーションを非同期で終了します。終了前に現在の設定を保存します。
    /// </summary>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public async Task ExitApplicationAsync()
    {
        logger.LogInformation("ExitApplication呼び出し。設定保存とシャットダウンを試みます。");
        try
        {
            await userDevicePreferencesService.SaveAllSettingsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "アプリケーション終了時の設定保存中にエラー発生。");
        }

        System.Windows.Application.Current?.Shutdown();
    }
}