// Infrastructure/Services/UserInterface/WpfDispatcherService.cs
// WPFのDispatcherをラップし、UIスレッドでの処理実行を抽象化します。
namespace OmniPans.Infrastructure.Services.UserInterface;

/// <summary>
/// WPFの <see cref="Dispatcher"/> をラップし、UIスレッドでの処理実行を抽象化する実装クラスです。
/// </summary>
/// <param name="dispatcher">ラップするWPFの <see cref="Dispatcher"/> インスタンス。</param>
public class WpfDispatcherService(Dispatcher dispatcher) : IDispatcherService
{
    /// <summary>
    /// 関連付けられたWPFの <see cref="Dispatcher"/> を取得します。
    /// </summary>
    public Dispatcher Dispatcher { get; } = dispatcher;

    /// <summary>
    /// UIスレッドで指定されたアクションを非同期で実行します。処理の完了を待ちません。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    public void BeginInvoke(Action action) => Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);

    /// <summary>
    /// UIスレッドで指定されたアクションを同期的に実行します。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    public void Invoke(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.Invoke(action);
        }
    }

    /// <summary>
    /// UIスレッドで指定されたアクションを非同期で実行し、その完了を待機可能な <see cref="Task"/> を返します。
    /// </summary>
    /// <param name="action">実行するアクション。</param>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public Task InvokeAsync(Action action) => Dispatcher.InvokeAsync(action, DispatcherPriority.DataBind).Task;
}