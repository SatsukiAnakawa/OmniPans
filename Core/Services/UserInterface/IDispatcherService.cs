// Core\Services\UserInterface\IDispatcherService.cs
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// UIスレッドでの処理実行を抽象化するサービスを提供します。
/// </summary>
public interface IDispatcherService
{
    /// <summary>
    /// 関連付けられたWPFの <see cref="Dispatcher"/> を取得します。
    /// </summary>
    Dispatcher Dispatcher { get; }

    /// <summary>
    /// UIスレッドで指定されたアクションを非同期で実行します。処理の完了を待ちません。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    void BeginInvoke(Action action);

    /// <summary>
    /// UIスレッドで指定されたアクションを同期的に実行します。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    void Invoke(Action action);

    /// <summary>
    /// UIスレッドで指定されたアクションを非同期で実行し、その完了を待機可能な <see cref="Task"/> を返します。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    Task InvokeAsync(Action action);
}