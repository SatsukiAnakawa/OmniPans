// Application/ServiceExtensions/UserSettingsServiceExtensions.cs
// ユーザー設定関連のサービスをDIコンテナに登録します。
namespace OmniPans.Application.ServiceExtensions;
public static class UserSettingsServiceExtensions
{
    // アプリケーションのユーザー設定関連サービスを登録します。
    public static IServiceCollection AddAppUserSettingsServices(this IServiceCollection services, string devicePreferencesFilePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(devicePreferencesFilePath);
        services.AddSingleton<IDeviceSettingsManager>(provider =>
            new DeviceSettingsManager(
                provider.GetRequiredService<ILogger<DeviceSettingsManager>>(),
                devicePreferencesFilePath
            ));

        services.AddSingleton<IUserDevicePreferencesService>(provider =>
            new UserDevicePreferencesService(
                provider.GetRequiredService<IDeviceSettingsManager>(),
                provider.GetRequiredService<IDeviceSettingsFactory>(),
                provider.GetRequiredService<ILogger<UserDevicePreferencesService>>(),
                provider.GetRequiredService<IAudioDeviceStateReader>(),
                provider.GetRequiredService<Core.Models.Configuration.BehaviorConfig>()
            ));

        services.AddSingleton<IUserInteractionTracker>(provider =>
            new UserInteractionTracker(
                provider.GetRequiredService<Core.Services.Common.ISystemClock>()
            ));
        services.AddSingleton<IDeviceSettingsFactory, DeviceSettingsFactory>();
        return services;
    }
}