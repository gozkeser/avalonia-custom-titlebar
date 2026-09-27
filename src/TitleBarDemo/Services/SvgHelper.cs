using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Svg.Skia;

namespace TitleBarDemo.Services;

public static class SvgHelper
{
    public static Bitmap LoadSvgBitmap(string avaresUri, int width, int height)
    {
        using var stream = AssetLoader.Open(new Uri(avaresUri));
        var svg = new SKSvg();
        svg.Load(stream);

        using var skBitmap = new SkiaSharp.SKBitmap(width, height);
        using var canvas = new SkiaSharp.SKCanvas(skBitmap);
        canvas.Clear(SkiaSharp.SKColors.Transparent);

        if (svg.Picture != null)
        {
            float scaleX = (float)width / svg.Picture.CullRect.Width;
            float scaleY = (float)height / svg.Picture.CullRect.Height;
            float scale = Math.Min(scaleX, scaleY);
            canvas.Scale(scale);
            canvas.DrawPicture(svg.Picture);
            canvas.Flush();
        }

        using var image = SkiaSharp.SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream(data.ToArray());
        return new Bitmap(ms);
    }
}
