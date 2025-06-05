namespace ResourceRequestHub;

internal static class HelpPrinter
{
    public static void Print()
    {
        Console.WriteLine("Group Scholar Resource Request Hub");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  init-db                             Create schema and tables.");
        Console.WriteLine("  seed                                Insert sample requests.");
        Console.WriteLine("  add --scholar NAME --type TYPE      Add a new request.");
        Console.WriteLine("      [--priority low|medium|high] [--status open|in_progress|fulfilled|closed]");
        Console.WriteLine("      [--needed-by YYYY-MM-DD] [--owner NAME] [--channel CHANNEL] [--notes TEXT]");
        Console.WriteLine("  list [--status STATUS] [--priority PRIORITY] [--limit N]");
        Console.WriteLine("  triage [--days N] [--priority PRIORITY] [--owner NAME] [--limit N]");
        Console.WriteLine("  update-status --id UUID --status STATUS");
        Console.WriteLine("  stats                               Show summary stats.");
        Console.WriteLine();
        Console.WriteLine("Environment:");
        Console.WriteLine("  GS_DB_HOST / GS_DB_PORT / GS_DB_USER / GS_DB_PASSWORD / GS_DB_NAME");
        Console.WriteLine("  (Falls back to PGHOST/PGPORT/PGUSER/PGPASSWORD/PGDATABASE.)");
    }
}
