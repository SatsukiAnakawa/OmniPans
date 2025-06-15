// Core/Services/Application/IApplicationLifecycleService.cs
// アプリケーションのライフサイクル（主に終了処理）を管理する機能を定義します。
namespace OmniPans.Core.Services.Application;

/// <summary>
/// アプリケーションのライフサイクル（主に終了処理）を管理する機能を提供します。
/// </summary>
public interface IApplicationLifecycleService
{
    /// <summary>
    /// アプリケーションを非同期で終了します。
    /// </summary>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task ExitApplicationAsync();
}