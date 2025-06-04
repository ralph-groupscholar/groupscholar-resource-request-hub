namespace ResourceRequestHub;

internal static class TablePrinter
{
    public static void PrintRequests(IReadOnlyList<RequestRecord> requests)
    {
        if (requests.Count == 0)
        {
            Console.WriteLine("No requests found.");
            return;
        }

        var rows = new List<string[]>
        {
            new[] { "ID", "Scholar", "Type", "Priority", "Status", "Needed By", "Owner", "Updated" }
        };

        foreach (var request in requests)
        {
            rows.Add(new[]
            {
                request.Id.ToString(),
                request.ScholarName,
                request.RequestType,
                request.Priority,
                request.Status,
                request.NeededBy?.ToString("yyyy-MM-dd") ?? "-",
                request.Owner ?? "-",
                request.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd")
            });
        }

        Print(rows);
    }

    public static void PrintStats(RequestStats stats)
    {
        Console.WriteLine("Status counts:");
        var statusRows = new List<string[]>
        {
            new[] { "Status", "Count" }
        };
        foreach (var row in stats.StatusCounts)
        {
            statusRows.Add(new[] { row.Status, row.Count.ToString() });
        }
        Print(statusRows);

        Console.WriteLine();
        Console.WriteLine("Priority counts:");
        var priorityRows = new List<string[]>
        {
            new[] { "Priority", "Count" }
        };
        foreach (var row in stats.PriorityCounts)
        {
            priorityRows.Add(new[] { row.Priority, row.Count.ToString() });
        }
        Print(priorityRows);

        Console.WriteLine();
        var avgText = stats.AverageDaysOpen.HasValue
            ? $"{stats.AverageDaysOpen.Value:F1} days"
            : "n/a";
        Console.WriteLine($"Average days open (open/in_progress): {avgText}");
    }

    private static void Print(IReadOnlyList<string[]> rows)
    {
        var widths = new int[rows[0].Length];
        foreach (var row in rows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                widths[i] = Math.Max(widths[i], row[i].Length);
            }
        }

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            var padded = row.Select((cell, i) => cell.PadRight(widths[i] + 2));
            Console.WriteLine(string.Join(string.Empty, padded).TrimEnd());
            if (r == 0)
            {
                var separators = widths.Select(w => new string('-', w).PadRight(w + 2));
                Console.WriteLine(string.Join(string.Empty, separators).TrimEnd());
            }
        }
    }
}
