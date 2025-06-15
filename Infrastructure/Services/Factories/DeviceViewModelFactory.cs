// Infrastructure/Services/Factories/DeviceViewModelFactory.cs
// DeviceViewModelのインスタンスを生成するファクトリクラスです。
namespace OmniPans.Infrastructure.Services.Factories;

/// <summary>
/// <see cref="DeviceViewModel"/> のインスタンスを生成するファクトリの実装クラスです。
/// </summary>
/// <param name="serviceProvider">DIコンテナのサービスプロバイダー。</param>
public class DeviceViewModelFactory(IServiceProvider serviceProvider) : IDeviceViewModelFactory
{
    /// <summary>
    /// 指定されたデバイスモデルから新しい <see cref="DeviceViewModel"/> インスタンスを生成します。
    /// </summary>
    /// <param name="device">ViewModelの基となるデバイスモデル。</param>
    /// <returns>新しく生成された <see cref="DeviceViewModel"/> インスタンス。</returns>
    public DeviceViewModel Create(IDisplayedDevice device)
    {
        return new DeviceViewModel(
            device,
            serviceProvider.GetRequiredService<IAudioEndpointController>(),
            serviceProvider.GetRequiredService<IUserDevicePreferencesService>(),
            serviceProvider.GetRequiredService<IUserInteractionTracker>()
        );
    }
}