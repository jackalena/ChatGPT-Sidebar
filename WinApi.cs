using System;
using System.Collections.Generic;
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

    // Import keybd_event from the Windows API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    // Virtual key codes
    private const byte VK_LWIN = 0x5b;
    private const byte VK_LEFT = 0x25;
    private const byte VK_RIGHT = 0x27;
    private const byte VK_ESCAPE = 0x1b;

    // Key flags
    private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down event
    private const uint KEYEVENTF_KEYUP = 0x0002;   // Key up event

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
}