// Presentation/Messages/OsVolumeChangedMessage.cs
// OSの音量が外部で変更されたことを通知するメッセージです。
namespace OmniPans.Presentation.Messages;

public record OsVolumeChangedMessage(string DeviceId, double NewVolume);