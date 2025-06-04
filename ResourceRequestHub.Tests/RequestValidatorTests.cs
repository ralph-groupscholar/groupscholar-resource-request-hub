using ResourceRequestHub;

namespace ResourceRequestHub.Tests;

public class RequestValidatorTests
{
    [Fact]
    public void RejectsMissingScholar()
    {
        var input = new ResourceRequestInput(
            ScholarName: null,
            RequestType: "Laptop",
            Priority: "high",
            Status: "open",
            NeededBy: null,
            Owner: null,
            Channel: null,
            Notes: null);

        var result = RequestValidator.Validate(input);
        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Contains("scholar"));
    }

    [Fact]
    public void AcceptsValidRequest()
    {
        var input = new ResourceRequestInput(
            ScholarName: "Jordan",
            RequestType: "Transit pass",
            Priority: "medium",
            Status: "open",
            NeededBy: new DateOnly(2026, 2, 20),
            Owner: "Ops",
            Channel: "email",
            Notes: "Weekly pass");

        var result = RequestValidator.Validate(input);
        Assert.True(result.IsValid);
    }
}
