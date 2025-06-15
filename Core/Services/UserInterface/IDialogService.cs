// Core/Services/UserInterface/IDialogService.cs
// ユーザーへのメッセージダイアログ表示機能のインターフェースを定義します。
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// ユーザーへのメッセージダイアログ表示機能を提供します。
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// メッセージダイアログを表示します。
    /// </summary>
    /// <param name="message">表示するメッセージ本文。</param>
    /// <param name="title">ダイアログのタイトル。</param>
    void ShowMessage(string message, string title);
}