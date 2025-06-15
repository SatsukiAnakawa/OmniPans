// Infrastructure/Services/Application/UnhandledExceptionUIService.cs
// UIスレッドの未処理例外をダイアログでユーザーに通知する実装クラスです。
namespace OmniPans.Infrastructure.Services.Application;

/// <summary>
/// UIスレッドの未処理例外をダイアログでユーザーに通知する実装クラスです。
/// </summary>
/// <param name="logger">ロギングサービス。</param>
/// <param name="dialogService">ユーザーにメッセージを表示するためのダイアログサービス。</param>
public class UnhandledExceptionUIService(
    ILogger<UnhandledExceptionUIService> logger,
    IDialogService dialogService) : IUnhandledExceptionUIService
{
    #region Public Methods

    /// <summary>
    /// UIスレッドで発生した例外を処理し、ユーザーにダイアログで通知します。
    /// </summary>
    /// <param name="exception">発生した例外。</param>
    public void HandleUiThreadException(Exception exception)
    {
        logger.LogCritical(exception, "未処理の例外発生 (UIスレッド)。");
        try
        {
            dialogService.ShowMessage(
                $"予期せぬエラーが発生しました: {exception.Message}\n\nアプリケーションの動作が不安定になる可能性があります。可能であれば作業内容を保存し、アプリケーションを再起動してください。",
                "アプリケーションエラー");
        }
        catch (Exception dialogEx)
        {
            logger.LogError(dialogEx, "エラーダイアログ表示失敗 (HandleUiThreadException)。");
        }
    }

    #endregion
}