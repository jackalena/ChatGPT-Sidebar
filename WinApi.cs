using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace ChatGPTSidebar;

public static class WinApi
{
    public static bool IsSnappingWindow { get; private set; }
    public static event Action? WindowSnapped;
    public static event Action? HotkeyPressed;
    public static event Action? RecievedShowMsg;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const byte VK_LWIN = 0x5b;
    private const byte VK_LEFT = 0x25;
    private const byte VK_RIGHT = 0x27;
    private const byte VK_ESCAPE = 0x1b;

    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private const int WM_SHOW_WINDOW = SingleInstanceEnforcer.WM_SHOW_WINDOW;

    public static void SnapWindowRight(Window window)
    {
        IsSnappingWindow = true;

        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0);
        Thread.Sleep(50);
        keybd_event(VK_ESCAPE, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(VK_ESCAPE, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0);

        window.Dispatcher.Invoke(() =>
        {
            WindowSnapped?.Invoke();
            window.Activate();
        });

        Thread.Sleep(500);

        IsSnappingWindow = false;
    }

    private const int MOD_ALT = 0x0001;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int MOD_WIN = 0x0008;
    private const int MOD_NOREPEAT = 0x4000;

    private const int WM_HOTKEY = 0x0312;

    private const byte VK_C = 0x43;

    private const int HOTKEY_ID = 1;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_HOTKEY when wParam.ToInt32() == HOTKEY_ID:
                HotkeyPressed?.Invoke();
                handled = true;

                break;
            case WM_SHOW_WINDOW:
                RecievedShowMsg?.Invoke();
                Debug.WriteLine("Showing window...");
                handled = true;

                break;
        }

        return IntPtr.Zero;
    }

    public static void RegisterHotkeys(IntPtr hWnd)
    {
        bool res = RegisterHotKey(hWnd, HOTKEY_ID, MOD_NOREPEAT|MOD_ALT|MOD_WIN, VK_C);
        Debug.WriteLineIf(!res, new Win32Exception(Marshal.GetLastWin32Error()).Message);
    }

    public static void UnregisterHotkeys(IntPtr hWnd)
    {
        UnregisterHotKey(hWnd, HOTKEY_ID);
    }
}