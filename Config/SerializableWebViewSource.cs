namespace ChatGPTSidebar.Config;

public class SerializableWebViewSource
{
    public string DisplayName;
    public string Uri;
    public SerializableColor BackgroundColor;
    public SerializableColor TextColor;
    public bool IsCustom;

    public SerializableWebViewSource() { }

    public SerializableWebViewSource(string uri, string displayName, SerializableColor backgroundColor, SerializableColor textColor,
                                     bool isCustom = false)
    {
        DisplayName = displayName;
        Uri = uri;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        IsCustom = isCustom;
    }

    public static WebViewSource GetWebViewSource(SerializableWebViewSource s)
    {
        return new(s.Uri, s.DisplayName, SerializableColor.GetColor(s.BackgroundColor), SerializableColor.GetColor(s.TextColor));
    }

    public static SerializableWebViewSource FromWebViewSource(WebViewSource s)
    {
        return new(s.Uri.ToString(),
                   s.DisplayName,
                   SerializableColor.FromColor(s.BackgroundColor),
                   SerializableColor.FromColor(s.TextColor),
                   s.IsCustom);
    }
}