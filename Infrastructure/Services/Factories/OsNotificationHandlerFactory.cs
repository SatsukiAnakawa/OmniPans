// Infrastructure/Services/Factories/OsNotificationHandlerFactory.cs
// OsNotificationHandlerのインスタンスを生成します。
namespace OmniPans.Infrastructure.Services.Factories;

public class OsNotificationHandlerFactory : IOsNotificationHandlerFactory
{
    private readonly IDispatcherService _dispatcher;
    private readonly IUserInteractionTracker _interactionTracker;
    private readonly IMessenger _messenger;
    private readonly Core.Models.Configuration.BehaviorConfig _config;

    // OsNotificationHandlerFactory の新しいインスタンスを初期化します。
    public OsNotificationHandlerFactory(
        IDispatcherService dispatcher,
        IUserInteractionTracker interactionTracker,
        IMessenger messenger,
        Core.Models.Configuration.BehaviorConfig config)
    {
        _dispatcher = dispatcher;
        _interactionTracker = interactionTracker;
        _messenger = messenger;
        _config = config;
    }

    #region Public Methods

    // IOsNotificationHandler の新しいインスタンスを作成します。
    public IOsNotificationHandler Create(string deviceId)
    {
        return new OsNotificationHandler(
            deviceId,
            _dispatcher,
            _interactionTracker,
            _messenger,
            _config.OsVolumeNotificationDebounceMs,
            _config.UserInteractionGracePeriodMs
        );
    }

    #endregion
}
