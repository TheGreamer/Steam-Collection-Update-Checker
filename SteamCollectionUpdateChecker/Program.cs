using HtmlAgilityPack;
using System.Text;
namespace SteamCollectionUpdateChecker;

// 3053765091 2104683692
internal class Program
{
    private static int startDateYear, startDateMonth, startDateDay;
    private static string language = string.Empty;

    private static async Task Main()
    {
        language = Utility.SelectAppLanguage(Constant.SELECT_LANGUAGE);
        Console.Title = LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE);
        Console.Clear();

        Start:
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write(LanguageManager.Translate(Constant.KEY_ENTER_COLLECTION_ID));
        string collectionId = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(collectionId) || !collectionId.All(char.IsDigit))
        {
            Console.Write(LanguageManager.Translate(Constant.KEY_INVALID_COLLECTION_ID));
            goto Start;
        }

        DateYear:
        (bool yearResult, int yearValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_YEAR), LanguageManager.Translate(Constant.KEY_INVALID_YEAR), startDateYear, 2012, DateTime.Now.Year);
        if (!yearResult) goto DateYear;
        else startDateYear = yearValue;

        DateMonth:
        (bool monthResult, int monthValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_MONTH), LanguageManager.Translate(Constant.KEY_INVALID_MONTH), startDateMonth, 1, 12);
        if (!monthResult) goto DateMonth;
        else startDateMonth = monthValue;

        DateDay:
        (bool dayResult, int dayValue) = Utility.WriteTextAndCheckValidity(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_DAY), LanguageManager.Translate(Constant.KEY_INVALID_DAY), startDateDay, 1, 30);
        if (!dayResult) goto DateDay;
        else startDateDay = dayValue;

        bool updateAvailableOnly = Utility.IsOnlyUpdateAvailable(LanguageManager.Translate(Constant.KEY_UPDATE_AVAILABLE_ONLY));
        Console.Write(LanguageManager.Translate(Constant.KEY_PROCESS_STARTING));
        using (var fileWriter = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{collectionId} ({startDateDay}.{startDateMonth}.{startDateYear} - {DateTime.Now:d}) - {LanguageManager.Translate(Constant.KEY_UPDATE_CHECK)}.txt"), false, Encoding.UTF8))
        {
            using var multiWriter = new MultiTextWriter(Console.Out, fileWriter);
            Console.SetOut(multiWriter);
            await ProcessCollection(collectionId, updateAvailableOnly);
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

    private static async Task ProcessCollection(string collectionId, bool updateAvailableOnly)
    {
        var document = new HtmlDocument();
        await GetCollection(collectionId, updateAvailableOnly, document);
        var subCollections = document.DocumentNode.SelectNodes(Constant.XPATH_SUB_COLLECTION_URLS);

        if (subCollections != null)
        {
            foreach (var subCollection in subCollections)
            {
                string subCollectionId = subCollection.Attributes[Constant.HREF].Value.Remove(0, 55);
                await GetCollection(subCollectionId, updateAvailableOnly, new HtmlDocument());
            }
        }
    }

    private static async Task GetCollection(string collectionId, bool updateAvailableOnly, HtmlDocument document)
    {
        string htmlContent = await Utility.GetHtmlContent(Constant.BASE_URL + collectionId);
        document.LoadHtml(htmlContent);
        var collectionItems = document.DocumentNode.SelectNodes(Constant.XPATH_COLLECTION_ITEMS);

        if (collectionItems != null)
        {
            foreach (var item in collectionItems)
            {
                string itemUrl = item.Attributes[Constant.HREF].Value;

                if (itemUrl.Contains(Constant.HASH) || !itemUrl.Contains(Constant.URL_FILE_DETAILS))
                    continue;

                Restart:
                string itemHtmlContent = string.Empty;
                try
                {
                    itemHtmlContent = await Utility.GetHtmlContent(itemUrl);
                }
                catch (HttpRequestException)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(120));
                    goto Restart;
                }

                var itemDocument = new HtmlDocument();
                itemDocument.LoadHtml(itemHtmlContent);
                var titleNode = itemDocument.DocumentNode.SelectSingleNode(Constant.XPATH_ITEM_TITLE);
                string title = titleNode?.InnerText.Trim() ?? LanguageManager.Translate(Constant.KEY_TITLE_NOT_FOUND);

                var itemDetails = itemDocument.DocumentNode.SelectNodes(Constant.XPATH_ITEM_UPDATE_DATE);
                string itemSize = string.Empty;
                string updateDate = null;
                bool isRecentUpdate = false;

                if (itemDetails != null)
                {
                    itemSize = itemDetails.First().InnerText.Trim();

                    if (itemDetails.Count > 2)
                    {
                        string updateInfo = itemDetails.Last().InnerText.Trim();
                        isRecentUpdate = updateInfo.IsDateBetween(new DateTime(startDateYear, startDateMonth, startDateDay), DateTime.Now);
                        updateDate = updateInfo.ChangeDateFormat(language);
                    }
                }

                if (updateDate != null)
                {
                    if (isRecentUpdate)
                    {
                        Utility.ColorfulWrite([LanguageManager.Translate(Constant.KEY_UPDATE_AVAILABLE), LanguageManager.Translate(Constant.KEY_ITEM), $"{title} ({itemSize})", LanguageManager.Translate(Constant.KEY_UPDATE_DATE), $"{updateDate}\n\n"],
                                              [ConsoleColor.White, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Yellow]);
                    }
                    else
                    {
                        if (!updateAvailableOnly)
                            Utility.ColorfulWrite([LanguageManager.Translate(Constant.KEY_UPDATED), LanguageManager.Translate(Constant.KEY_ITEM), $"{title} ({itemSize})", LanguageManager.Translate(Constant.KEY_UPDATE_DATE), $"{updateDate}\n\n"],
                                                  [ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Green]);
                    }
                }
                else
                {
                    if (!updateAvailableOnly)
                        Utility.ColorfulWrite([LanguageManager.Translate(Constant.KEY_NOT_UPDATED), LanguageManager.Translate(Constant.KEY_ITEM), $"{title} ({itemSize})", LanguageManager.Translate(Constant.KEY_UPDATE_DATE), $"{LanguageManager.Translate(Constant.KEY_NONE)}\n\n"],
                                              [ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.Red, ConsoleColor.Gray, ConsoleColor.Red]);
                }

                Console.ResetColor();
            }
        }
    }
}