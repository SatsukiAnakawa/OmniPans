// Core/Services/UserSettings/IUserDevicePreferencesService.cs
// ユーザーによるデバイスごとの設定（音量、パン、表示状態）を管理するサービスのインターフェースです。
namespace OmniPans.Core.Services.UserSettings;

/// <summary>
/// ユーザーによるデバイスごとの設定（音量、パン、表示状態）を管理するサービスです。
/// </summary>
public interface IUserDevicePreferencesService
{
    /// <summary>
    /// 指定されたデバイスIDの設定をキャッシュから取得します。設定が存在しない場合は、ファクトリを使用して新しい設定を生成して返します。
    /// </summary>
    /// <param name="deviceId">設定を取得するデバイスのID。</param>
    /// <returns>デバイスの設定。</returns>
    DeviceSettings GetDeviceSettings(string deviceId);

    /// <summary>
    /// 指定されたデバイスIDの設定を現在のOSの状態と同期し、その最新の設定を取得します。
    /// </summary>
    /// <param name="deviceId">同期および取得するデバイスのID。</param>
    /// <returns>OSと同期された最新のデバイス設定。</returns>
    DeviceSettings GetSyncedSettings(string deviceId);

    /// <summary>
    /// 指定されたデバイスIDの設定を更新または追加します。
    /// </summary>
    /// <param name="deviceId">設定を更新するデバイスのID。</param>
    /// <param name="newSettings">新しい設定。</param>
    void UpdateDeviceSetting(string deviceId, DeviceSettings newSettings);

    /// <summary>
    /// デバイスのユーザーによる非表示設定を更新します。
    /// </summary>
    /// <param name="deviceId">設定を更新するデバイスのID。</param>
    /// <param name="isHidden">非表示にする場合は <c>true</c>、再表示する場合は <c>false</c>。</param>
    void SetUserHiddenPreference(string deviceId, bool isHidden);

    /// <summary>
    /// 非表示に設定されているすべてのデバイスIDのリストを取得します。
    /// </summary>
    /// <returns>非表示に設定されたデバイスIDのリスト。</returns>
    List<string> GetHiddenDeviceIds();

    /// <summary>
    /// 現在のすべての設定を永続的なストレージに非同期で保存します。
    /// </summary>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task SaveAllSettingsAsync();
}