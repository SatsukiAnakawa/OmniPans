// Core/Services/Factories/IManagedDeviceFactory.cs
// 管理対象となるデバイスオブジェクトを生成・破棄するファクトリを定義します。
namespace OmniPans.Core.Services.Factories;

/// <summary>
/// 管理対象となるデバイスオブジェクト（<see cref="IDisplayedDevice"/>）を生成・破棄するファクトリを定義します。
/// </summary>
public interface IManagedDeviceFactory
{
    /// <summary>
    /// <see cref="MMDevice"/> から、関連オブジェクトを含む <see cref="IDisplayedDevice"/> を生成します。
    /// </summary>
    /// <param name="rawDevice">基となるNAudioのデバイスオブジェクト。</param>
    /// <returns>生成された <see cref="IDisplayedDevice"/>。</returns>
    IDisplayedDevice Create(MMDevice rawDevice);

    /// <summary>
    /// 生成された <see cref="IDisplayedDevice"/> とその関連オブジェクトを安全に破棄します。
    /// </summary>
    /// <param name="displayedDevice">破棄するデバイスオブジェクト。</param>
    void TearDown(IDisplayedDevice displayedDevice);
}