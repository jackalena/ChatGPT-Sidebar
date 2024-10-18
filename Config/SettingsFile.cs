using System.Reflection;

namespace ChatGPTSidebar.Config;

internal class SettingsFile
{
    public int LastWindowWidth;
    public int WebViewSource;

    public SerializableWebViewSource[]? Sources = [];

    private static readonly FieldInfo[] fields = typeof(SettingsFile).GetFields();

    public static SettingsFile GetDefault()
    {
        return new() { LastWindowWidth = 400, WebViewSource = 0, Sources = WebViewSourceManager.DefaultSources, };
    }

    public override bool Equals(object? obj)
    {
        if (obj?.GetType() != GetType()) return false;

        for (int i = 0; i < fields.Length; i++)
            if (fields[i].GetValue(this)?.Equals(fields[i].GetValue(obj)) != true)
                return false;

        return true;
    }

    public SettingsFile Clone()
    {
        SettingsFile file = new();

        foreach (FieldInfo field in fields) field.SetValue(file, field.GetValue(this));

        return file;
    }
}