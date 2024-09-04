﻿namespace SteamCollectionUpdateChecker;

public static class Constant
{
    public const char HASH = '#';
    public const string EN = "en";
    public const string TR = "tr";
    public const string HREF = "href";
    public const string URL_FILE_DETAILS = "/filedetails/?id=";
    public const string XPATH_COLLECTION_ITEMS = "//div[@class='collectionItem']//div[@class='workshopItem']/a";
    public const string XPATH_SUB_COLLECTION_URLS = "//div[@class='collections']//div[@class='workshopItem']/a[2]";
    public const string XPATH_ITEM_TITLE = "//div[@class='workshopItemTitle']";
    public const string XPATH_ITEM_UPDATE_DATE = "//div[@class='detailsStatRight']";
    public const string BASE_URL = "https://steamcommunity.com/sharedfiles/filedetails/?id=";

    public const string EN_DEFAULT_CULTURE_INFO = "en-US";
    public const string TR_DEFAULT_CULTURE_INFO = "tr-TR";
    public const string EN_DEFAULT_DATE = "f";
    public const string TR_DEFAULT_DATE = "d MMMM yyyy - HH:mm";
    public const string DATE_VARIANT_1 = "d MMM, yyyy @ h:mmtt";
    public const string DATE_VARIANT_2 = "dd MMM, yyyy @ h:mmtt";
    public const string DATE_VARIANT_3 = "d MMM @ h:mmtt";
    public const string DATE_VARIANT_4 = "dd MMM @ h:mmtt";

    public const string KEY_CONSOLE_TITLE = "ConsoleTitle";
    public const string KEY_ENTER_COLLECTION_ID = "EnterCollectionId";
    public const string KEY_INVALID_COLLECTION_ID = "InvalidCollectionId";
    public const string KEY_ENTER_START_DATE_YEAR = "EnterStartDateYear";
    public const string KEY_INVALID_YEAR = "InvalidYear";
    public const string KEY_ENTER_START_DATE_MONTH = "EnterStartDateMonth";
    public const string KEY_INVALID_MONTH = "InvalidMonth";
    public const string KEY_ENTER_START_DATE_DAY = "EnterStartDateDay";
    public const string KEY_INVALID_DAY = "InvalidDay";
    public const string KEY_UPDATE_AVAILABLE_ONLY = "UpdateAvailableOnly";
    public const string KEY_PROCESS_STARTING = "ProcessStarting";
    public const string KEY_UPDATE_CHECK = "UpdateCheck";
    public const string KEY_RESTART_MESSAGE = "RestartMessage";
    public const string KEY_TITLE_NOT_FOUND = "TitleNotFound";
    public const string KEY_UPDATE_AVAILABLE = "UpdateAvailable";
    public const string KEY_UPDATED = "Updated";
    public const string KEY_NOT_UPDATED = "NotUpdated";
    public const string KEY_ITEM = "Item";
    public const string KEY_UPDATE_DATE = "UpdateDate";
    public const string KEY_NONE = "None";

    public const string EN_CONSOLE_TITLE = "Update Checker For Steam Workshop Collections";
    public const string EN_ENTER_COLLECTION_ID = "Steam Workshop Collection ID : ";
    public const string EN_INVALID_COLLECTION_ID = "You must specify a steam workshop collection ID. Restarting the process.\n";
    public const string EN_ENTER_START_DATE_YEAR = "Minimum Update Year (e.g. 2023) : ";
    public const string EN_INVALID_YEAR = "You must specify a minimum update year. Restarting the process.\n";
    public const string EN_ENTER_START_DATE_MONTH = "Minimum Update Month (e.g. 6) : ";
    public const string EN_INVALID_MONTH = "You must specify a minimum update month. Restarting the process.\n";
    public const string EN_ENTER_START_DATE_DAY = "Minimum Update Day (e.g. 15) : ";
    public const string EN_INVALID_DAY = "You must specify a minimum update day. Restarting the process.\n";
    public const string EN_UPDATE_AVAILABLE_ONLY = "Should only objects in the collection that have an existing update be listed from the date you specified until now? (1 - Yes, 2 - No): ";
    public const string EN_PROCESS_STARTING = "\nThe listing process has started. This process may take a while depending on the number of items in the collection. Please wait until it's finished...\n\n";
    public const string EN_UPDATE_CHECK = "Update Check";
    public const string EN_RESTART_MESSAGE = "Press Enter to restart the program, or any other key to exit...";
    public const string EN_TITLE_NOT_FOUND = "Title not found";
    public const string EN_UPDATE_AVAILABLE = "(UPDATE AVAILABLE) - ";
    public const string EN_UPDATED = "(UPDATED) - ";
    public const string EN_NOT_UPDATED = "(NOT UPDATED) - ";
    public const string EN_ITEM = "Item: ";
    public const string EN_UPDATE_DATE = ", Last Update Date: ";
    public const string EN_NONE = "None";

    public const string TR_CONSOLE_TITLE = "Steam Atölye Koleksiyonları İçin Güncelleme Kontrol Edicisi";
    public const string TR_ENTER_COLLECTION_ID = "Steam Atölye Koleksiyonu Numarası : ";
    public const string TR_INVALID_COLLECTION_ID = "Bir steam atölye koleksiyon numarası belirtmelisiniz. İşlem başa sarılıyor.\n";
    public const string TR_ENTER_START_DATE_YEAR = "Minimum Güncellenme Yılı (Örnek 2023) : ";
    public const string TR_INVALID_YEAR = "Minimum bir güncellenme yılı belirtmelisiniz. İşlem başa sarılıyor.\n";
    public const string TR_ENTER_START_DATE_MONTH = "Minimum Güncellenme Ayı (Örnek 6) : ";
    public const string TR_INVALID_MONTH = "Minimum bir güncellenme ayı belirtmelisiniz. İşlem başa sarılıyor.\n";
    public const string TR_ENTER_START_DATE_DAY = "Minimum Güncellenme Günü (Örnek 15) : ";
    public const string TR_INVALID_DAY = "Minimum bir güncellenme günü belirtmelisiniz. İşlem başa sarılıyor.\n";
    public const string TR_UPDATE_AVAILABLE_ONLY = "Belirlediğiniz tarihten şu ana kadar kontrol edilecek olan koleksiyondaki nesnelerden sadece mevcut güncellemesi olanlar mı listelensin? (1 - Evet, 2 - Hayır): ";
    public const string TR_PROCESS_STARTING = "\nListeleme işlemi başladı. Bu işlem koleksiyondaki öğe sayısına göre uzun sürebilir. Lütfen tamamlanana kadar bekleyiniz...\n\n";
    public const string TR_UPDATE_CHECK = "Güncelleme Kontrolleri";
    public const string TR_RESTART_MESSAGE = "Programı baştan başlatmak için Enter tuşuna, kapatmak için herhangi bir tuşa basınız...";
    public const string TR_TITLE_NOT_FOUND = "Başlık bulunamadı";
    public const string TR_UPDATE_AVAILABLE = "(GÜNCELLEME VAR) - ";
    public const string TR_UPDATED = "(GÜNCELLENMİŞ) - ";
    public const string TR_NOT_UPDATED = "(GÜNCELLENMEMİŞ) - ";
    public const string TR_ITEM = "Öğe: ";
    public const string TR_UPDATE_DATE = ", Son Güncellenme Tarihi: ";
    public const string TR_NONE = "Yok";
}