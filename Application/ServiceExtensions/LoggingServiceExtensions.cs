// Application/ServiceExtensions/LoggingServiceExtensions.cs
// アプリケーションのロギングサービスをDIコンテナに登録します。
namespace OmniPans.Application.ServiceExtensions;

/// <summary>
/// ロギングサービスをDIコンテナに登録するための拡張メソッドを提供します。
/// </summary>
public static class LoggingServiceExtensions
{
    /// <summary>
    /// アプリケーションのロギングサービス (Serilog) を設定し、DIコンテナに登録します。
    /// </summary>
    /// <param name="services">サービスを追加する <see cref="IServiceCollection"/>。</param>
    /// <returns>設定後の <see cref="IServiceCollection"/>。</returns>
    public static IServiceCollection AddAppLoggingServices(this IServiceCollection services)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });
        return services;
    }
}