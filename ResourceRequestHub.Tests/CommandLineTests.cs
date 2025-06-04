using ResourceRequestHub;

namespace ResourceRequestHub.Tests;

public class CommandLineTests
{
    [Fact]
    public void ParsesCommandAndOptions()
    {
        var command = CommandLine.Parse(new[]
        {
            "add",
            "--scholar",
            "Aisha",
            "--priority=high",
            "--limit",
            "5"
        });

        Assert.Equal("add", command.Name);
        Assert.Equal("Aisha", command.GetOption("scholar"));
        Assert.Equal("high", command.GetOption("priority"));
        Assert.Equal(5, command.GetIntOption("limit", 0));
    }

    [Fact]
    public void FallsBackToHelpWhenNoArgs()
    {
        var command = CommandLine.Parse(Array.Empty<string>());
        Assert.Equal("help", command.Name);
    }
}
