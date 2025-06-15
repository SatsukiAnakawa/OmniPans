// Core/Services/Audio/IDisplayedDeviceProvider.cs
// 表示されているデバイスを取得する機能を提供します。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// UIに現在表示されているデバイスのインスタンスを取得する機能を提供します。
/// </summary>
public interface IDisplayedDeviceProvider
{
    /// <summary>
    /// IDを指定して表示中のデバイスを取得します。
    /// </summary>
    /// <param name="deviceId">取得したいデバイスのID。</param>
    /// <returns>見つかった <see cref="IDisplayedDevice"/> インスタンス。見つからない場合は null。</returns>
    IDisplayedDevice? GetById(string deviceId);
}