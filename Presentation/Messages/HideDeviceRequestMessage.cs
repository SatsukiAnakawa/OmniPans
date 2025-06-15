// Presentation/Messages/HideDeviceRequestMessage.cs
// 特定のデバイスを非表示にするよう要求するメッセージです。
namespace OmniPans.Presentation.Messages;

public record HideDeviceRequestMessage(string DeviceId);