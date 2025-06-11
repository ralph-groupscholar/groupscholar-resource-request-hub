using ResourceRequestHub;

namespace ResourceRequestHub.Tests;

public class CsvExporterTests
{
    [Fact]
    public void WritesEscapedCsvLines()
    {
        var request = new RequestExportRecord(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Ari",
            "Laptop",
            "high",
            "open",
            new DateOnly(2026, 2, 20),
            null,
            "email",
            "Need \"urgent\", help",
            new DateTimeOffset(2026, 2, 8, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 2, 9, 10, 0, 0, TimeSpan.Zero));

        var path = Path.GetTempFileName();
        try
        {
            CsvExporter.WriteRequests(path, new[] { request });

            var lines = File.ReadAllLines(path);
            Assert.Equal("id,scholar_name,request_type,priority,status,needed_by,owner,channel,notes,created_at,updated_at", lines[0]);

            var expected = string.Join(',', new[]
            {
                "11111111-1111-1111-1111-111111111111",
                "Ari",
                "Laptop",
                "high",
                "open",
                "2026-02-20",
                string.Empty,
                "email",
                "\"Need \"\"urgent\"\", help\"",
                "2026-02-08T10:00:00.0000000+00:00",
                "2026-02-09T10:00:00.0000000+00:00"
            });

            Assert.Equal(expected, lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
