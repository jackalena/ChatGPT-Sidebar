using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;

namespace ChatGPTSidebar.Config;

internal static class Settings
{
    public static SettingsFile Current => currentSettings!;
    public static SettingsFile Default { get; } = SettingsFile.GetDefault();

    public static string BaseSettingsFolder => Path.GetDirectoryName(settingsFilePath)!;
    private static readonly string settingsFilePath = @"%localappdata%\ChatGPTSidebar\settings.json";

    private static SettingsFile? currentSettings;
    private static SettingsFile? lastSavedSettings;

    static Settings()
    {
        settingsFilePath = Environment.ExpandEnvironmentVariables(settingsFilePath);
        Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath)!);
    }

    public static void Load()
    {
        if (File.Exists(settingsFilePath))
            ReadFileAsync().Wait();
        else
        {
            currentSettings = SettingsFile.GetDefault();
            WriteFile();
        }
    }

    public static void Save()
    {
        WriteFile();
    }

    public static async Task SaveAsync()
    {
        await Task.Run(WriteFile);
    }

    private static void WriteFile()
    {
        if (WebViewSourceManager.Sources is null) WebViewSourceManager.Init();

        currentSettings!.Sources = WebViewSourceManager.Sources!.Select(SerializableWebViewSource.FromWebViewSource).ToArray();
        string s = JsonConvert.SerializeObject(currentSettings);
        File.WriteAllText(settingsFilePath, s);
    }

    private static async Task ReadFileAsync()
    {
        string s = await File.ReadAllTextAsync(settingsFilePath);
        try
        {
            currentSettings = JsonConvert.DeserializeObject<SettingsFile>(s)!;
        }
        catch (JsonException e)
        {
            Debug.WriteLine("App data file was invalid and will be restored to the default:");
            Debug.WriteLine(e.Message);
            currentSettings = SettingsFile.GetDefault();
            WriteFile();
        }

        WebViewSourceManager.Init();

        //lastSavedSettings = currentSettings.Clone();
    }
}