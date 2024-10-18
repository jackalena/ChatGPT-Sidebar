using System.Configuration;
using System.Data;
using System.Windows;

using ChatGPTSidebar.Config;

namespace ChatGPTSidebar;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        Settings.Load();
    }

    private void App_OnExit(object sender, ExitEventArgs e)
    {
        Settings.Save();
    }
}