// Core/Services/UserSettings/IUserInteractionTracker.cs
// ユーザーのデバイス操作時刻を追跡する機能を定義します。
namespace OmniPans.Core.Services.UserSettings;

/// <summary>
/// ユーザーのデバイス操作時刻を追跡する機能を提供します。
/// </summary>
public interface IUserInteractionTracker
{
    /// <summary>
    /// 指定されたデバイスに対するユーザー操作を記録します。
    /// </summary>
    /// <param name="deviceId">操作があったデバイスのID。</param>
    void RecordUserInteraction(string deviceId);

    /// <summary>
    /// 指定されたデバイスに対する最後のユーザー操作時刻を取得します。
    /// </summary>
    /// <param name="deviceId">時刻を取得したいデバイスのID。</param>
    /// <returns>最後の操作の <see cref="DateTime"/>。操作記録がない場合は <see cref="DateTime.MinValue"/>。</returns>
    DateTime GetLastUserInteractionTime(string deviceId);
}