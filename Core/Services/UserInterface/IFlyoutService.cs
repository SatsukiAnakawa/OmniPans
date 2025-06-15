// Core\Services\UserInterface\IFlyoutService.cs
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// UIのフライアウトウィンドウの表示・非表示を管理するサービスを提供します。
/// </summary>
public interface IFlyoutService : IDisposable
{
    /// <summary>
    /// フライアウトウィンドウの表示状態を切り替えます。表示されていなければ表示し、表示されていればアクティブにします。
    /// </summary>
    void ToggleFlyoutWindow();
}