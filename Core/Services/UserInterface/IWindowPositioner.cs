// Core/Services/UserInterface/IWindowPositioner.cs
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// ウィンドウの位置決め処理を定義するインターフェースです。
/// </summary>
public interface IWindowPositioner
{
    /// <summary>
    /// フライアウトウィンドウをタスクバーの近く、マウスカーソルの水平位置に合わせて配置します。
    /// </summary>
    /// <param name="window">配置対象のウィンドウ。</param>
    /// <returns>計算されたウィンドウの左端のX座標。</returns>
    double PositionFlyout(Window window);

    /// <summary>
    /// フライアウトウィンドウのY座標を、現在の高さに基づいて再計算して配置します。
    /// </summary>
    /// <param name="window">再配置対象のウィンドウ。</param>
    /// <param name="currentLeft">現在のウィンドウの左端のX座標。</param>
    void RepositionFlyoutY(Window window, double currentLeft);
}