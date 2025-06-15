// Infrastructure/Services/Audio/Helpers/DeviceFriendlyNameCache.cs
// IDeviceFriendlyNameCacheの具象実装。デバイス名をメモリ内の辞書にキャッシュします。
namespace OmniPans.Infrastructure.Services.Audio;

using System.Collections.Concurrent;

/// <summary>
/// <see cref="IDeviceFriendlyNameCache"/> の実装クラス。デバイス名をメモリ内のスレッドセーフな辞書にキャッシュします。
/// </summary>
/// <param name="coreAudioDeviceService">Core Audioデバイス操作サービス。</param>
/// <param name="logger">ロギングサービス。</param>
[SupportedOSPlatform("windows")]
public class DeviceFriendlyNameCache(
    ICoreAudioDeviceService coreAudioDeviceService,
    ILogger<DeviceFriendlyNameCache> logger) : IDeviceFriendlyNameCache
{
    private readonly ConcurrentDictionary<string, string> _nameCache = new();

    /// <summary>
    /// 指定された <see cref="MMDevice"/> オブジェクトからフレンドリー名を取得し、キャッシュします。
    /// </summary>
    /// <param name="device">名前を取得するデバイスオブジェクト。</param>
    /// <returns>デバイスのフレンドリー名。</returns>
    public string GetFriendlyName(MMDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (_nameCache.TryGetValue(device.ID, out var cachedName))
        {
            return cachedName;
        }

        try
        {
            var friendlyName = device.FriendlyName;
            _nameCache[device.ID] = friendlyName;
            return friendlyName;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "デバイスID {DeviceId} のFriendlyName取得に失敗しました。", device.ID);
            var fallbackName = $"不明なデバイス({device.ID.AsSpan(0, 4)}..)";
            _nameCache[device.ID] = fallbackName;
            return fallbackName;
        }
    }

    /// <summary>
    /// 指定されたデバイスIDからフレンドリー名を取得します。キャッシュが存在すればそれを返し、なければデバイスを取得して名前を解決・キャッシュします。
    /// </summary>
    /// <param name="deviceId">名前を取得するデバイスのID。</param>
    /// <returns>デバイスのフレンドリー名。</returns>
    public string GetFriendlyName(string deviceId)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        if (_nameCache.TryGetValue(deviceId, out var cachedName))
        {
            return cachedName;
        }

        using var device = coreAudioDeviceService.GetDeviceById(deviceId);
        if (device is null)
        {
            logger.LogWarning("デバイスID {DeviceId} の名前取得に失敗しました。デバイスが見つかりません。", deviceId);
            var fallbackName = $"不明なデバイス({deviceId.AsSpan(0, 4)}..)";
            _nameCache[deviceId] = fallbackName;
            return fallbackName;
        }

        return GetFriendlyName(device);
    }

    /// <summary>
    /// 指定されたデバイスIDのキャッシュを無効化します。
    /// </summary>
    /// <param name="deviceId">キャッシュを無効化するデバイスのID。</param>
    public void Invalidate(string deviceId)
    {
        _nameCache.TryRemove(deviceId, out _);
    }
}