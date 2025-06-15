// Core/Services/Application/IApplicationInitializer.cs
// アプリケーションの主要なサービスの初期化処理を定義します。
namespace OmniPans.Core.Services.Application;

/// <summary>
/// アプリケーションの主要なサービスの初期化処理を定義します。
/// </summary>
public interface IApplicationInitializer
{
    /// <summary>
    /// アプリケーションのサービスを初期化します。
    /// </summary>
    void Initialize();
}