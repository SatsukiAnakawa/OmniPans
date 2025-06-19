// Infrastructure/Services/Factories/DeviceViewModelFactory.cs
// DeviceViewModelのインスタンスを生成するファクトリクラスです。
namespace OmniPans.Infrastructure.Services.Factories;

public class DeviceViewModelFactory(IServiceProvider serviceProvider) : IDeviceViewModelFactory
{
    // 指定されたデバイスモデルから新しい DeviceViewModel インスタンスを生成します。
    public DeviceViewModel Create(IDisplayedDevice device)
    {
        return new DeviceViewModel(
            device,
            serviceProvider.GetRequiredService<IAudioEndpointController>(),
            serviceProvider.GetRequiredService<IUserDevicePreferencesService>(),
            serviceProvider.GetRequiredService<IUserInteractionTracker>(),
            serviceProvider.GetRequiredService<IMessenger>(),
            serviceProvider.GetRequiredService<ILogger<DeviceViewModel>>()
         );
    }
}
