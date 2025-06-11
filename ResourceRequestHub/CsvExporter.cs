namespace ResourceRequestHub;

internal static class CsvExporter
{
    private static readonly string[] Header =
    {
        "id",
        "scholar_name",
        "request_type",
        "priority",
        "status",
        "needed_by",
        "owner",
        "channel",
        "notes",
        "created_at",
        "updated_at"
    };

    public static int WriteRequests(string path, IReadOnlyList<RequestExportRecord> requests)
    {
        var lines = new List<string>(requests.Count + 1)
        {
            string.Join(',', Header)
        };

        foreach (var request in requests)
        {
            lines.Add(string.Join(',', new[]
            {
                Escape(request.Id.ToString()),
                Escape(request.ScholarName),
                Escape(request.RequestType),
                Escape(request.Priority),
                Escape(request.Status),
                Escape(request.NeededBy?.ToString("yyyy-MM-dd") ?? string.Empty),
                Escape(request.Owner ?? string.Empty),
                Escape(request.Channel ?? string.Empty),
                Escape(request.Notes ?? string.Empty),
                Escape(request.CreatedAt.ToString("o")),
                Escape(request.UpdatedAt.ToString("o"))
            }));
        }

        File.WriteAllLines(path, lines);
        return requests.Count;
    }

    internal static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
        {
            var escaped = value.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }

        return value;
    }
}
