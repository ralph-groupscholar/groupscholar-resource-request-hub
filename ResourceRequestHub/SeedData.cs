namespace ResourceRequestHub;

internal static class SeedData
{
    public static IReadOnlyList<ResourceRequestInput> SampleRequests() => new List<ResourceRequestInput>
    {
        new(
            ScholarName: "Aisha Thompson",
            RequestType: "Laptop replacement",
            Priority: "high",
            Status: "open",
            NeededBy: new DateOnly(2026, 2, 20),
            Owner: "Casework",
            Channel: "email",
            Notes: "Current device failing during midterm projects."
        ),
        new(
            ScholarName: "Miguel Santos",
            RequestType: "Textbook voucher",
            Priority: "medium",
            Status: "in_progress",
            NeededBy: new DateOnly(2026, 2, 16),
            Owner: "Operations",
            Channel: "form",
            Notes: "Biology lab manual and access code."
        ),
        new(
            ScholarName: "Priya Desai",
            RequestType: "Transit pass",
            Priority: "low",
            Status: "fulfilled",
            NeededBy: new DateOnly(2026, 2, 12),
            Owner: "Operations",
            Channel: "slack",
            Notes: "Renewed monthly pass for campus commute."
        ),
        new(
            ScholarName: "Jordan Lee",
            RequestType: "Emergency grant",
            Priority: "high",
            Status: "open",
            NeededBy: new DateOnly(2026, 2, 11),
            Owner: "Financial Aid",
            Channel: "phone",
            Notes: "Housing deposit due within 72 hours."
        ),
        new(
            ScholarName: "Sofia Ramirez",
            RequestType: "Childcare support",
            Priority: "medium",
            Status: "in_progress",
            NeededBy: new DateOnly(2026, 2, 25),
            Owner: "Student Support",
            Channel: "email",
            Notes: "Looking for after-school coverage for two weeks."
        ),
        new(
            ScholarName: "Noah Patel",
            RequestType: "Testing accommodations",
            Priority: "medium",
            Status: "fulfilled",
            NeededBy: new DateOnly(2026, 2, 14),
            Owner: "Student Support",
            Channel: "form",
            Notes: "Approved extra time for certification exam."
        ),
        new(
            ScholarName: "Emma Robinson",
            RequestType: "Mentoring match",
            Priority: "low",
            Status: "closed",
            NeededBy: null,
            Owner: "Mentorship",
            Channel: "email",
            Notes: "Matched with volunteer in data science."
        ),
        new(
            ScholarName: "Kai Nguyen",
            RequestType: "Interview attire stipend",
            Priority: "high",
            Status: "open",
            NeededBy: new DateOnly(2026, 2, 18),
            Owner: "Career Services",
            Channel: "form",
            Notes: "Interview scheduled with partner on Feb 19."
        )
    };
}
