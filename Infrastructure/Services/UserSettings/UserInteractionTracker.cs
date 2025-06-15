// Infrastructure/Services/UserSettings/UserInteractionTracker.cs
// ユーザーのデバイス操作時刻をセッション内メモリで追跡します。
namespace OmniPans.Infrastructure.Services.UserSettings;

using OmniPans.Core.Services.Common;
using System.Collections.Concurrent;

/// <summary>
/// ユーザーのデバイス操作時刻をセッション内メモリで追跡する実装クラスです。
/// </summary>
public class UserInteractionTracker : IUserInteractionTracker
{
    private readonly ConcurrentDictionary<string, DateTime> _lastInteractionTimes = [];
    private readonly ISystemClock _systemClock;

    /// <summary>
    /// <see cref="UserInteractionTracker"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="systemClock">現在時刻を提供する時計サービス。</param>
    public UserInteractionTracker(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    /// <summary>
    /// 指定されたデバイスに対するユーザー操作を記録します。
    /// </summary>
    /// <param name="deviceId">操作があったデバイスのID。</param>
    public void RecordUserInteraction(string deviceId) => _lastInteractionTimes[deviceId] = _systemClock.UtcNow;

    /// <summary>
    /// 指定されたデバイスに対する最後のユーザー操作時刻を取得します。
    /// </summary>
    /// <param name="deviceId">時刻を取得したいデバイスのID。</param>
    /// <returns>最後の操作の <see cref="DateTime"/>。操作記録がない場合は <see cref="DateTime.MinValue"/>。</returns>
    public DateTime GetLastUserInteractionTime(string deviceId) => _lastInteractionTimes.GetValueOrDefault(deviceId, DateTime.MinValue);
}