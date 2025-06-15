// Core/Models/Configuration/BehaviorConfig.cs
// アプリケーションの動作タイミングなど、振る舞いに関する設定を定義します。
namespace OmniPans.Core.Models.Configuration;

/// <summary>
/// アプリケーションの動作タイミングなど、振る舞いに関する設定を定義します。
/// </summary>
public record BehaviorConfig
{
    /// <summary>
    /// OSからの音量変更通知を処理する際のデバウンス時間（ミリ秒）を取得または設定します。
    /// </summary>
    public int OsVolumeNotificationDebounceMs { get; init; } = 300;

    /// <summary>
    /// ユーザー自身の操作によるOS通知を無視するための猶予期間（ミリ秒）を取得または設定します。
    /// </summary>
    public int UserInteractionGracePeriodMs { get; init; } = 200;

    /// <summary>
    /// OSからのパン（左右バランス）変更通知を処理する際のデバウンス時間（ミリ秒）を取得または設定します。
    /// </summary>
    public int OsPanNotificationDebounceMs { get; init; } = 400;

    /// <summary>
    /// ユーザー設定を自動保存する際の遅延時間（秒）を取得または設定します。
    /// </summary>
    public int AutoSaveDelaySeconds { get; init; } = 3;
}