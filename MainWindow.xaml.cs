using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

using ChatGPTSidebar.Config;

using Microsoft.Web.WebView2.Core;

namespace ChatGPTSidebar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private Rect workingArea = SystemParameters.WorkArea;
    private ObservableCollection<WebViewSource> sources = [];

    public MainWindow()
    {
        InitializeComponent();
        WebView.BeginInit();

        Loaded += MainWindow_Loaded;

        MoveToSide();
        ContentRendered += (_, _) =>
        {
            Show();
            Activate();

            string[] args = Environment.GetCommandLineArgs();
            bool shouldHide = args.Length > 1 && args[1] == "-hidden";
            Task.Run(() => WinApi.SnapWindowRight(this, shouldHide));
        };

        WinApi.WindowSnapped += WindowSnapped;
        WinApi.RecievedShowMsg += () =>
        {
            Show();
            Activate();
        };

        ChatTypeComboBox.ItemsSource = WebViewSourceManager.Sources;
        ChatTypeComboBox.SelectedIndex = Settings.Current.WebViewSource;
        ChatTypeComboBox.DisplayMemberPath = "DisplayName";
        ChatTypeComboBox.SelectionChanged += ChatTypeComboBox_SelectionChanged;

        WebView.Source = WebViewSourceManager.CurrentSource.Uri;

        _ = InitialiseWebViewAsync();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        WindowInteropHelper helper = new(this);
        HwndSource? source = HwndSource.FromHwnd(helper.Handle);

        source.AddHook(WinApi.HwndHook);

        WinApi.RegisterHotkeys(source.Handle);

        WinApi.HotkeyPressed += WinApi_HotkeyPressed;
    }

    private void WinApi_HotkeyPressed()
    {
        if (IsActive)
        {
            _ = Settings.SaveAsync();
            Hide();
        }
        else
        {
            if (!IsVisible) Show();
            Activate();
        }
    }

    private async Task InitialiseWebViewAsync()
    {
        string folder = Path.Combine(Settings.BaseSettingsFolder, "WebView2");
        CoreWebView2Environment? env = await CoreWebView2Environment.CreateAsync(null, folder);

        await WebView.EnsureCoreWebView2Async(env);

        WebView.CoreWebView2.HistoryChanged += WebView_HistoryChanged;

        Dispatcher.Invoke(UpdateWebViewSources);
        WebView.EndInit();
    }

    private void ChatTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateWebViewSources();
    }

    private void UpdateWebViewSources()
    {
        Settings.Current.WebViewSource = ChatTypeComboBox.SelectedIndex;

        Application.Current.Resources["BackgroundBrush"] =
            new SolidColorBrush(WebViewSourceManager.CurrentSource.BackgroundColor);
        Application.Current.Resources["TextBrush"] = new SolidColorBrush(WebViewSourceManager.CurrentSource.TextColor);
        Application.Current.Resources["MouseOverBrush"] =
            new SolidColorBrush(WebViewSourceManager.CurrentSource.MouseOverColor);
        Application.Current.Resources["PressedBrush"] =
            new SolidColorBrush(WebViewSourceManager.CurrentSource.PressedColor);
        Application.Current.Resources["DisabledBrush"] =
            new SolidColorBrush(WebViewSourceManager.CurrentSource.DisabledColor);

        WebView.Source = WebViewSourceManager.CurrentSource.Uri;

        Color color = WebViewSourceManager.CurrentSource.BackgroundColor;
        WebView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
    }

    // TODO: custom option adds to menu, saved, new window to manage

    private void WindowSnapped()
    {
        MoveToSide();
    }

    private void MoveToSide()
    {
        int windowWidth = Settings.Current.LastWindowWidth;
        Left = workingArea.Right - windowWidth;
        Top = workingArea.Top;
        Height = workingArea.Height;
        Width = windowWidth;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (WinApi.IsSnappingWindow) return;

        _ = Settings.SaveAsync();
        Hide();
    }

    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
        WindowState = WindowState.Normal;
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (WinApi.IsSnappingWindow) return;

        if ((int) (Height + 0.5) == (int) (workingArea.Height + 0.5)) Settings.Current.LastWindowWidth = (int) (Width + 0.5);

        MoveToSide();
    }

    private void MenuButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuDropdown.PlacementTarget = (UIElement) sender;
        MenuDropdown.Placement = PlacementMode.Bottom;
        MenuDropdown.IsOpen = true;
    }

    private void WebView_HistoryChanged(object? sender, object e)
    {
        BackButton.IsEnabled = WebView.CanGoBack;
        ForwardButton.IsEnabled = WebView.CanGoForward;
    }

    private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
    {
        WebView.GoForward();
    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        WebView.GoBack();
    }

    private void ReloadButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (WebView.CoreWebView2 is null) return;
        WebView.Reload();
    }

    private void SettingsButton_OnClick(object sender, RoutedEventArgs e) { }

    private void ExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Hide();

        WindowInteropHelper helper = new(this);
        WinApi.UnregisterHotkeys(helper.Handle);

        Application.Current.Shutdown();
    }

    private void ShowButton_OnClick(object sender, RoutedEventArgs e)
    {
        Show();
        Activate();
    }

    private void HomeButton_OnClick(object sender, RoutedEventArgs e)
    {
        WebView.Source = WebViewSourceManager.CurrentSource.Uri;
    }
}