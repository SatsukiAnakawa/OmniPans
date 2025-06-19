// Core/Services/Factories/IOsNotificationHandlerFactory.cs
// IOsNotificationHandlerのインスタンスを生成するファクトリーのインターフェースです。
namespace OmniPans.Core.Services.Factories;

public interface IOsNotificationHandlerFactory
{
    // IOsNotificationHandlerの新しいインスタンスを作成します。
    IOsNotificationHandler Create(string deviceId);
}
