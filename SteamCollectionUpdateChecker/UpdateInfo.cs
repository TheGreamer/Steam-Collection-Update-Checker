namespace SteamCollectionUpdateChecker;

public class UpdateInfo(string collectionId, int startDateYear, int startDateMonth, int startDateDay, string language, bool updateAvailableOnly, bool isRedirectionEnabled, bool includeUpdateNotes)
{
    public string CollectionId { get; set; } = collectionId;
    public int StartDateYear { get; set; } = startDateYear;
    public int StartDateMonth { get; set; } = startDateMonth;
    public int StartDateDay { get; set; } = startDateDay;
    public string Language { get; set; } = language;
    public bool UpdateAvailableOnly { get; set; } = updateAvailableOnly;
    public bool IsRedirectionEnabled { get; set; } = isRedirectionEnabled;
    public bool IncludeUpdateNotes { get; set; } = includeUpdateNotes;
}