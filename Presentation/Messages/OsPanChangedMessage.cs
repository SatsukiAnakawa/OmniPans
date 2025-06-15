// Presentation/Messages/OsPanChangedMessage.cs
// OSのパン（左右バランス）が外部で変更されたことを通知するメッセージです。
namespace OmniPans.Presentation.Messages;

public record OsPanChangedMessage(string DeviceId, double NewPan);