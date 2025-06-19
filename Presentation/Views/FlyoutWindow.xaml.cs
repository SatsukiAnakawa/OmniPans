// Presentation/Views/FlyoutWindow.xaml.cs
// フライアウトウィンドウの分離コードです。タスクバーに表示しないための設定を行います。
namespace OmniPans.Presentation.Views;

using System.Runtime.InteropServices;

public partial class FlyoutWindow : System.Windows.Window
{
    #region Win32 API 定数

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    #endregion

    #region コンストラクタ

    public FlyoutWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region オーバーライド

    // ウィンドウのソースが初期化された後に、スタイルを変更してタスクバーから隠します。
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var helper = new WindowInteropHelper(this);
        var hwnd = helper.Handle;

        var extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(extendedStyle.ToInt64() | WS_EX_TOOLWINDOW));
    }

    #endregion

    #region Win32 API 呼び出し (32/64bit対応)

    // 現在のウィンドウのスタイルを取得します。
    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : GetWindowLongPtr32(hWnd, nIndex);
    }

    // ウィンドウのスタイルを設定します。
    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    #endregion
}
