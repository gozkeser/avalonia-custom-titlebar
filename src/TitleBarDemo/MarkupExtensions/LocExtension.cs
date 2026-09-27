using System;
using Avalonia.Markup.Xaml;
using TitleBarDemo.Localization;

namespace TitleBarDemo.MarkupExtensions;

/// <summary>
/// Avalonia XAML markup extension providing localized strings from symmetric locale dictionaries.
/// Supports single key lookup, string formatting, and multi-key composite formatting.
/// Usage:
///   Text="{loc:Loc App.Title}"
///   Text="{loc:Loc shell.demo.check_drag, StringFormat='• {0}'}"
///   Text="{loc:Loc Keys='Company.Division,Company.Name', Format='{0}, {1}'}"
/// </summary>
public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;
    public string? Keys { get; set; }
    public string? StringFormat { get; set; }
    public string? Format { get; set; }

    public LocExtension() { }

    public LocExtension(string key)
    {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var fmt = Format ?? StringFormat;

        if (!string.IsNullOrWhiteSpace(Keys))
        {
            var parts = Keys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var values = new object[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                values[i] = LocalizationSource.Instance.GetString(parts[i]);
            }

            return !string.IsNullOrEmpty(fmt)
                ? string.Format(fmt, values)
                : string.Join(", ", values);
        }

        var value = LocalizationSource.Instance.GetString(Key);
        if (!string.IsNullOrEmpty(fmt))
        {
            return string.Format(fmt, value);
        }

        return value;
    }
}
