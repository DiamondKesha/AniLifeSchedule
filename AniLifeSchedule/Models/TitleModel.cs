namespace AniLifeSchedule.Models;

public class TitleModel(string russian, string romaji)
{
    public string Russian { get; set; } = russian;

    public string Romaji { get; set; } = romaji;

    public static TitleModel Create(string russian, string romaji)
    {
        return new(russian, romaji);
    }
}
