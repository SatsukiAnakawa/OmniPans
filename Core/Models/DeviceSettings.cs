// Core/Models/DeviceSettings.cs
// デバイスごとの設定を保持する不変のデータ構造です。
namespace OmniPans.Core.Models;

/// <summary>
/// デバイスごとの設定を保持する不変のデータ構造です。
/// </summary>
public record DeviceSettings
{
    /// <summary>
    /// 音量のデフォルト値 (0-100)。
    /// </summary>
    public const double DefaultVolume = 100.0;
    /// <summary>
    /// 音量の最小値。
    /// </summary>
    public const double MinVolume = 0.0;
    /// <summary>
    /// 音量の最大値。
    /// </summary>
    public const double MaxVolume = 100.0;
    /// <summary>
    /// パン（左右バランス）のデフォルト値 (-100 to 100)。
    /// </summary>
    public const double DefaultPan = 0.0;
    /// <summary>
    /// パンの最小値（左端）。
    /// </summary>
    public const double MinPan = -100.0;
    /// <summary>
    /// パンの最大値（右端）。
    /// </summary>
    public const double MaxPan = 100.0;
    /// <summary>
    /// デバイスの音量 (0-100) を取得または初期化します。
    /// </summary>
    public double Volume { get; init; } = DefaultVolume;
    /// <summary>
    /// デバイスのパン（左右バランス, -100 to 100）を取得または初期化します。
    /// </summary>
    public double Pan { get; init; } = DefaultPan;
    /// <summary>
    /// ユーザーによってこのデバイスが非表示に設定されているかどうかを示す値を取得または初期化します。
    /// </summary>
    public bool IsUserHidden { get; init; } = false;
}