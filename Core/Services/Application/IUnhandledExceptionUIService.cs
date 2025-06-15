// Core/Services/Application/IUnhandledExceptionUIService.cs
// UIスレッドで発生した未処理例外の通知処理を定義します。
namespace OmniPans.Core.Services.Application;

/// <summary>
/// UIスレッドで発生した未処理例外のユーザー通知処理を定義します。
/// </summary>
public interface IUnhandledExceptionUIService
{
    /// <summary>
    /// UIスレッドで発生した例外を処理し、ユーザーにダイアログで通知します。
    /// </summary>
    /// <param name="exception">発生した例外。</param>
    void HandleUiThreadException(Exception exception);
}