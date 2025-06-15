// Infrastructure/Services/Application/CriticalErrorHandler.cs
// 致命的なエラーを処理する実装クラスです。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// 致命的なエラーを処理する実装クラスです。
/// </summary>
/// <param name="dialogService">ユーザーにメッセージを表示するためのダイアログサービス。</param>
/// <param name="lifecycleService">アプリケーションのライフサイクルを管理するサービス。</param>
[SupportedOSPlatform("windows")]
public class CriticalErrorHandler(
    IDialogService dialogService,
    IApplicationLifecycleService lifecycleService) : ICriticalErrorHandler
{
    #region Public Methods

    /// <summary>
    /// 致命的なエラーを処理し、ユーザーに通知後、アプリケーションを終了します。
    /// </summary>
    /// <param name="ex">発生した例外。</param>
    /// <param name="errorMessage">ユーザーに表示する追加のエラーメッセージ。</param>
    public void Handle(Exception ex, string errorMessage)
    {
        const string errorTitle = "致命的な起動エラー";
        string fullErrorMessage = $"アプリケーションの起動中に致命的なエラーが発生しました。\n" +
                                  $"{errorMessage}\n" +
                                  $"エラー内容: {ex.Message}\n\n" +
                                  "アプリケーションを終了します。";

        dialogService.ShowMessage(fullErrorMessage, errorTitle);
        lifecycleService.ExitApplicationAsync().GetAwaiter().GetResult();
    }

    #endregion
}