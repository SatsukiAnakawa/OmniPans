// Core/Services/UserSettings/IDeviceSettingsManager.cs
// デバイス設定の読み込みと保存の永続化処理を定義します。
namespace OmniPans.Core.Services.UserSettings;

/// <summary>
/// デバイス設定の読み込みと保存（永続化）の機能を提供します。
/// </summary>
public interface IDeviceSettingsManager
{
    /// <summary>
    /// 永続化ストレージ（ファイルなど）から全デバイスの設定を読み込みます。
    /// </summary>
    /// <returns>デバイスIDをキー、<see cref="DeviceSettings"/> を値とするディクショナリ。</returns>
    Dictionary<string, DeviceSettings> LoadSettings();

    /// <summary>
    /// 指定された設定を永続化ストレージに非同期で保存します。
    /// </summary>
    /// <param name="settings">保存するデバイス設定のディクショナリ。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task SaveSettingsAsync(Dictionary<string, DeviceSettings>? settings);
}