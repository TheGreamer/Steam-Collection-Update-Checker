namespace SteamCollectionUpdateChecker;

public static class LanguageManager
{
    private static Dictionary<string, string> _translations;

    public static void SetLanguage(string language)
    {
        if (language == Constant.TR)
        {
            _translations = new Dictionary<string, string>
            {
                { Constant.KEY_CONSOLE_TITLE, Constant.TR_CONSOLE_TITLE },
                { Constant.KEY_SELECT_THEME, Constant.TR_SELECT_THEME },
                { Constant.KEY_ENTER_COLLECTION_ID, Constant.TR_ENTER_COLLECTION_ID },
                { Constant.KEY_INVALID_COLLECTION_ID, Constant.TR_INVALID_COLLECTION_ID },
                { Constant.KEY_ENTER_START_DATE_YEAR, Constant.TR_ENTER_START_DATE_YEAR },
                { Constant.KEY_INVALID_YEAR, Constant.TR_INVALID_YEAR },
                { Constant.KEY_ENTER_START_DATE_MONTH, Constant.TR_ENTER_START_DATE_MONTH },
                { Constant.KEY_INVALID_MONTH, Constant.TR_INVALID_MONTH },
                { Constant.KEY_ENTER_START_DATE_DAY, Constant.TR_ENTER_START_DATE_DAY },
                { Constant.KEY_INVALID_DAY, Constant.TR_INVALID_DAY },
                { Constant.KEY_UPDATE_AVAILABLE_ONLY, Constant.TR_UPDATE_AVAILABLE_ONLY },
                { Constant.KEY_INCLUDE_UPDATE_NOTES, Constant.TR_INCLUDE_UPDATE_NOTES },
                { Constant.KEY_PROCESS_STARTING, Constant.TR_PROCESS_STARTING },
                { Constant.KEY_UPDATE_CHECK, Constant.TR_UPDATE_CHECK },
                { Constant.KEY_RESTART_MESSAGE, Constant.TR_RESTART_MESSAGE },
                { Constant.KEY_TITLE_NOT_FOUND, Constant.TR_TITLE_NOT_FOUND },
                { Constant.KEY_UPDATE_AVAILABLE, Constant.TR_UPDATE_AVAILABLE },
                { Constant.KEY_UPDATED, Constant.TR_UPDATED },
                { Constant.KEY_NOT_UPDATED, Constant.TR_NOT_UPDATED },
                { Constant.KEY_ITEM, Constant.TR_ITEM },
                { Constant.KEY_UPDATE_DATE, Constant.TR_UPDATE_DATE },
                { Constant.KEY_NONE, Constant.TR_NONE },
                { Constant.KEY_DATE, Constant.TR_DATE },
                { Constant.KEY_DESCRIPTION, Constant.TR_DESCRIPTION },
                { Constant.KEY_NO_INFO, Constant.TR_NO_INFO },
                { Constant.KEY_UPDATE_INFO_TEXT_1, Constant.TR_UPDATE_INFO_TEXT_1 },
                { Constant.KEY_UPDATE_INFO_TEXT_2, Constant.TR_UPDATE_INFO_TEXT_2 },
                { Constant.KEY_UPDATE_INFO_TEXT_3, Constant.TR_UPDATE_INFO_TEXT_3 },
                { Constant.KEY_UPDATE_INFO_TEXT_4, Constant.TR_UPDATE_INFO_TEXT_4 },
                { Constant.KEY_UPDATE_INFO_TEXT_5, Constant.TR_UPDATE_INFO_TEXT_5 },
                { Constant.KEY_UPDATE_INFO_TEXT_6, Constant.TR_UPDATE_INFO_TEXT_6 }
            };
        }
        else if (language == Constant.EN)
        {
            _translations = new Dictionary<string, string>
            {
                { Constant.KEY_CONSOLE_TITLE, Constant.EN_CONSOLE_TITLE },
                { Constant.KEY_SELECT_THEME, Constant.EN_SELECT_THEME },
                { Constant.KEY_ENTER_COLLECTION_ID, Constant.EN_ENTER_COLLECTION_ID },
                { Constant.KEY_INVALID_COLLECTION_ID, Constant.EN_INVALID_COLLECTION_ID },
                { Constant.KEY_ENTER_START_DATE_YEAR, Constant.EN_ENTER_START_DATE_YEAR },
                { Constant.KEY_INVALID_YEAR, Constant.EN_INVALID_YEAR },
                { Constant.KEY_ENTER_START_DATE_MONTH, Constant.EN_ENTER_START_DATE_MONTH },
                { Constant.KEY_INVALID_MONTH, Constant.EN_INVALID_MONTH },
                { Constant.KEY_ENTER_START_DATE_DAY, Constant.EN_ENTER_START_DATE_DAY },
                { Constant.KEY_UPDATE_AVAILABLE_ONLY, Constant.EN_UPDATE_AVAILABLE_ONLY },
                { Constant.KEY_INCLUDE_UPDATE_NOTES, Constant.EN_INCLUDE_UPDATE_NOTES },
                { Constant.KEY_PROCESS_STARTING, Constant.EN_PROCESS_STARTING },
                { Constant.KEY_INVALID_DAY, Constant.EN_INVALID_DAY },
                { Constant.KEY_UPDATE_CHECK, Constant.EN_UPDATE_CHECK },
                { Constant.KEY_RESTART_MESSAGE, Constant.EN_RESTART_MESSAGE },
                { Constant.KEY_TITLE_NOT_FOUND, Constant.EN_TITLE_NOT_FOUND },
                { Constant.KEY_UPDATE_AVAILABLE, Constant.EN_UPDATE_AVAILABLE },
                { Constant.KEY_UPDATED, Constant.EN_UPDATED },
                { Constant.KEY_NOT_UPDATED, Constant.EN_NOT_UPDATED },
                { Constant.KEY_ITEM, Constant.EN_ITEM },
                { Constant.KEY_UPDATE_DATE, Constant.EN_UPDATE_DATE },
                { Constant.KEY_NONE, Constant.EN_NONE },
                { Constant.KEY_DATE, Constant.EN_DATE },
                { Constant.KEY_DESCRIPTION, Constant.EN_DESCRIPTION },
                { Constant.KEY_NO_INFO, Constant.EN_NO_INFO },
                { Constant.KEY_UPDATE_INFO_TEXT_1, Constant.EN_UPDATE_INFO_TEXT_1 },
                { Constant.KEY_UPDATE_INFO_TEXT_2, Constant.EN_UPDATE_INFO_TEXT_2 },
                { Constant.KEY_UPDATE_INFO_TEXT_3, Constant.EN_UPDATE_INFO_TEXT_3 },
                { Constant.KEY_UPDATE_INFO_TEXT_4, Constant.EN_UPDATE_INFO_TEXT_4 },
                { Constant.KEY_UPDATE_INFO_TEXT_5, Constant.EN_UPDATE_INFO_TEXT_5 },
                { Constant.KEY_UPDATE_INFO_TEXT_6, Constant.EN_UPDATE_INFO_TEXT_6 }
            };
        }
    }

    public static string Translate(string key)
    {
        return _translations.TryGetValue(key, out string value) ? value : key;
    }
}