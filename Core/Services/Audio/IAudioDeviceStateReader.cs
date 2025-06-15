// Core/Services/Audio/IAudioDeviceStateReader.cs
// オーディオデバイスの現在の状態（音量、パン）をOSから直接読み取る機能を提供します。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// オーディオデバイスの現在の状態（音量、パン）をOSから直接読み取る機能を提供します。
/// </summary>
public interface IAudioDeviceStateReader
{
    /// <summary>
    /// 指定されたデバイスの現在の音量とパンをOSから読み取ります。
    /// </summary>
    /// <param name="deviceId">状態を読み取るデバイスのID。</param>
    /// <returns>成功した場合は音量(0-100)とパン(-100-100)のタプル、失敗した場合は null。</returns>
    (double Volume, double Pan)? ReadCurrentState(string deviceId);
}