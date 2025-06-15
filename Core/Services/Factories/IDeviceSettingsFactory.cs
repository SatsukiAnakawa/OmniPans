// Core/Services/Factories/IDeviceSettingsFactory.cs
// 新しいデバイスのデフォルト設定を生成するファクトリを定義します。
namespace OmniPans.Core.Services.Factories;

/// <summary>
/// 新しいデバイスのデフォルト設定を生成するファクトリを定義します。
/// </summary>
public interface IDeviceSettingsFactory
{
    /// <summary>
    /// 指定されたデバイスIDのデフォルト設定を生成します。
    /// </summary>
    /// <param name="deviceId">デバイスID。</param>
    /// <returns>生成された <see cref="DeviceSettings"/>。</returns>
    DeviceSettings CreateDefaultSettings(string deviceId);
}