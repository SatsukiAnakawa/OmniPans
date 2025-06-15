// Core/AppConstants.cs
// アプリケーション全体で共有される定数を定義します。
namespace OmniPans.Core;

public static class AppConstants
{
    #region 全般

    public const string AppName = "OmniPans";
#pragma warning disable S1075
    public const string AppIconUri = "pack://application:,,,/Application/app_icon.ico";
#pragma warning restore S1075

    #endregion

    #region ログ関連

    public const string MainLoggerName = "App";
    public const string LogFilePath = "logs/omnipans_log_.txt";

    #endregion

    #region 設定関連

    public const string UserSettingsFileName = "omnipans_user_device_preferences.json";

    #endregion

    #region UI関連

    public const string TrayContextMenuResourceKey = "TrayContextMenu";

    #endregion
}