using System.Text;
namespace SteamCollectionUpdateChecker;

// 3053765091 2104683692
internal class Program
{
    private static async Task Main()
    {
        string language = Utility.SelectAppLanguage(Constant.SELECT_LANGUAGE);
        Console.Title = LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE);
        Console.Clear();

        Start:
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write(LanguageManager.Translate(Constant.KEY_ENTER_COLLECTION_ID));
        string collectionId = Console.ReadLine();

        if (!Utility.IsValidCollectionId(collectionId))
        {
            Console.Write(LanguageManager.Translate(Constant.KEY_INVALID_COLLECTION_ID));
            goto Start;
        }

        DateYear:
        int startDateYear = 0;
        (bool yearResult, int yearValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_YEAR), LanguageManager.Translate(Constant.KEY_INVALID_YEAR), startDateYear, 2012, DateTime.Now.Year);
        if (!yearResult) goto DateYear;
        else startDateYear = yearValue;

        DateMonth:
        int startDateMonth = 0;
        (bool monthResult, int monthValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_MONTH), LanguageManager.Translate(Constant.KEY_INVALID_MONTH), startDateMonth, 1, 12);
        if (!monthResult) goto DateMonth;
        else startDateMonth = monthValue;

        DateDay:
        int startDateDay = 0;
        (bool dayResult, int dayValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_DAY), LanguageManager.Translate(Constant.KEY_INVALID_DAY), startDateDay, 1, 30);
        if (!dayResult) goto DateDay;
        else startDateDay = dayValue;

        bool updateAvailableOnly = Utility.IsOnlyUpdateAvailable(LanguageManager.Translate(Constant.KEY_UPDATE_AVAILABLE_ONLY));
        Console.Write(LanguageManager.Translate(Constant.KEY_PROCESS_STARTING));
        using (var fileWriter = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{collectionId} ({startDateDay}.{startDateMonth}.{startDateYear} - {DateTime.Now:d}) - {LanguageManager.Translate(Constant.KEY_UPDATE_CHECK)}.txt"), false, Encoding.UTF8))
        {
            using var multiWriter = new MultiTextWriter(Console.Out, fileWriter);
            Console.SetOut(multiWriter);
            var updateInfo = new UpdateInfo(collectionId, startDateYear, startDateMonth, startDateDay, language, updateAvailableOnly);
            await Scraper.ProcessCollection(updateInfo);
        }

        var standardOutput = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8);
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        Console.Write(LanguageManager.Translate(Constant.KEY_RESTART_MESSAGE));
        if (Console.ReadKey().Key == ConsoleKey.Enter)
        {
            Console.Clear();
            goto Start;
        }
    }
}