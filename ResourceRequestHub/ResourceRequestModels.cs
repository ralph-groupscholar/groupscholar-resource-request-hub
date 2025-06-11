namespace ResourceRequestHub;

internal sealed record ResourceRequestInput(
    string? ScholarName,
    string? RequestType,
    string Priority,
    string Status,
    DateOnly? NeededBy,
    string? Owner,
    string? Channel,
    string? Notes);

internal sealed record RequestRecord(
    Guid Id,
    string ScholarName,
    string RequestType,
    string Priority,
    string Status,
    DateOnly? NeededBy,
    string? Owner,
    DateTimeOffset UpdatedAt);

internal sealed record RequestExportRecord(
    Guid Id,
    string ScholarName,
    string RequestType,
    string Priority,
    string Status,
    DateOnly? NeededBy,
    string? Owner,
    string? Channel,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

internal sealed record RequestFilter(string? Status, string? Priority, int Limit);

internal sealed record TriageRecord(
    Guid Id,
    string ScholarName,
    string RequestType,
    string Priority,
    string Status,
    DateOnly NeededBy,
    string? Owner,
    int DaysUntilDue,
    DateTimeOffset UpdatedAt);

internal sealed record TriageFilter(int WindowDays, string? Priority, string? Owner, int Limit);

internal sealed record StatusStat(string Status, int Count);

internal sealed record PriorityStat(string Priority, int Count);

internal sealed record RequestStats(
    IReadOnlyList<StatusStat> StatusCounts,
    IReadOnlyList<PriorityStat> PriorityCounts,
    double? AverageDaysOpen);

internal sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Issues)
{
    public static ValidationResult Success() => new(true, Array.Empty<string>());

    public static ValidationResult Failure(params string[] issues) => new(false, issues);
}

internal static class RequestValidator
{
    private static readonly HashSet<string> AllowedPriorities = new(StringComparer.OrdinalIgnoreCase)
    {
        "low",
        "medium",
        "high"
    };

    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "open",
        "in_progress",
        "fulfilled",
        "closed"
    };

    public static ValidationResult Validate(ResourceRequestInput input)
    {
        var issues = new List<string>();

        if (string.IsNullOrWhiteSpace(input.ScholarName))
        {
            issues.Add("scholar name is required");
        }

        if (string.IsNullOrWhiteSpace(input.RequestType))
        {
            issues.Add("request type is required");
        }

        if (!AllowedPriorities.Contains(input.Priority))
        {
            issues.Add("priority must be low, medium, or high");
        }

        if (!AllowedStatuses.Contains(input.Status))
        {
            issues.Add("status must be open, in_progress, fulfilled, or closed");
        }

        return issues.Count == 0 ? ValidationResult.Success() : new ValidationResult(false, issues);
    }
}
