// Infrastructure/Services/Audio/AudioDeviceStateReader.cs
// IAudioDeviceStateReaderの具象実装。CoreAudio APIを使用してデバイスの状態を読み取ります。
namespace OmniPans.Infrastructure.Services.Audio;

/// <summary>
/// <see cref="IAudioDeviceStateReader"/> の実装クラス。CoreAudio APIを使用してデバイスの状態を読み取ります。
/// </summary>
/// <param name="coreAudioDeviceService">Core Audioデバイス操作サービス。</param>
/// <param name="logger">ロギングサービス。</param>
/// <param name="nameCache">デバイス名キャッシュサービス。</param>
[SupportedOSPlatform("windows")]
public class AudioDeviceStateReader(
    ICoreAudioDeviceService coreAudioDeviceService,
    ILogger<AudioDeviceStateReader> logger,
    IDeviceFriendlyNameCache nameCache) : IAudioDeviceStateReader
{
    /// <summary>
    /// 指定されたデバイスの現在の音量とパンをOSから読み取ります。
    /// </summary>
    /// <param name="deviceId">状態を読み取るデバイスのID。</param>
    /// <returns>成功した場合は音量(0-100)とパン(-100-100)のタプル、失敗した場合は null。</returns>
    public (double Volume, double Pan)? ReadCurrentState(string deviceId)
    {
        using var device = coreAudioDeviceService.GetDeviceById(deviceId);
        if (device is null)
        {
            logger.LogWarning("デバイスID {DeviceId} の状態読み取りに失敗しました。デバイスが見つかりません。", deviceId);
            return null;
        }

        string friendlyName = nameCache.GetFriendlyName(device);

        var endpointVolume = device.AudioEndpointVolume;
        if (endpointVolume is null)
        {
            logger.LogWarning("'{FriendlyName}' の AudioEndpointVolume が nullでした。", friendlyName);
            return (DeviceSettings.DefaultVolume, DeviceSettings.DefaultPan);
        }

        try
        {
            double osVolume = endpointVolume.MasterVolumeLevelScalar * 100.0;
            double osPan = DeviceSettings.DefaultPan;

            var channels = endpointVolume.Channels;
            if (channels?.Count >= 2)
            {
                osPan = PanCalculator.ConvertScalarsToPan(channels[0].VolumeLevelScalar, channels[1].VolumeLevelScalar);
            }

            return (Math.Round(osVolume), Math.Round(osPan));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "'{FriendlyName}' の現在のOS設定読み取り中にエラー発生。", friendlyName);
            return null;
        }
    }
}