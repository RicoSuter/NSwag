using System.IO;
using System.Text.Json;

namespace NSwagStudio.Helpers;

/// <summary>Simple JSON-based application settings replacement for MyToolkit.Storage.ApplicationSettings.</summary>
public static class ApplicationSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NSwagStudio",
        "settings.json");

    private static Dictionary<string, JsonElement>? _cache;

    private static Dictionary<string, JsonElement> LoadAll()
    {
        if (_cache != null)
            return _cache;

        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                _cache = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                         ?? new Dictionary<string, JsonElement>();
                return _cache;
            }
        }
        catch
        {
            // ignore corrupt settings
        }

        _cache = new Dictionary<string, JsonElement>();
        return _cache;
    }

    private static void SaveAll()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // ignore write errors
        }
    }

    public static T GetSetting<T>(string key, T defaultValue)
    {
        var all = LoadAll();
        if (all.TryGetValue(key, out var element))
        {
            try
            {
                var result = element.Deserialize<T>();
                if (result != null)
                    return result;
            }
            catch
            {
                // ignore deserialization errors
            }
        }
        return defaultValue;
    }

    public static void SetSetting<T>(string key, T value)
    {
        var all = LoadAll();
        all[key] = JsonSerializer.SerializeToElement(value);
        SaveAll();
    }
}
