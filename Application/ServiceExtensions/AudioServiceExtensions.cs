// Application/ServiceExtensions/AudioServiceExtensions.cs
// オーディオ関連のサービスをDIコンテナに登録します。
namespace OmniPans.Application.ServiceExtensions;
public static class AudioServiceExtensions
{
    // アプリケーションのオーディオ関連サービスを登録します。
    public static IServiceCollection AddAppAudioServices(this IServiceCollection services)
    {
        services.AddSingleton<ICoreAudioDeviceService, CoreAudioDeviceService>();
        services.AddSingleton<IAudioEndpointController, AudioEndpointController>();
        services.AddSingleton<IAudioDeviceStateReader, AudioDeviceStateReader>();
        services.AddSingleton<IDeviceFriendlyNameCache, DeviceFriendlyNameCache>();

        services.AddSingleton<PanChangeNotifier>(provider =>
            new PanChangeNotifier(
                provider.GetRequiredService<ILogger<PanChangeNotifier>>(),
                provider.GetRequiredService<ICoreAudioDeviceService>(),
                provider.GetRequiredService<IDispatcherService>(),
                provider.GetRequiredService<Core.Models.Configuration.BehaviorConfig>()
            ));

        services.AddSingleton<IDeviceFilter, DeviceFilter>();
        services.AddSingleton<IManagedDeviceFactory, ManagedDeviceFactory>();

        // AudioDeviceMonitorの依存関係からIUserDevicePreferencesServiceを削除
        services.AddSingleton<AudioDeviceMonitor>(provider =>
            new AudioDeviceMonitor(
                provider.GetRequiredService<ILogger<AudioDeviceMonitor>>(),
                provider.GetRequiredService<ICoreAudioDeviceService>(),
                provider.GetRequiredService<IDispatcherService>(),
                provider.GetRequiredService<IDeviceFilter>(),
                provider.GetRequiredService<IManagedDeviceFactory>(),
                provider.GetRequiredService<IDeviceFriendlyNameCache>(),
                provider.GetRequiredService<PanChangeNotifier>()
            ));
        services.AddSingleton<IAudioDeviceMonitor>(provider => provider.GetRequiredService<AudioDeviceMonitor>());
        services.AddSingleton<IDisplayedDeviceProvider, DisplayedDeviceProvider>();

        return services;
    }
}