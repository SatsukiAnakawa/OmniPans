// Infrastructure/Services/UserSettings/IOsNotificationHandlerFactory.cs
// ユーザーのデバイス操作時刻をセッション内メモリで追跡します。

// IOsNotificationHandlerのインスタンスを生成するファクトリーのインターフェースです。
namespace OmniPans.Core.Services.Factories;

public interface IOsNotificationHandlerFactory
{
    // IOsNotificationHandlerの新しいインスタンスを作成します。
    IOsNotificationHandler Create(string deviceId);
}