// Core/Services/Audio/IDeviceFriendlyNameCache.cs
// オーディオデバイスのフレンドリ名をキャッシュし、効率的な名前解決を提供します。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// オーディオデバイスのフレンドリ名をキャッシュし、効率的な名前解決を提供します。
/// </summary>
public interface IDeviceFriendlyNameCache
{
    /// <summary>
    /// 指定された <see cref="MMDevice"/> オブジェクトからフレンドリー名を取得し、キャッシュします。
    /// </summary>
    /// <param name="device">名前を取得するデバイスオブジェクト。</param>
    /// <returns>デバイスのフレンドリー名。</returns>
    string GetFriendlyName(MMDevice device);

    /// <summary>
    /// 指定されたデバイスIDからフレンドリー名を取得します。キャッシュが存在すればそれを返し、なければデバイスを取得して名前を解決・キャッシュします。
    /// </summary>
    /// <param name="deviceId">名前を取得するデバイスのID。</param>
    /// <returns>デバイスのフレンドリー名。</returns>
    string GetFriendlyName(string deviceId);

    /// <summary>
    /// 指定されたデバイスIDのキャッシュを無効化します。
    /// </summary>
    /// <param name="deviceId">キャッシュを無効化するデバイスのID。</param>
    void Invalidate(string deviceId);
}