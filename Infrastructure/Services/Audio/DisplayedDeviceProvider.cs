// Infrastructure/Services/Audio/DisplayedDeviceProvider.cs
// 表示されているデバイスを取得する機能を提供します。
namespace OmniPans.Infrastructure.Services.Audio;

/// <summary>
/// UIに現在表示されているデバイスのインスタンスを取得する機能の実装クラスです。
/// </summary>
/// <param name="audioDeviceMonitor">オーディオデバイスを監視するサービス。</param>
public class DisplayedDeviceProvider(IAudioDeviceMonitor audioDeviceMonitor) : IDisplayedDeviceProvider
{
    /// <summary>
    /// IDを指定して表示中のデバイスを取得します。
    /// </summary>
    /// <param name="deviceId">取得したいデバイスのID。</param>
    /// <returns>見つかった <see cref="IDisplayedDevice"/> インスタンス。見つからない場合は null。</returns>
    public IDisplayedDevice? GetById(string deviceId)
    {
        return audioDeviceMonitor.DisplayDevices.FirstOrDefault(d => d.Id == deviceId);
    }
}