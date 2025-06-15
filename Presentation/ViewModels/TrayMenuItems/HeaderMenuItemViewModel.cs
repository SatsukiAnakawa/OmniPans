// Presentation/ViewModels/TrayMenuItems/HeaderMenuItemViewModel.cs
// コンテキストメニューに表示される、クリックできないヘッダーテキスト用のViewModelです。
namespace OmniPans.Presentation.ViewModels;

public record HeaderMenuItemViewModel(string Header) : IMenuItemViewModel;