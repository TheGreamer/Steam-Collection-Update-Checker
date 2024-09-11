using System.Globalization;
using System.Text.RegularExpressions;
namespace SteamCollectionUpdateChecker;

public static class Utility
{
    public static string SelectAppLanguage(string text)
    {
        string language = string.Empty;
        Console.Title = string.Empty;
        Console.Write(text);

        switch (Console.ReadKey().KeyChar)
        {
            case '1': LanguageManager.SetLanguage(Constant.EN); language = Constant.EN; break;
            case '2': LanguageManager.SetLanguage(Constant.TR); language = Constant.TR; break;
            default: LanguageManager.SetLanguage(Constant.EN); language = Constant.EN; break;
        }

        return language;
    }

    public static bool IsValidCollectionId(string collectionId)
    {
        if (string.IsNullOrWhiteSpace(collectionId) || !collectionId.All(char.IsDigit) || collectionId.Length < 9 || collectionId.Length > 10)
        {
            return false;
        }
        return true;
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

    public static (bool result, int value) WriteTextAndCheckValidity(string text, string failedText, int checkValue, int minValue, int maxValue)
    {
        Console.Write(text);

        if (!int.TryParse(Console.ReadLine(), out checkValue))
        {
            Console.Write(failedText);
            return (false, checkValue);
        }
        else
        {
            if (checkValue > maxValue || checkValue < minValue)
            {
                Console.Write(failedText);
                return (false, checkValue);
            }

            return (true, checkValue);
        }
    }
}