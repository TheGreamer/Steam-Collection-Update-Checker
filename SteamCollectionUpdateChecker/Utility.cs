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

        return language;
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

            Console.WriteLine(failedText);
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

    public static void PauseApp(string text, int seconds)
    {
        Console.Write(text);
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }

    public static void WriteUpdateInfo(UpdateInfo updateInfo)
    {
        Console.Clear();
        ColorfulWrite([$"{LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE).ToUpper()}\n\n", LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_1), updateInfo.CollectionId, LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_2), $"{new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay):d}", LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_3), $"{(updateInfo.UpdateAvailableOnly ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_5) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6))}", LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_4), $"{(updateInfo.IncludeUpdateNotes ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_5) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6))}", "\n\n-----------------------------------------------------\n\n"],
                      [ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.White]);
    }

    public static int GetNumericValue(this string text)
    {
        return int.TryParse(Regex.Replace(text, @"\D", ""), out int result) ? result : 0;
    }

    public static async Task<string> GetHtmlContent(string url)
    {
        using HttpResponseMessage response = await new HttpClient().GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static void ColorfulWrite(string[] texts, ConsoleColor[] colors)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            Console.ForegroundColor = colors[i];
            Console.Write(texts[i]);
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