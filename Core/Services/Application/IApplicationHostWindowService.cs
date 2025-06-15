// Core/Services/Application/IApplicationHostWindowService.cs
// アプリケーションのホストウィンドウ（メインウィンドウ）を管理するサービスを定義します。
namespace OmniPans.Core.Services.Application;

/// <summary>
/// アプリケーションのホストウィンドウ（メインウィンドウ）を管理するサービスを定義します。
/// </summary>
public interface IApplicationHostWindowService
{
    /// <summary>
    /// ホストウィンドウを作成し、アプリケーションのメインウィンドウとして設定します。
    /// </summary>
    void CreateAndSetMainWindow();
}