// Infrastructure/Services/Application/StartupService.cs
// Windowsのスタートアップ設定（レジストリ）を管理するサービスです。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// Windowsのスタートアップ設定（レジストリ）を管理するサービスの実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
[SupportedOSPlatform("windows")]
public class StartupService(ILogger<StartupService> logger) : IStartupService
{
    private readonly RegistryKey? _runKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    /// <summary>
    /// アプリケーションがスタートアップに登録されているか確認します。
    /// </summary>
    /// <returns>登録されている場合は <c>true</c>、それ以外は <c>false</c>。</returns>
    public bool IsEnabled()
    {
        if (_runKey is null) return false;
        return _runKey.GetValue(AppConstants.AppName) is not null;
    }

    /// <summary>
    /// アプリケーションのスタートアップ登録状態を設定します。
    /// </summary>
    /// <param name="isEnabled">スタートアップに登録する場合は <c>true</c>、登録を解除する場合は <c>false</c>。</param>
    public void SetEnabled(bool isEnabled)
    {
        if (_runKey is null)
        {
            logger.LogError("レジストリキーを開けませんでした。スタートアップ設定を変更できません。");
            return;
        }

        try
        {
            if (isEnabled)
            {
                string? exePath = Environment.ProcessPath;
                if (string.IsNullOrEmpty(exePath))
                {
                    logger.LogError("実行ファイルのパスを取得できませんでした。");
                    return;
                }
                _runKey.SetValue(AppConstants.AppName, $"\"{exePath}\"");
                logger.LogInformation("スタートアップに登録しました。");
            }
            else
            {
                _runKey.DeleteValue(AppConstants.AppName, false);
                logger.LogInformation("スタートアップから登録解除しました。");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "スタートアップ設定の変更中にエラーが発生しました。");
        }
    }
}