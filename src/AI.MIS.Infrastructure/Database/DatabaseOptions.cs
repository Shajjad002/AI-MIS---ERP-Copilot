namespace AI.MIS.Infrastructure.Database;

public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string ErpReadOnlyDatabase { get; init; } = string.Empty;
    public int CommandTimeoutSeconds { get; init; } = 15;
}
