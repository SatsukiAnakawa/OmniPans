// Application/ServiceExtensions/ApplicationServicesExtensions.cs
// アプリケーション層で利用する各種サービスをDIコンテナに登録します。
namespace OmniPans.Application.ServiceExtensions;
public static class ApplicationServicesExtensions
{
    // アプリケーションのコアサービスをDIコンテナに登録します。
    public static IServiceCollection AddAppApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
        services.AddSingleton<IStartupService, StartupService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IUnhandledExceptionUIService, UnhandledExceptionUIService>();
        services.AddSingleton<ITaskbarIconService, TaskbarIconService>();
        services.AddSingleton<IWindowPositioner, WindowPositioner>();
        services.AddSingleton<ICriticalErrorHandler, CriticalErrorHandler>();
        services.AddSingleton<IApplicationInitializer, ApplicationInitializer>();
        services.AddSingleton<IApplicationHostWindowService, ApplicationHostWindowService>();
        services.AddSingleton<IDispatcherService>(provider =>
            new WpfDispatcherService(
                System.Windows.Application.Current.Dispatcher
            ));
        services.AddSingleton<IDeviceViewModelFactory, DeviceViewModelFactory>();
        services.AddSingleton<IOsNotificationHandlerFactory>(provider =>
            new OsNotificationHandlerFactory(
                provider.GetRequiredService<IDispatcherService>(),
                provider.GetRequiredService<IUserInteractionTracker>(),
                provider.GetRequiredService<Core.Models.Configuration.BehaviorConfig>()
            ));
        services.AddTransient<FlyoutViewModel>();
        services.AddTransient<ITrayMenuViewModel, TrayMenuViewModel>();

        services.AddSingleton<IFlyoutService>(provider =>
             new FlyoutService(
                 provider.GetRequiredService<ILogger<FlyoutService>>(),
                 () => provider.GetRequiredService<FlyoutViewModel>(),
                 provider.GetRequiredService<IWindowPositioner>()
             ));
        return services;
    }
}