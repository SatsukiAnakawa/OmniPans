// Core/Services/Factories/IDeviceViewModelFactory.cs
namespace OmniPans.Core.Services.Factories;

/// <summary>
/// <see cref="DeviceViewModel"/> のインスタンスを生成するファクトリのインターフェースです。
/// </summary>
public interface IDeviceViewModelFactory
{
    /// <summary>
    /// 指定されたデバイスモデルから新しい <see cref="DeviceViewModel"/> インスタンスを生成します。
    /// </summary>
    /// <param name="device">ViewModelの基となるデバイスモデル。</param>
    /// <returns>新しく生成された <see cref="DeviceViewModel"/> インスタンス。</returns>
    DeviceViewModel Create(IDisplayedDevice device);
}