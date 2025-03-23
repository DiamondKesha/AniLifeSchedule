using SkiaSharp;
using Topten.RichTextKit;

namespace AniLifeSchedule.Services.ImageCreator;

public static class ImageStyles
{

    public static readonly Style ScheduleTitle = new()
    {
        FontFamily = "Montserrat",
        FontSize = 38f,
        TextColor = SKColors.White,
        LineHeight = 1,
        FontWeight = 800
    };

    public static readonly Style ScheduleInfo = new()
    {
        FontFamily = "JetBrains Mono",
        FontSize = 14.0f,
        TextColor = SKColor.Parse("#c0c0c0"),
        LineHeight = 1.0f,
        FontWeight = 500
    };

    public static readonly Style ScheduleAnimeTitle = new()
    {
        FontFamily = "JetBrains Mono",
        FontSize = 14.0f,
        TextColor = SKColors.White,
        LineHeight = 1.0f,
        FontWeight = 500
    };

    public static readonly Style ScheduleAnimeStatus = new()
    {
        FontFamily = "JetBrains Mono",
        FontSize = 14f,
        TextColor = SKColors.White,
        LineHeight = 1.0f,
        FontWeight = 500
    };
}
