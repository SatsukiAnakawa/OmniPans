// Presentation/ViewModels/TrayMenuItems/HiddenDeviceMenuItemViewModel.cs
// コンテキストメニューに表示される、再表示可能な非表示デバイスを表すViewModelです。
namespace OmniPans.Presentation.ViewModels;

public record HiddenDeviceMenuItemViewModel(string DeviceId, string Header) : IMenuItemViewModel;