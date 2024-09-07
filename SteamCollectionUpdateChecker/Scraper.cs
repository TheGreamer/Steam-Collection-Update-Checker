using HtmlAgilityPack;
namespace SteamCollectionUpdateChecker;

public static class Scraper
{
    public static async Task ProcessCollection(UpdateInfo updateInfo)
    {
        var document = new HtmlDocument();
        await GetCollection(updateInfo, document);
        var subCollections = document.DocumentNode.SelectNodes(Constant.XPATH_SUB_COLLECTION_URLS);

        if (subCollections != null)
        {
            foreach (var subCollection in subCollections)
            {
                updateInfo.CollectionId = subCollection.Attributes[Constant.HREF].Value.Remove(0, 55);
                await GetCollection(updateInfo, new HtmlDocument());
            }
        }
    }

    private static async Task GetCollection(UpdateInfo updateInfo, HtmlDocument document)
    {
        string htmlContent = await Utility.GetHtmlContent(Constant.BASE_URL + updateInfo.CollectionId);
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
                        string updateDetail = itemDetails.Last().InnerText.Trim();
                        isRecentUpdate = updateDetail.IsDateBetween(new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay), DateTime.Now);
                        updateDate = updateDetail.ChangeDateFormat(updateInfo.Language);
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
                        if (!updateInfo.UpdateAvailableOnly)
                            Utility.ColorfulWrite([LanguageManager.Translate(Constant.KEY_UPDATED), LanguageManager.Translate(Constant.KEY_ITEM), $"{title} ({itemSize})", LanguageManager.Translate(Constant.KEY_UPDATE_DATE), $"{updateDate}\n\n"],
                                                  [ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Green]);
                    }
                }
                else
                {
                    if (!updateInfo.UpdateAvailableOnly)
                        Utility.ColorfulWrite([LanguageManager.Translate(Constant.KEY_NOT_UPDATED), LanguageManager.Translate(Constant.KEY_ITEM), $"{title} ({itemSize})", LanguageManager.Translate(Constant.KEY_UPDATE_DATE), $"{LanguageManager.Translate(Constant.KEY_NONE)}\n\n"],
                                              [ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.Red, ConsoleColor.Gray, ConsoleColor.Red]);
                }

                Console.ResetColor();
            }
        }
    }
}