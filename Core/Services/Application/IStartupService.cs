// Core\Services\Application\IStartupService.cs
namespace OmniPans.Core.Services.Application;

/// <summary>
/// アプリケーションのWindowsスタートアップへの登録状態を管理する機能を提供します。
/// </summary>
public interface IStartupService
{
    /// <summary>
    /// アプリケーションがスタートアップに登録されているか確認します。
    /// </summary>
    /// <returns>登録されている場合は <c>true</c>、それ以外は <c>false</c>。</returns>
    bool IsEnabled();

    /// <summary>
    /// アプリケーションのスタートアップ登録状態を設定します。
    /// </summary>
    /// <param name="isEnabled">スタートアップに登録する場合は <c>true</c>、登録を解除する場合は <c>false</c>。</param>
    void SetEnabled(bool isEnabled);
}