using System;
using System.Runtime.InteropServices;

namespace QuickRoute.UI.Classes
{
  public static class WinApi
  {
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(int which);

    [DllImport("user32.dll")]
    public static extern void
        SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                     int X, int Y, int width, int height, uint flags);

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private static readonly IntPtr HWND_TOP = IntPtr.Zero;
    private const int SWP_SHOWWINDOW = 64; // 0×0040

    public static int ScreenX
    {
      get { return GetSystemMetrics(SM_CXSCREEN); }
    }

    public static int ScreenY
    {
      get { return GetSystemMetrics(SM_CYSCREEN); }
    }

    public static void SetWinFullScreen(IntPtr hwnd)
    {
      SetWindowPos(hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
    }
  }
}
