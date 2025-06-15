// Core/Services/Audio/IAudioDeviceMonitor.cs
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// オーディオデバイスの接続状態や変更を監視し、UIに表示すべきデバイスのコレクションを管理するサービスです。
/// </summary>
public interface IAudioDeviceMonitor : IDisposable
{
    /// <summary>
    /// UIに表示対象となるオーディオデバイスの読み取り専用コレクション。
    /// </summary>
    ReadOnlyObservableCollection<IDisplayedDevice> DisplayDevices { get; }

    /// <summary>
    /// 最新のデバイスリストに強制的に更新します。
    /// </summary>
    void RefreshDeviceList();

    /// <summary>
    /// デバイスIDに基づいて、管理下のデバイスのフレンドリー名を取得します。
    /// </summary>
    /// <param name="deviceId">フレンドリー名を取得したいデバイスのID。</param>
    /// <returns>デバイスのフレンドリー名。見つからない場合は null。</returns>
    string? GetDeviceFriendlyNameById(string deviceId);
}