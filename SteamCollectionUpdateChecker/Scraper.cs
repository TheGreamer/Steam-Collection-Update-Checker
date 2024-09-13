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

                string itemHtmlContent = await Utility.GetHtmlContent(itemUrl);
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
                        var startDate = new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay);
                        isRecentUpdate = updateDetail.IsDateBetween(startDate, DateTime.Now);
                        updateDate = updateDetail.ChangeDateFormat(updateInfo.Language);
                    }
                }

                var colors = Utility.GetThemeColors();

                if (updateDate != null)
                {
                    if (isRecentUpdate)
                    {
                        Utility.ColorfulWrite(
                        [
                            new(colors[0], LanguageManager.Translate(Constant.KEY_UPDATE_AVAILABLE)),
                            new(colors[1], LanguageManager.Translate(Constant.KEY_ITEM)),
                            new(colors[2], $"{title} ({itemSize})"),
                            new(colors[1], LanguageManager.Translate(Constant.KEY_UPDATE_DATE)),
                            new(colors[2], $"{updateDate}\n\n")
                        ]);

                        if (updateInfo.IncludeUpdateNotes)
                        {
                            string itemId = itemDocument.DocumentNode.SelectSingleNode(Constant.XPATH_ITEM_UPDATE_NOTE_URL).Attributes[Constant.HREF].Value.Remove(0, 61);
                            int noteCount = itemDocument.DocumentNode.SelectSingleNode(Constant.XPATH_UPDATE_NOTE_COUNT).InnerText.GetNumericValue();
                            var startDate = new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay);
                            var updateNotes = await GetUpdateNotes(itemId, noteCount, startDate);

                            if (updateNotes != null)
                            {
                                var updateTitles = updateNotes.Keys.ToList();
                                var updateDescriptions = updateNotes.Values.ToList();

                                for (int i = 0; i < updateNotes.Count; i++)
                                {
                                    Utility.ColorfulWrite(
                                    [
                                        new(colors[0], (i + 1).ToString() + ") "),
                                        new(colors[1], LanguageManager.Translate(Constant.KEY_DATE)),
                                        new(colors[2], $"{updateTitles[i].ChangeDateFormat(updateInfo.Language)}\n"),
                                        new(colors[1], $"   {LanguageManager.Translate(Constant.KEY_DESCRIPTION)}"),
                                        new(colors[2], $"{(string.IsNullOrWhiteSpace(updateDescriptions[i]) ? LanguageManager.Translate(Constant.KEY_NO_INFO) : updateDescriptions[i])}\n\n")
                                    ]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!updateInfo.UpdateAvailableOnly)
                        {
                            Utility.ColorfulWrite(
                            [
                                new(colors[0], LanguageManager.Translate(Constant.KEY_UPDATED)),
                                new(colors[3], LanguageManager.Translate(Constant.KEY_ITEM)),
                                new(colors[4], $"{title} ({itemSize})"),
                                new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_DATE)),
                                new(colors[4], $"{updateDate}\n\n")
                            ]);
                        }
                    }
                }
                else
                {
                    if (!updateInfo.UpdateAvailableOnly)
                    {
                        Utility.ColorfulWrite(
                        [
                            new(colors[0], LanguageManager.Translate(Constant.KEY_NOT_UPDATED)),
                            new(colors[5], LanguageManager.Translate(Constant.KEY_ITEM)),
                            new(colors[6], $"{title} ({itemSize})"),
                            new(colors[5], LanguageManager.Translate(Constant.KEY_UPDATE_DATE)),
                            new(colors[6], $"{LanguageManager.Translate(Constant.KEY_NONE)}\n\n")
                        ]);
                    }
                }

                Console.ForegroundColor = colors[0];
            }
        }
    }

    private static async Task<Dictionary<string, string>> GetUpdateNotes(string itemId, int noteCount, DateTime startDate)
    {
        var allUpdateNotes = new Dictionary<string, string>();

        int pageCount = noteCount switch
        {
            >= 10 => noteCount % 10 == 0 ? noteCount / 10 : (noteCount / 10) + 1,
            _ => 1
        };

        for (int i = 1; i <= pageCount; i++)
        {
            string htmlContent = await Utility.GetHtmlContent($"{Constant.UPDATE_NOTES_URL}{itemId}?p={i}");
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var updateNoteTitles = document.DocumentNode.SelectNodes(Constant.XPATH_UPDATE_NOTE_TITLES);
            var updateNoteDescriptions = document.DocumentNode.SelectNodes(Constant.XPATH_UPDATE_NOTE_DESCRIPTIONS);

            var titles = updateNoteTitles.Select(title => title.InnerText.Trim().Remove(0, 8)).ToList();
            var descriptions = updateNoteDescriptions.Select(description => description.InnerText.Trim()).ToList();
            var updateNotes = titles.Zip(descriptions, (title, description) => (title, description));
            bool state = false;

            foreach (var (title, description) in updateNotes)
            {
                if (title.IsDateBetween(startDate, DateTime.Now))
                {
                    try { allUpdateNotes.Add(title, description); }
                    catch (Exception) { continue; }
                }
                else
                {
                    state = true;
                    break;
                }
            }

            if (state)
                break;
        }

        return allUpdateNotes;
    }
}