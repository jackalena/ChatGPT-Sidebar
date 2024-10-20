using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

using ChatGPTSidebar.Config;

namespace ChatGPTSidebar;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static Mutex mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        mutex = new(true, "ChatGPTSidebar", out bool isNewInstance);

        if (!isNewInstance)
        {
            SingleInstanceEnforcer.ActivateExistingInstance();

            Current.Shutdown();

            return;
        }

        base.OnStartup(e);
    }

    public App()
    {
        Settings.Load();
    }

    private void App_OnExit(object sender, ExitEventArgs e)
    {
        Settings.Save();
    }
}