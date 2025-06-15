// Infrastructure/Services/UserInterface/DialogService.cs
// WPFのMessageBoxを使ってダイアログを表示するサービスです。
namespace OmniPans.Infrastructure.Services.UserInterface;

/// <summary>
/// WPFの <see cref="MessageBox"/> を使ってダイアログを表示するサービスの実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
public class DialogService(ILogger<DialogService> logger) : IDialogService
{
    /// <summary>
    /// メッセージダイアログを表示します。
    /// </summary>
    /// <param name="message">表示するメッセージ本文。</param>
    /// <param name="title">ダイアログのタイトル。</param>
    public void ShowMessage(string message, string title)
    {
        logger.LogInformation("メッセージダイアログ表示: {Title}", title);
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}