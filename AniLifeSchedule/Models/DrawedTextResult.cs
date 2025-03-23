namespace AniLifeSchedule.Models;

public class DrawedTextResult(float height, int lines)
{
    public float Height { get; private set; } = height;

    public int Lines { get; private set; } = lines;

    public static DrawedTextResult Create(float height, int lines)
    {
        return new DrawedTextResult(height, lines);
    }
}
