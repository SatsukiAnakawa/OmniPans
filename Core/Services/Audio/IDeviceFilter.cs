// Core/Services/Audio/IDeviceFilter.cs
// 表示すべきオーディオデバイスをフィルタリングする機能を定義します。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// 表示すべきオーディオデバイスをフィルタリングする機能を提供します。
/// </summary>
public interface IDeviceFilter
{
    /// <summary>
    /// 指定されたデバイスリストから、ユーザー設定に基づきUIに表示すべきデバイスのみを抽出します。
    /// </summary>
    /// <param name="allActiveDevices">フィルタリング対象となる全てのアクティブなデバイスのコレクション。</param>
    /// <returns>表示すべきデバイスのコレクション。</returns>
    IEnumerable<MMDevice> GetDisplayableDevices(IEnumerable<MMDevice> allActiveDevices);

    /// <summary>
    /// ユーザーによって非表示に設定されているデバイスの情報を取得します。
    /// </summary>
    /// <returns>非表示デバイスのIDとフレンドリー名のタプルのコレクション。</returns>
    IEnumerable<(string Id, string FriendlyName)> GetHiddenDeviceInfos();
}