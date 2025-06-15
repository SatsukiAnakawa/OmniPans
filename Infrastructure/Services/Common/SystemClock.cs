// Infrastructure/Services/Common/SystemClock.cs
// 現在時刻を提供するサービスの実装クラスです。
using OmniPans.Core.Services.Common;

namespace OmniPans.Infrastructure.Services.Common;

/// <summary>
/// <see cref="DateTime.UtcNow"/> を使用して現在時刻を提供するサービスの実装です。
/// </summary>
public class SystemClock : ISystemClock
{
    /// <summary>
    /// 現在の協定世界時 (UTC) を取得します。
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}