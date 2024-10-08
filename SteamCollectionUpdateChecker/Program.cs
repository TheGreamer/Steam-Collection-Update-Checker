﻿using System.Text;
namespace SteamCollectionUpdateChecker;

// 3053765091 2104683692
internal class Program
{
    private static async Task Main()
    {
        string language = Utility.SelectAppLanguage(Constant.SELECT_LANGUAGE);
        Utility.SelectAppTheme(LanguageManager.Translate(Constant.KEY_SELECT_THEME));

        while (true)
        {
            string collectionId = Utility.ValidateCollectionId(LanguageManager.Translate(Constant.KEY_ENTER_COLLECTION_ID), LanguageManager.Translate(Constant.KEY_INVALID_COLLECTION_ID));
            int startDateYear = Utility.ValidateDate(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_YEAR), LanguageManager.Translate(Constant.KEY_INVALID_YEAR), Constant.MIN_YEAR, Constant.MAX_YEAR);
            int startDateMonth = Utility.ValidateDate(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_MONTH), LanguageManager.Translate(Constant.KEY_INVALID_MONTH), Constant.MIN_MONTH, Constant.MAX_MONTH);
            int startDateDay = Utility.ValidateDate(LanguageManager.Translate(Constant.KEY_ENTER_START_DATE_DAY), LanguageManager.Translate(Constant.KEY_INVALID_DAY), Constant.MIN_DAY, Constant.MAX_DAY);
            bool updateAvailableOnly = Utility.GetState(LanguageManager.Translate(Constant.KEY_UPDATE_AVAILABLE_ONLY));
            bool isRedirectionEnabled = Utility.GetState(LanguageManager.Translate(Constant.KEY_IS_REDIRECTION_ENABLED));
            bool includeUpdateNotes = Utility.GetState(LanguageManager.Translate(Constant.KEY_INCLUDE_UPDATE_NOTES));
            var updateInfo = new UpdateInfo(collectionId, startDateYear, startDateMonth, startDateDay, language, updateAvailableOnly, isRedirectionEnabled, includeUpdateNotes);

            await Utility.PauseApp(LanguageManager.Translate(Constant.KEY_PROCESS_STARTING), 5);

            using (var fileWriter = new StreamWriter(Utility.SetFilePathAndFileName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), LanguageManager.Translate(Constant.KEY_UPDATE_CHECK), updateInfo), false, Encoding.UTF8))
            {
                using var multiWriter = new MultiTextWriter(Console.Out, fileWriter);
                Console.SetOut(multiWriter);
                await Scraper.ProcessCollection(updateInfo);
            }

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8)
            {
                AutoFlush = true
            });

            if (!Utility.RestartApp(LanguageManager.Translate(Constant.KEY_RESTART_MESSAGE)))
                break;
        }
    }
}