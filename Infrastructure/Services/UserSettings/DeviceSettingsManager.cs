// Infrastructure/Services/UserSettings/DeviceSettingsManager.cs
// JSONファイルとしてデバイス設定の読み書きを行います。
namespace OmniPans.Infrastructure.Services.UserSettings;

/// <summary>
/// JSONファイルとしてデバイス設定の読み書きを行う実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
/// <param name="settingsFilePath">設定ファイルの完全パス。</param>
public class DeviceSettingsManager(
    ILogger<DeviceSettingsManager> logger,
    string settingsFilePath) : IDeviceSettingsManager
{
    /// <summary>
    /// 設定ファイルから全デバイスの設定を読み込みます。
    /// </summary>
    /// <returns>デバイスIDをキー、<see cref="DeviceSettings"/> を値とするディクショナリ。</returns>
    public Dictionary<string, DeviceSettings> LoadSettings()
    {
        if (!File.Exists(settingsFilePath))
        {
            logger.LogWarning("設定ファイルが見つかりません: {FilePath}。空の設定を返します。", settingsFilePath);
            return [];
        }

        try
        {
            string jsonString = File.ReadAllText(settingsFilePath);
            var settings = JsonSerializer.Deserialize<Dictionary<string, DeviceSettings>>(jsonString);
            return settings ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "設定ファイル {FilePath} の読み込みまたは解析中にエラー発生。", settingsFilePath);
            return [];
        }
    }

    /// <summary>
    /// 指定された設定を設定ファイルに非同期で保存します。
    /// </summary>
    /// <param name="settings">保存するデバイス設定のディクショナリ。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public async Task SaveSettingsAsync(Dictionary<string, DeviceSettings>? settings)
    {
        if (settings is null) return;
        try
        {
            string? directoryName = Path.GetDirectoryName(settingsFilePath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync(settingsFilePath, jsonString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "設定ファイル {FilePath} への保存中にエラー発生。", settingsFilePath);
        }
    }
}