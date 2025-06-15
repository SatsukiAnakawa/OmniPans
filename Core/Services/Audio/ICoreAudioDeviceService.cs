// Core/Services/Audio/ICoreAudioDeviceService.cs
// OSのCore Audio APIを介してオーディオデバイスを操作するサービスのインターフェースです。
namespace OmniPans.Core.Services.Audio;

/// <summary>
/// OSのCore Audio APIを介してオーディオデバイスを操作するサービスのインターフェースです。
/// </summary>
public interface ICoreAudioDeviceService : IDisposable
{
    /// <summary>
    /// 新しいオーディオデバイスがシステムに追加されたときに発生します。
    /// </summary>
    event EventHandler<string>? DeviceAdded;

    /// <summary>
    /// オーディオデバイスがシステムから削除されたときに発生します。
    /// </summary>
    event EventHandler<string>? DeviceRemoved;

    /// <summary>
    /// デバイスの状態（有効、無効など）が変更されたときに発生します。
    /// </summary>
    event EventHandler<DeviceStateChangedArgs>? DeviceStateChanged;

    /// <summary>
    /// デフォルトのオーディオデバイスが変更されたときに発生します。
    /// </summary>
    event EventHandler<DefaultDeviceChangedArgs>? DefaultDeviceChanged;

    /// <summary>
    /// デバイスのプロパティ（名前など）が変更されたときに発生します。
    /// </summary>
    event EventHandler<DevicePropertyChangedArgs>? DevicePropertyChanged;

    /// <summary>
    /// 現在アクティブな音声出力デバイスの一覧を取得します。
    /// </summary>
    /// <returns>アクティブな <see cref="MMDevice"/> のコレクション。</returns>
    IEnumerable<MMDevice> GetActiveRenderDevices();

    /// <summary>
    /// デバイスIDを指定して、対応する <see cref="MMDevice"/> オブジェクトを取得します。
    /// </summary>
    /// <param name="deviceId">取得するデバイスのID。</param>
    /// <returns>見つかった <see cref="MMDevice"/> オブジェクト。見つからない場合は null。</returns>
    MMDevice? GetDeviceById(string deviceId);
}