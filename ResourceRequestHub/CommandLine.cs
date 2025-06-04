namespace ResourceRequestHub;

internal sealed record Command(string Name, IReadOnlyDictionary<string, string> Options)
{
    public string? GetOption(string key)
    {
        return Options.TryGetValue(key, out var value) ? value : null;
    }

    public int GetIntOption(string key, int fallback)
    {
        if (!Options.TryGetValue(key, out var value))
        {
            return fallback;
        }

        return int.TryParse(value, out var parsed) ? parsed : fallback;
    }
}

internal static class CommandLine
{
    public static Command Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new Command("help", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        var command = args[0];
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 1; i < args.Length; i++)
        {
            var token = args[i];
            if (!token.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var trimmed = token[2..];
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            var equalsIndex = trimmed.IndexOf('=');
            if (equalsIndex > 0)
            {
                var key = trimmed[..equalsIndex];
                var value = trimmed[(equalsIndex + 1)..];
                options[key] = value;
                continue;
            }

            if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
            {
                options[trimmed] = args[i + 1];
                i++;
                continue;
            }

            options[trimmed] = "true";
        }

        return new Command(command, options);
    }
}
