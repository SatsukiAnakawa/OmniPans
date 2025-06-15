// Core/Services/Common/ISystemClock.cs
// 現在時刻を提供するサービスのインターフェースを定義します。
namespace OmniPans.Core.Services.Common;

/// <summary>
/// 現在時刻を提供するサービスのインターフェースを定義します。
/// これにより、時刻に依存するロジックのテストが容易になります。
/// </summary>
public interface ISystemClock
{
    /// <summary>
    /// 現在の協定世界時 (UTC) を取得します。
    /// </summary>
    DateTime UtcNow { get; }
}