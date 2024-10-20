using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTSidebar;

public static class SingleInstanceEnforcer
{
    private const int WM_APP = 0x8000;
    public const int WM_SHOW_WINDOW = WM_APP + 1;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    private static IntPtr FindWindowInAnotherProcess(int targetProcessId)
    {
        IntPtr foundHwnd = IntPtr.Zero;
        const string classNameStart = "HwndWrapper[ChatGPTSidebar;;";

        EnumWindows((hWnd, lParam) =>
        {
            GetWindowThreadProcessId(hWnd, out uint processId);

            if (processId == targetProcessId)
            {
                StringBuilder className = new(256);
                GetClassName(hWnd, className, className.Capacity);

                if (className.ToString().StartsWith(classNameStart))
                {
                    foundHwnd = hWnd;

                    return false;
                }
            }

            return true;
        }, IntPtr.Zero);

        return foundHwnd;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public static void ActivateExistingInstance()
    {
        using Process currentProcess = Process.GetCurrentProcess();
        using Process? existingProcess = Process.GetProcessesByName(currentProcess.ProcessName).FirstOrDefault(p => p.Id != currentProcess.Id);

        if (existingProcess is null) return;

        IntPtr hWnd = FindWindowInAnotherProcess(existingProcess.Id);

        bool res = PostMessage(hWnd, WM_SHOW_WINDOW, IntPtr.Zero, IntPtr.Zero);
        if (!res) Debug.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()).Message);
    }
}