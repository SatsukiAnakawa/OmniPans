// Infrastructure/Services/Audio/AudioEndpointController.cs
// オーディオデバイスの音量やパンを実際に制御します。
namespace OmniPans.Infrastructure.Services.Audio;

/// <summary>
/// オーディオデバイスの音量やパンを実際に制御する実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
/// <param name="displayedDeviceProvider">表示中のデバイスを取得するサービス。</param>
public class AudioEndpointController(
    ILogger<AudioEndpointController> logger,
    IDisplayedDeviceProvider displayedDeviceProvider) : IAudioEndpointController
{
    /// <summary>
    /// 指定されたデバイスの音量とパンを非同期で適用します。
    /// </summary>
    /// <param name="deviceId">対象のデバイスID。</param>
    /// <param name="targetUIVolume">適用するUI上の音量値 (0-100)。</param>
    /// <param name="targetPan">適用するパン値 (-100-100)。</param>
    /// <param name="cancellationToken">操作をキャンセルするためのトークン。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public Task ApplyVolumeAsync(string deviceId, double targetUIVolume, double targetPan, CancellationToken cancellationToken = default)
    {
        var device = displayedDeviceProvider.GetById(deviceId);
        if (device is null) return Task.CompletedTask;

        float masterVolumeScalar = Math.Clamp((float)targetUIVolume / 100.0f, 0.0f, 1.0f);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (device.CanPan)
            {
                var (leftPanScalar, rightPanScalar) = PanCalculator.ConvertPanToScalars((float)targetPan);
                float finalLeftScalar = masterVolumeScalar * leftPanScalar;
                float finalRightScalar = masterVolumeScalar * rightPanScalar;
                device.SetStereoVolumeScalars(finalLeftScalar, finalRightScalar);
            }
            else
            {
                device.SetMonoVolumeScalar(masterVolumeScalar);
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogDebug(ex, "'{DeviceName}' への音量適用タスクはキャンセルされました。", device.FriendlyName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "'{DeviceName}' の音量適用(パン考慮)中にエラー発生。", device.FriendlyName);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 指定されたデバイスのパンのみを現在の音量を維持したまま非同期で適用します。
    /// </summary>
    /// <param name="deviceId">対象のデバイスID。</param>
    /// <param name="targetPan">適用するパン値 (-100-100)。</param>
    /// <param name="cancellationToken">操作をキャンセルするためのトークン。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public Task ApplyPanAsync(string deviceId, double targetPan, CancellationToken cancellationToken = default)
    {
        var device = displayedDeviceProvider.GetById(deviceId);
        if (device is null || !device.CanPan) return Task.CompletedTask;

        double currentVolume = device.GetMasterVolumeScalar() * 100.0;
        return ApplyVolumeAsync(deviceId, currentVolume, targetPan, cancellationToken);
    }
}