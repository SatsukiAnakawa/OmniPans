// Core/Services/UserInterface/ITrayMenuViewModel.cs
// トレイアイコンのコンテキストメニューの振る舞いを定義します。
namespace OmniPans.Core.Services.UserInterface;

/// <summary>
/// タスクトレイアイコンのコンテキストメニューのロジックと状態を定義します。
/// </summary>
public interface ITrayMenuViewModel
{
    /// <summary>
    /// アプリケーションがWindowsのスタートアップに登録されているかどうかを示す値を取得または設定します。
    /// </summary>
    bool IsStartupEnabled { get; set; }

    /// <summary>
    /// 非表示に設定されているデバイスを表すメニュー項目のコレクションを取得します。
    /// </summary>
    ObservableCollection<IMenuItemViewModel> HiddenDeviceMenuItems { get; }

    /// <summary>
    /// 非表示のデバイスメニュー項目が1つ以上存在するかどうかを示す値を取得します。
    /// </summary>
    bool HasHiddenDeviceItems { get; }

    /// <summary>
    /// アプリケーションのスタートアップ登録状態を切り替えるコマンドを取得します。
    /// </summary>
    IRelayCommand<bool> ToggleStartupCommand { get; }

    /// <summary>
    /// 指定されたデバイスを再表示（非表示設定を解除）するコマンドを取得します。
    /// </summary>
    IRelayCommand<string> UnhideDeviceCommand { get; }

    /// <summary>
    /// アプリケーションを終了するコマンドを取得します。
    /// </summary>
    IAsyncRelayCommand ExitApplicationCommand { get; }

    /// <summary>
    /// コンテキストメニューが開かれる直前にメニュー項目を最新の状態に更新します。
    /// </summary>
    void LoadMenuItems();
}