// Infrastructure/Services/Factories/DeviceSettingsFactory.cs
// 新しいデバイスのデフォルト設定を、現在のOSの状態に基づいて生成するファクトリの実装です。
namespace OmniPans.Infrastructure.Services.Factories;

/// <summary>
/// 新しいデバイスのデフォルト設定を、現在のOSの状態に基づいて生成するファクトリの実装です。
/// </summary>
/// <param name="audioDeviceStateReader">OSからデバイス状態を読み取るサービス。</param>
public class DeviceSettingsFactory(IAudioDeviceStateReader audioDeviceStateReader) : IDeviceSettingsFactory
{
    /// <summary>
    /// 指定されたデバイスIDのデフォルト設定を、現在のOSの音量とパンを読み取って生成します。
    /// </summary>
    /// <param name="deviceId">デバイスID。</param>
    /// <returns>生成された <see cref="DeviceSettings"/>。</returns>
    public DeviceSettings CreateDefaultSettings(string deviceId)
    {
        var currentState = audioDeviceStateReader.ReadCurrentState(deviceId);
        double currentVolume = currentState?.Volume ?? DeviceSettings.DefaultVolume;
        double currentPan = currentState?.Pan ?? DeviceSettings.DefaultPan;

        return new DeviceSettings
        {
            Volume = Math.Clamp(currentVolume, DeviceSettings.MinVolume, DeviceSettings.MaxVolume),
            Pan = Math.Clamp(currentPan, DeviceSettings.MinPan, DeviceSettings.MaxPan),
            IsUserHidden = false
        };
    }
}