using SkiaSharp;
using Topten.RichTextKit;

namespace AniLifeSchedule.Services.ImageCreator;

public class ImageCreator : IDisposable
{
    private SKBitmap bitmap { get; set; } = default!;
    private SKCanvas canvas { get; set; } = default!;
    private SKPaint paint { get; set; } = default!;

    public ImageCreator(int width, int height)
    {
        bitmap = new(width, height);
        canvas = new(bitmap);
        paint = new();
    }

    public void ChangeColorPaint(string hexColor)
    {
        paint.Color = SKColor.Parse(hexColor);
    }

    public void ChangeColorPaint(SKColor color)
    {
        paint.Color = color;
    }

    public void DrawSKRect(SKRect rect)
    {
        canvas.DrawRect(rect, paint);
    }

    public DrawedTextResult DrawText(string text, Style textStyle, TextAlignment textAlignment, SKPoint point, float maxWidth, int maxLines = 1)
    {
        TextBlock textBlock = new() { Alignment = textAlignment, MaxWidth = maxWidth, MaxLines = maxLines };

        textBlock.AddText(text, textStyle);
        textBlock.Paint(canvas, point);

        return DrawedTextResult.Create(textBlock.MeasuredHeight, textBlock.LineCount);
    }

    public void ResizeAndDrawImage(SKImage image, int resizedWidth, int resizedHeight, SKPoint position)
    {
        if (image == null) return;

        using SKBitmap bitmap = SKBitmap.FromImage(image);
        using SKBitmap resizedBitmap = bitmap.Resize(new SKImageInfo(resizedWidth, resizedHeight), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Nearest));

        canvas.DrawBitmap(resizedBitmap, position, null);
    }

    public string Save(string pathToSave, string filename)
    {
        using SKImage outputImage = SKImage.FromBitmap(bitmap);
        using SKData bitmapData = outputImage.Encode(SKEncodedImageFormat.Jpeg, 100);

        using var fileStream = File.OpenWrite($"{pathToSave}\\{filename}.jpg");
        bitmapData.SaveTo(fileStream);

        return Convert.ToBase64String(bitmapData.ToArray());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}