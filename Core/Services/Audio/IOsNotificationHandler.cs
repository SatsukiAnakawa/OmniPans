// Core/Services/Audio/IOsNotificationHandler.cs
// OSからの音量変更通知を処理するハンドラのインターフェースです。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// OSからの音量変更通知を処理するハンドラのインターフェースです。
/// </summary>
public interface IOsNotificationHandler : IDisposable
{
    /// <summary>
    /// OSからの音量変更通知を処理します。
    /// </summary>
    /// <param name="newVolumeScalar">OSから通知された新しい音量のスカラー値 (0.0-1.0)。</param>
    void HandleOsVolumeNotification(float newVolumeScalar);
}