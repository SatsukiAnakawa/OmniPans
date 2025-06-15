// Infrastructure/Services/Factories/ManagedDeviceFactory.cs
// 管理対象となるデバイスオブジェクトを生成・破棄するファクトリの実装です。
namespace OmniPans.Infrastructure.Services.Factories;

/// <summary>
/// 管理対象となるデバイスオブジェクト（<see cref="IDisplayedDevice"/>）を生成・破棄するファクトリの実装です。
/// </summary>
/// <param name="loggerFactory">ロガーを生成するためのファクトリ。</param>
/// <param name="notificationHandlerFactory">OS通知ハンドラを生成するためのファクトリ。</param>
/// <param name="nameCache">デバイス名キャッシュサービス。</param>
public class ManagedDeviceFactory(
    ILoggerFactory loggerFactory,
    IOsNotificationHandlerFactory notificationHandlerFactory,
    IDeviceFriendlyNameCache nameCache) : IManagedDeviceFactory
{
    /// <summary>
    /// <see cref="MMDevice"/> から、関連オブジェクト（ロガー、通知ハンドラ）を含む <see cref="IDisplayedDevice"/> を生成します。
    /// </summary>
    /// <param name="rawDevice">基となるNAudioのデバイスオブジェクト。</param>
    /// <returns>生成された <see cref="IDisplayedDevice"/>。</returns>
    public IDisplayedDevice Create(MMDevice rawDevice)
    {
        var friendlyName = nameCache.GetFriendlyName(rawDevice);
        var displayDevice = new DisplayedDevice(rawDevice, friendlyName, loggerFactory.CreateLogger<DisplayedDevice>());

        var handler = notificationHandlerFactory.Create(displayDevice.Id);
        displayDevice.OsVolumeNotificationReceived += handler.HandleOsVolumeNotification;

        // ハンドラをDisplayedDeviceに紐付けて、一緒に破棄できるようにする
        if (displayDevice is DisplayedDevice concreteDevice)
        {
            concreteDevice.AttachNotificationHandler(handler);
        }

        return displayDevice;
    }

    /// <summary>
    /// 生成された <see cref="IDisplayedDevice"/> とその関連オブジェクトを安全に破棄します。
    /// </summary>
    /// <param name="displayedDevice">破棄するデバイスオブジェクト。</param>
    public void TearDown(IDisplayedDevice displayedDevice)
    {
        // DisplayedDeviceのDisposeが、ハンドラのDisposeも呼び出すように設計されているため、
        // ここではDisplayedDeviceのDisposeを呼び出すだけでよい。
        displayedDevice.Dispose();
    }
}