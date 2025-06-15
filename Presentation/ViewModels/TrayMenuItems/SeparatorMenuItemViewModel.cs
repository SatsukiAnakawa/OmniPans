// Presentation/ViewModels/TrayMenuItems/SeparatorMenuItemViewModel.cs
// コンテキストメニューに区切り線を表示するためのViewModelです。
namespace OmniPans.Presentation.ViewModels;

public class SeparatorMenuItemViewModel : IMenuItemViewModel
{
    // アプリケーション全体で共有される唯一のインスタンスを取得します。
    public static SeparatorMenuItemViewModel Instance { get; } = new();

    // 外部からのインスタンス化を防ぎます。
    private SeparatorMenuItemViewModel() { }
}