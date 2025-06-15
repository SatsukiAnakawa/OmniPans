// Infrastructure/Services/Application/ApplicationHostWindowService.cs
// アプリケーションのホストウィンドウを管理するサービスの実装クラスです。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// アプリケーションのホストウィンドウ（メインウィンドウ）を管理するサービスの実装です。
/// この実装では、タスクトレイアプリケーションとして動作するために不可視のウィンドウを生成します。
/// </summary>
public class ApplicationHostWindowService : IApplicationHostWindowService
{
    /// <summary>
    /// アプリケーションのメインウィンドウとして機能する、タスクバーに表示されない不可視のウィンドウを作成します。
    /// </summary>
    public void CreateAndSetMainWindow()
    {
        var invisibleWindow = new Window
        {
            Width = 0,
            Height = 0,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
            AllowsTransparency = true,
            Background = Brushes.Transparent
        };
        invisibleWindow.Show();
        invisibleWindow.Hide();
        System.Windows.Application.Current.MainWindow = invisibleWindow;
    }
}