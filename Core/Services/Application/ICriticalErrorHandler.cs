// Core/Services/Application/ICriticalErrorHandler.cs
// 致命的なエラーを処理するサービスのインターフェースです。
namespace OmniPans.Core.Services.Application;

/// <summary>
/// 致命的なエラーを処理するサービスのインターフェースを定義します。
/// </summary>
public interface ICriticalErrorHandler
{
    /// <summary>
    /// 致命的なエラーを処理し、ユーザーに通知後、アプリケーションを終了します。
    /// </summary>
    /// <param name="ex">発生した例外。</param>
    /// <param name="errorMessage">ユーザーに表示する追加のエラーメッセージ。</param>
    void Handle(Exception ex, string errorMessage);
}