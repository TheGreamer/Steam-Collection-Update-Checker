using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace SteamCollectionUpdateChecker;

public static class Utility
{
    public static string SelectAppLanguage(string text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Title = string.Empty;
        Console.Write(text);

        string language;

        switch (Console.ReadKey().KeyChar)
        {
            case '1': LanguageManager.SetLanguage(Constant.EN); language = Constant.EN; break;
            case '2': LanguageManager.SetLanguage(Constant.TR); language = Constant.TR; break;
            default: LanguageManager.SetLanguage(Constant.EN); language = Constant.EN; break;
        }

        Console.Title = LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE);
        Console.Clear();

        return language;
    }

    public static void SelectAppTheme(string text)
    {
        Console.Write(text);

        var backgroundColor = Console.ReadKey().KeyChar switch
        {
            '1' => ConsoleColor.Black,
            '2' => ConsoleColor.White,
            _ => ConsoleColor.Black,
        };

        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = Console.BackgroundColor == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.White;
        Console.Clear();
    }

    public static string ValidateCollectionId(string text, string failedText)
    {
        string collectionId;

        while (true)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write(text);
            collectionId = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(collectionId) && collectionId.All(char.IsDigit) && collectionId.Length >= 9 && collectionId.Length <= 10)
                return collectionId;

            Console.Write(failedText);
        }
    }

    public static int ValidateDate(string text, string failedText, int minValue, int maxValue)
    {
        int checkValue = 0;
        bool validInput = false;

        while (!validInput)
        {
            Console.Write(text);

            if (!int.TryParse(Console.ReadLine(), out checkValue))
                Console.Write(failedText);
            else if (checkValue < minValue || checkValue > maxValue)
                Console.Write(failedText);
            else
                validInput = true;
        }

        return checkValue;
    }

    public static bool GetState(string text)
    {
        Console.Write(text);

        bool state = Console.ReadKey().KeyChar switch
        {
            '1' => true,
            '2' => false,
            _ => false,
        };

        Console.Write("\n");
        return state;
    }

    public static async Task PauseApp(string text, int seconds)
    {
        Console.Write(text);
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

    public static void WriteUpdateInfo(UpdateInfo updateInfo)
    {
        Console.Clear();
        var colors = GetThemeColors();

        ColorfulWrite(
        [
            new(colors[0], $"{LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE).ToUpper()}\n\n"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_1)),
            new(colors[4], updateInfo.CollectionId),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_2)),
            new(colors[4], $"{new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay):d}"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_3)),
            new(colors[4], $"{(updateInfo.UpdateAvailableOnly ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_5) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6))}"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_4)),
            new(colors[4], $"{(updateInfo.IncludeUpdateNotes ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_5) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6))}"),
            new(colors[0], "\n\n-----------------------------------------------------\n\n")
        ]);
    }

    public static ConsoleColor[] GetThemeColors()
    {
        bool theme = Console.BackgroundColor.Equals(ConsoleColor.Black);

        ConsoleColor whiteBlack = theme ? ConsoleColor.White : ConsoleColor.Black;
        ConsoleColor magenta = theme ? ConsoleColor.Magenta : ConsoleColor.DarkMagenta;
        ConsoleColor yellow = theme ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
        ConsoleColor cyan = theme ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
        ConsoleColor green = theme ? ConsoleColor.Green : ConsoleColor.DarkGreen;
        ConsoleColor gray = theme ? ConsoleColor.Gray : ConsoleColor.DarkGray;
        ConsoleColor red = theme ? ConsoleColor.Red : ConsoleColor.DarkRed;

        return [whiteBlack, magenta, yellow, cyan, green, gray, red];
    }

    public static int GetNumericValue(this string text)
    {
        return int.TryParse(Regex.Replace(text, @"\D", ""), out int result) ? result : 0;
    }

    public static async Task<string> GetHtmlContent(string url)
    {
        string htmlContent;

        while (true)
        {
            try
            {
                using HttpResponseMessage response = await new HttpClient().GetAsync(url);
                response.EnsureSuccessStatusCode();
                htmlContent = await response.Content.ReadAsStringAsync();
                break;
            }
            catch (HttpRequestException)
            {
                await Task.Delay(TimeSpan.FromSeconds(120));
            }
        }

        return htmlContent;
    }

    public static void ColorfulWrite(ColorfulText[] texts)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            Console.ForegroundColor = texts[i].Color;
            Console.Write(texts[i].Text);
        }
    }

    public static string ChangeDateFormat(this string originalDate, string language)
    {
        return TryParseExactDates(originalDate, out DateTime date)
            ? language.Equals(Constant.TR)
                ? date.ToString(Constant.TR_DEFAULT_DATE, new CultureInfo(Constant.TR_DEFAULT_CULTURE_INFO))
                : date.ToString(Constant.EN_DEFAULT_DATE, new CultureInfo(Constant.EN_DEFAULT_CULTURE_INFO))
            : originalDate;
    }

    public static bool IsDateBetween(this string originalDate, DateTime startDate, DateTime endDate)
    {
        return TryParseExactDates(originalDate, out DateTime date)
            ? date >= startDate && date <= endDate
            : false;
    }

    private static bool TryParseExactDates(string originalDate, out DateTime date)
    {
        return DateTime.TryParseExact(originalDate, Constant.DATE_VARIANT_1, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
               DateTime.TryParseExact(originalDate, Constant.DATE_VARIANT_2, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
               DateTime.TryParseExact(originalDate, Constant.DATE_VARIANT_3, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
               DateTime.TryParseExact(originalDate, Constant.DATE_VARIANT_4, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    public static bool RestartApp(string text)
    {
        Console.Write(text);

        if (Console.ReadKey().Key.Equals(ConsoleKey.Enter))
        {
            Console.Clear();
            return true;
        }

        return false;
    }
}