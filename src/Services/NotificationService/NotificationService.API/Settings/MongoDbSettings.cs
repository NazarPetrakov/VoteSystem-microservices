namespace NotificationService.API.Settings;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    public string ConnectionString { get; set; } = String.Empty;
    public string DatabaseName { get; set; } = String.Empty;
    public string PollWithTotalVotesCollectionName { get; set; } = String.Empty;
    public string UserStatsCollectionName { get; set; } = String.Empty;
}
