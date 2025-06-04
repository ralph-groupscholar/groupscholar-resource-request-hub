namespace ResourceRequestHub;

internal sealed record DbConfig(string Host, int Port, string User, string Password, string Database)
{
    public static bool TryFromEnvironment(out DbConfig config, out string error)
    {
        var host = GetEnv("GS_DB_HOST", "PGHOST");
        var portValue = GetEnv("GS_DB_PORT", "PGPORT");
        var user = GetEnv("GS_DB_USER", "PGUSER");
        var password = GetEnv("GS_DB_PASSWORD", "PGPASSWORD");
        var database = GetEnv("GS_DB_NAME", "PGDATABASE");

        var issues = new List<string>();
        if (string.IsNullOrWhiteSpace(host)) issues.Add("GS_DB_HOST or PGHOST is required");
        if (string.IsNullOrWhiteSpace(portValue)) issues.Add("GS_DB_PORT or PGPORT is required");
        if (string.IsNullOrWhiteSpace(user)) issues.Add("GS_DB_USER or PGUSER is required");
        if (string.IsNullOrWhiteSpace(password)) issues.Add("GS_DB_PASSWORD or PGPASSWORD is required");
        if (string.IsNullOrWhiteSpace(database)) issues.Add("GS_DB_NAME or PGDATABASE is required");

        if (issues.Count > 0)
        {
            config = new DbConfig("", 0, "", "", "");
            error = "Database configuration missing: " + string.Join(", ", issues);
            return false;
        }

        if (!int.TryParse(portValue, out var port))
        {
            config = new DbConfig("", 0, "", "", "");
            error = "GS_DB_PORT/PGPORT must be numeric.";
            return false;
        }

        config = new DbConfig(host!, port, user!, password!, database!);
        error = string.Empty;
        return true;
    }

    public string ConnectionString()
    {
        return $"Host={Host};Port={Port};Username={User};Password={Password};Database={Database};Ssl Mode=Disable";
    }

    private static string? GetEnv(string primary, string fallback)
    {
        return Environment.GetEnvironmentVariable(primary)
               ?? Environment.GetEnvironmentVariable(fallback);
    }
}
