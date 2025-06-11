using System.Globalization;

namespace ResourceRequestHub;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var command = CommandLine.Parse(args);
        if (command.Name is "help" or "--help" or "-h")
        {
            HelpPrinter.Print();
            return 0;
        }

        if (!DbConfig.TryFromEnvironment(out var config, out var error))
        {
            Console.Error.WriteLine(error);
            return 1;
        }

        var repository = new RequestRepository(config);

        switch (command.Name)
        {
            case "init-db":
                await repository.InitializeAsync();
                Console.WriteLine("Database initialized.");
                return 0;
            case "seed":
                await repository.SeedAsync();
                Console.WriteLine("Seed data inserted.");
                return 0;
            case "add":
                return await HandleAddAsync(command, repository);
            case "list":
                return await HandleListAsync(command, repository);
            case "export":
                return await HandleExportAsync(command, repository);
            case "triage":
                return await HandleTriageAsync(command, repository);
            case "update-status":
                return await HandleUpdateStatusAsync(command, repository);
            case "stats":
                return await HandleStatsAsync(repository);
            default:
                Console.Error.WriteLine($"Unknown command: {command.Name}");
                HelpPrinter.Print();
                return 1;
        }
    }

    private static async Task<int> HandleAddAsync(Command command, RequestRepository repository)
    {
        var input = new ResourceRequestInput(
            ScholarName: command.GetOption("scholar"),
            RequestType: command.GetOption("type"),
            Priority: command.GetOption("priority") ?? "medium",
            Status: command.GetOption("status") ?? "open",
            NeededBy: ParseDate(command.GetOption("needed-by")),
            Owner: command.GetOption("owner"),
            Channel: command.GetOption("channel"),
            Notes: command.GetOption("notes")
        );

        var validation = RequestValidator.Validate(input);
        if (!validation.IsValid)
        {
            Console.Error.WriteLine("Invalid request:");
            foreach (var issue in validation.Issues)
            {
                Console.Error.WriteLine($"- {issue}");
            }
            return 1;
        }

        var id = await repository.AddAsync(input);
        Console.WriteLine($"Added request {id}.");
        return 0;
    }

    private static async Task<int> HandleListAsync(Command command, RequestRepository repository)
    {
        var filter = new RequestFilter(
            Status: command.GetOption("status"),
            Priority: command.GetOption("priority"),
            Limit: command.GetIntOption("limit", 25)
        );

        var requests = await repository.ListAsync(filter);
        TablePrinter.PrintRequests(requests);
        return 0;
    }

    private static async Task<int> HandleExportAsync(Command command, RequestRepository repository)
    {
        var filter = new RequestFilter(
            Status: command.GetOption("status"),
            Priority: command.GetOption("priority"),
            Limit: command.GetIntOption("limit", 200)
        );

        var path = command.GetOption("path");
        if (string.IsNullOrWhiteSpace(path))
        {
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
            path = $"resource-requests-{stamp}.csv";
        }

        var requests = await repository.ExportAsync(filter);
        var count = CsvExporter.WriteRequests(path, requests);
        Console.WriteLine($"Exported {count} requests to {Path.GetFullPath(path)}.");
        return 0;
    }

    private static async Task<int> HandleUpdateStatusAsync(Command command, RequestRepository repository)
    {
        var id = command.GetOption("id");
        var status = command.GetOption("status");
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
        {
            Console.Error.WriteLine("update-status requires --id and --status.");
            return 1;
        }

        var updated = await repository.UpdateStatusAsync(id, status);
        Console.WriteLine(updated
            ? $"Updated status for {id}."
            : $"No request found for {id}.");
        return 0;
    }

    private static async Task<int> HandleTriageAsync(Command command, RequestRepository repository)
    {
        var filter = new TriageFilter(
            WindowDays: command.GetIntOption("days", 7),
            Priority: command.GetOption("priority"),
            Owner: command.GetOption("owner"),
            Limit: command.GetIntOption("limit", 25)
        );

        var results = await repository.GetTriageAsync(filter);
        TablePrinter.PrintTriage(results);
        return 0;
    }

    private static async Task<int> HandleStatsAsync(RequestRepository repository)
    {
        var stats = await repository.GetStatsAsync();
        TablePrinter.PrintStats(stats);
        return 0;
    }

    private static DateOnly? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date))
        {
            return date;
        }

        return null;
    }
}
