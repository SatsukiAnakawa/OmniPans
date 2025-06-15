// Infrastructure/Services/Factories/OsNotificationHandlerFactory.cs
// OsNotificationHandlerのインスタンスを生成します。
namespace OmniPans.Infrastructure.Services.Factories;

/// <summary>
/// <see cref="OsNotificationHandler"/> のインスタンスを生成するファクトリの実装クラスです。
/// </summary>
public class OsNotificationHandlerFactory : IOsNotificationHandlerFactory
{
    private readonly IDispatcherService _dispatcher;
    private readonly IUserInteractionTracker _interactionTracker;
    private readonly Core.Models.Configuration.BehaviorConfig _config;

    /// <summary>
    /// <see cref="OsNotificationHandlerFactory"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="dispatcher">UIスレッド操作サービス。</param>
    /// <param name="interactionTracker">ユーザー操作追跡サービス。</param>
    /// <param name="config">振る舞いに関する設定。</param>
    public OsNotificationHandlerFactory(
        IDispatcherService dispatcher,
        IUserInteractionTracker interactionTracker,
        Core.Models.Configuration.BehaviorConfig config)
    {
        _dispatcher = dispatcher;
        _interactionTracker = interactionTracker;
        _config = config;
    }

    #region Public Methods

    /// <summary>
    /// <see cref="IOsNotificationHandler"/> の新しいインスタンスを作成します。
    /// </summary>
    /// <param name="deviceId">ハンドラーが対象とするデバイスのID。</param>
    /// <returns>新しく作成された <see cref="IOsNotificationHandler"/> のインスタンス。</returns>
    public IOsNotificationHandler Create(string deviceId)
    {
        return new OsNotificationHandler(
            deviceId,
            _dispatcher,
            _interactionTracker,
            _config.OsVolumeNotificationDebounceMs,
            _config.UserInteractionGracePeriodMs
        );
    }

    #endregion
}