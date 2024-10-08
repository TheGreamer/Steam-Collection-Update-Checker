﻿using HtmlAgilityPack;
namespace SteamCollectionUpdateChecker;

public static class Scraper
{
    private static readonly ConsoleColor[] colors = Utility.GetThemeColors();

    public static async Task ProcessCollection(UpdateInfo updateInfo)
    {
        Console.Clear();
        Utility.ColorfulWrite(
        [
            new(colors[0], $"{LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE).ToUpper()}\n\n"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_1)),
            new(colors[4], updateInfo.CollectionId),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_2)),
            new(colors[4], $"{new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay):d}"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_3)),
            new(colors[4], $"{(updateInfo.UpdateAvailableOnly ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_7))}"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_4)),
            new(colors[4], $"{(updateInfo.IsRedirectionEnabled ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_7))}"),
            new(colors[3], LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_5)),
            new(colors[4], $"{(updateInfo.IncludeUpdateNotes ? LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_6) : LanguageManager.Translate(Constant.KEY_UPDATE_INFO_TEXT_7))}"),
            new(colors[0], $"\n\n{new('-', LanguageManager.Translate(Constant.KEY_CONSOLE_TITLE).Length)}\n\n")
        ]);

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

                        if (updateInfo.IsRedirectionEnabled)
                            Utility.RedirectTo(itemUrl);

                        if (updateInfo.IncludeUpdateNotes)
                        {
                            string itemId = itemDocument.DocumentNode.SelectSingleNode(Constant.XPATH_ITEM_UPDATE_NOTE_URL).Attributes[Constant.HREF].Value.Remove(0, 61);
                            int noteCount = itemDocument.DocumentNode.SelectSingleNode(Constant.XPATH_UPDATE_NOTE_COUNT).InnerText.GetNumericValue();
                            var startDate = new DateTime(updateInfo.StartDateYear, updateInfo.StartDateMonth, updateInfo.StartDateDay);
                            var updateNotes = await GetUpdateNotes(itemId, noteCount, startDate);

                            if (updateNotes != null)
                            {
                                for (int i = 0; i < updateNotes.Count; i++)
                                {
                                    Utility.ColorfulWrite(
                                    [
                                        new(colors[0], (i + 1).ToString() + ") "),
                                        new(colors[1], LanguageManager.Translate(Constant.KEY_DATE)),
                                        new(colors[2], $"{updateNotes[i].Title.ChangeDateFormat(updateInfo.Language)}\n"),
                                        new(colors[1], $"   {LanguageManager.Translate(Constant.KEY_DESCRIPTION)}"),
                                        new(colors[2], $"{(string.IsNullOrWhiteSpace(updateNotes[i].Description) ? LanguageManager.Translate(Constant.KEY_NO_INFO) : updateNotes[i].Description)}\n\n")
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

    private static async Task<List<UpdateNote>> GetUpdateNotes(string itemId, int noteCount, DateTime startDate)
    {
        var allUpdateNotes = new List<UpdateNote>();

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
            var updateNotes = titles.Zip(descriptions, (title, description) => new UpdateNote(title, description)).ToList();
            bool state = false;
            
            foreach (var updateNote in updateNotes)
            {
                if (updateNote.Title.IsDateBetween(startDate, DateTime.Now))
                {
                    allUpdateNotes.Add(new(updateNote.Title, updateNote.Description));
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