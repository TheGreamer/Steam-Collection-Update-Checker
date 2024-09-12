namespace SteamCollectionUpdateChecker;

public class ColorfulText(ConsoleColor color, string text)
{
    public ConsoleColor Color { get; set; } = color;
    public string Text { get; set; } = text;
}