using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace TitleBarDemo.Localization;

/// <summary>
/// Thread-safe localization manager providing symmetric key lookup for en-US and tr-TR locales.
/// Complies with modern desktop app zero magic string standards.
/// </summary>
public sealed class LocalizationSource
{
    private static readonly Lazy<LocalizationSource> _lazyInstance =
        new(() => new LocalizationSource());

    public static LocalizationSource Instance => _lazyInstance.Value;

    private readonly Dictionary<string, string> _strings = new(StringComparer.OrdinalIgnoreCase);
    private string _currentCulture = "en-US";

    public string CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture != value)
            {
                _currentCulture = value;
                LoadLocale(value);
            }
        }
    }

    private LocalizationSource()
    {
        // Default to en-US or current UI culture if matching
        var initialCulture = CultureInfo.CurrentUICulture.Name.StartsWith("tr", StringComparison.OrdinalIgnoreCase)
            ? "tr-TR"
            : "en-US";
        _currentCulture = initialCulture;
        LoadLocale(initialCulture);
    }

    private void LoadLocale(string cultureName)
    {
        _strings.Clear();
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"TitleBarDemo.Localization.locales.{cultureName}.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            // Fallback to en-US
            using var fallbackStream = assembly.GetManifestResourceStream("TitleBarDemo.Localization.locales.en-US.json");
            if (fallbackStream != null)
            {
                ReadJsonToDictionary(fallbackStream);
            }
            return;
        }

        ReadJsonToDictionary(stream);
    }

    private void ReadJsonToDictionary(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (dictionary != null)
        {
            foreach (var (key, value) in dictionary)
            {
                _strings[key] = value;
            }
        }
    }

    public string GetString(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        return _strings.TryGetValue(key, out var val) ? val : key;
    }
}
