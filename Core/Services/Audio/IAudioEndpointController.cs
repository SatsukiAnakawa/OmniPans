// Core/Services/Audio/IAudioEndpointController.cs
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// オーディオデバイスの音量やパン設定を実際に適用する機能を提供します。
/// </summary>
public interface IAudioEndpointController
{
    /// <summary>
    /// 指定されたデバイスの音量とパンを非同期で適用します。
    /// </summary>
    /// <param name="deviceId">対象のデバイスID。</param>
    /// <param name="targetUIVolume">適用するUI上の音量値 (0-100)。</param>
    /// <param name="targetPan">適用するパン値 (-100-100)。</param>
    /// <param name="cancellationToken">操作をキャンセルするためのトークン。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task ApplyVolumeAsync(string deviceId, double targetUIVolume, double targetPan, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定されたデバイスのパンのみを現在の音量を維持したまま非同期で適用します。
    /// </summary>
    /// <param name="deviceId">対象のデバイスID。</param>
    /// <param name="targetPan">適用するパン値 (-100-100)。</param>
    /// <param name="cancellationToken">操作をキャンセルするためのトークン。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task ApplyPanAsync(string deviceId, double targetPan, CancellationToken cancellationToken = default);
}