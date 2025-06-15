// Core\Services\UserInterface\ITaskbarIconService.cs
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// タスクバー通知領域のアイコンの初期化と管理を行うサービスを提供します。
/// </summary>
public interface ITaskbarIconService : IDisposable
{
    /// <summary>
    /// タスクバーアイコンを初期化し、表示します。
    /// </summary>
    void Initialize();
}