// Presentation/Views/FlyoutWindow.xaml.cs
// フライアウトウィンドウの分離コードです。タスクバーに表示しないための設定を行います。
namespace OmniPans.Presentation.Views;

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

        int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
    }

    #endregion

    #region Win32 API 呼び出し

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    #endregion
}