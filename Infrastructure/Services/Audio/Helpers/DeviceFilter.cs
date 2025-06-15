// Infrastructure/Services/Audio/Helpers/DeviceFilter.cs
// ユーザー設定に基づいて表示すべきオーディオデバイスをフィルタリングします。
namespace OmniPans.Infrastructure.Services.Audio;

/// <summary>
/// ユーザー設定に基づいて表示すべきオーディオデバイスをフィルタリングする実装クラスです。
/// </summary>
/// <param name="userDevicePreferencesService">ユーザー設定を管理するサービス。</param>
/// <param name="nameCache">デバイス名キャッシュサービス。</param>
public class DeviceFilter(
    IUserDevicePreferencesService userDevicePreferencesService,
    IDeviceFriendlyNameCache nameCache) : IDeviceFilter
{
    /// <summary>
    /// 指定されたデバイスリストから、ユーザー設定に基づきUIに表示すべきデバイスのみを抽出します。
    /// </summary>
    /// <param name="allActiveDevices">フィルタリング対象となる全てのアクティブなデバイスのコレクション。</param>
    /// <returns>表示すべきデバイスのコレクション。</returns>
    public IEnumerable<MMDevice> GetDisplayableDevices(IEnumerable<MMDevice> allActiveDevices)
    {
        foreach (var device in allActiveDevices)
        {
            if (!userDevicePreferencesService.GetDeviceSettings(device.ID).IsUserHidden)
            {
                yield return device;
            }
            else
            {
                device.Dispose();
            }
        }
    }

    /// <summary>
    /// ユーザーによって非表示に設定されているデバイスの情報を取得します。
    /// </summary>
    /// <returns>非表示デバイスのIDとフレンドリー名のタプルのコレクション。</returns>
    public IEnumerable<(string Id, string FriendlyName)> GetHiddenDeviceInfos()
    {
        var hiddenDeviceIds = userDevicePreferencesService.GetHiddenDeviceIds();
        var hiddenDevices = new List<(string Id, string FriendlyName)>();

        foreach (var deviceId in hiddenDeviceIds)
        {
            hiddenDevices.Add((deviceId, nameCache.GetFriendlyName(deviceId)));
        }
        return hiddenDevices;
    }
}