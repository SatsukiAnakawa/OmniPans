// Core/Models/IDisplayedDevice.cs
// UIに表示されるオーディオデバイスが実装すべきインターフェースを定義します。
namespace OmniPans.Core.Models;

/// <summary>
/// OSからの音量変更を通知するためのデリゲートを定義します。
/// </summary>
/// <param name="newVolumeScalar">OSから通知された新しい音量のスカラー値 (0.0-1.0)。</param>
public delegate void OsVolumeNotificationDelegate(float newVolumeScalar);

/// <summary>
/// UIに表示されるオーディオデバイスが実装すべきインターフェースを定義します。
/// </summary>
public interface IDisplayedDevice : IDisposable
{
    /// <summary>
    /// OSネイティブの音量変更が通知されると発生します。
    /// </summary>
    event OsVolumeNotificationDelegate? OsVolumeNotificationReceived;

    /// <summary>
    /// デバイスの一意なIDを取得します。
    /// </summary>
    string Id { get; }

    /// <summary>
    /// デバイスの表示名を取得します。
    /// </summary>
    string FriendlyName { get; }

    /// <summary>
    /// デバイスがパンコントロール（ステレオチャンネル）をサポートしているかどうかを示す値を取得します。
    /// </summary>
    bool CanPan { get; }

    /// <summary>
    /// 現在のマスター音量を0.0から1.0の範囲のスカラー値で取得します。
    /// </summary>
    /// <returns>現在のマスター音量スカラー。失敗した場合は0.0f。</returns>
    float GetMasterVolumeScalar();

    /// <summary>
    /// 左右チャンネルの音量を個別のスカラー値で設定します。
    /// </summary>
    /// <param name="leftScalar">左チャンネルの音量スカラー (0.0-1.0)。</param>
    /// <param name="rightScalar">右チャンネルの音量スカラー (0.0-1.0)。</param>
    void SetStereoVolumeScalars(float leftScalar, float rightScalar);

    /// <summary>
    /// モノラルデバイスの音量を単一のスカラー値で設定します。
    /// </summary>
    /// <param name="masterScalar">設定するマスター音量スカラー (0.0-1.0)。</param>
    void SetMonoVolumeScalar(float masterScalar);
}