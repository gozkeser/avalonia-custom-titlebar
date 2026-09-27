using System;
using Avalonia.Markup.Xaml;
using TitleBarDemo.Services;

namespace TitleBarDemo.MarkupExtensions;

public class SvgExtension : MarkupExtension
{
    public string Path { get; set; } = string.Empty;
    public int Size { get; set; } = 128;

    public SvgExtension() { }
    public SvgExtension(string path) => Path = path;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        string uri = Path.StartsWith("avares://") 
            ? Path 
            : $"avares://TitleBarDemo{(Path.StartsWith('/') ? "" : "/")}{Path}";
        return SvgHelper.LoadSvgBitmap(uri, Size, Size);
    }
}
