using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatGPTSidebar.Config;

internal static class WebViewSourceManager
{
    public static SerializableWebViewSource[] DefaultSources { get; } = [
        new("https://chatgpt.com/", "ChatGPT", new(33, 33, 33), (SerializableColor) Colors.White),
        new("https://copilot.microsoft.com/", "Microsoft Copilot", new(12, 16, 28), (SerializableColor) Colors.White),
        new("https://gemini.google.com/app", "Google Gemini", new(19, 19, 20), (SerializableColor) Colors.White),
    ];

    public static void Init()
    {
        if (Settings.Current.Sources?.Length > 1)
            Sources = new(Settings.Current.Sources.Select(SerializableWebViewSource.GetWebViewSource));
        else
            Sources = new(DefaultSources.Select(SerializableWebViewSource.GetWebViewSource));
    }

    public static ObservableCollection<WebViewSource> Sources { get; private set; }
    public static WebViewSource CurrentSource => Sources[Settings.Current.WebViewSource];
}