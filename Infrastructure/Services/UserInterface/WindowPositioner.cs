// Infrastructure/Services/UserInterface/WindowPositioner.cs
// ウィンドウの位置を計算し、配置する責務を持ちます。
namespace OmniPans.Infrastructure.Services.UserInterface;

[SupportedOSPlatform("windows")]
public class WindowPositioner : IWindowPositioner
{
    #region Public Methods

    // フライアウトウィンドウをタスクバーの近く、マウスカーソルの水平位置に合わせて配置します。
    public double PositionFlyout(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        double flyoutWidth = window.ActualWidth;
        double flyoutHeight = window.ActualHeight;
        if (flyoutWidth <= 0 || flyoutHeight <= 0) return window.Left;
        var workArea = SystemParameters.WorkArea;
        var mousePosition = GetMousePosition();

        double targetLeft = mousePosition.X - (flyoutWidth / 2);
        targetLeft = Math.Max(workArea.Left, Math.Min(targetLeft, workArea.Right - flyoutWidth));

        const double taskbarOffset = 5;
        double targetTop = workArea.Bottom - flyoutHeight - taskbarOffset;
        targetTop = Math.Max(workArea.Top, targetTop);

        window.Left = targetLeft;
        window.Top = targetTop;
        return targetLeft;
    }

    // フライアウトウィンドウのY座標を、現在の高さに基づいて再計算して配置します。
    public void RepositionFlyoutY(Window window, double currentLeft)
    {
        ArgumentNullException.ThrowIfNull(window);
        double flyoutHeight = window.ActualHeight;
        if (flyoutHeight <= 0) return;

        var workArea = SystemParameters.WorkArea;
        const double taskbarOffset = 5;
        double targetTop = workArea.Bottom - flyoutHeight - taskbarOffset;

        window.Top = Math.Max(workArea.Top, targetTop);
        window.Left = currentLeft;
    }

    #endregion

    #region Win32 Helpers

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out Win32Point lpPoint);

    // 現在のマウスカーソルの位置を取得します。
    private static System.Windows.Point GetMousePosition()
    {
        return GetCursorPos(out Win32Point lpPoint)
            ? new System.Windows.Point(lpPoint.X, lpPoint.Y)
            : new System.Windows.Point(0, 0);
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct Win32Point { public int X; public int Y; }

    #endregion
}
