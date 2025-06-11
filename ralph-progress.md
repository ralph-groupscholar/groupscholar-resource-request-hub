# Ralph Progress Log

## 2026-02-08
- Started groupscholar-resource-request-hub.
- Built C# CLI with Postgres persistence for logging and triaging resource requests.
- Added schema initialization, seed data, listing, status updates, and stats reporting.
- Added unit tests for command parsing and validation.
- Initialized production schema and seeded sample requests.

## 2026-02-08
- Added triage command to surface open/in-progress requests due within a configurable window.
- Implemented triage query/filtering with priority/owner options and due-date urgency display.
- Updated help and README usage examples.

## 2026-02-08
- Added CSV export command with filtered output and automatic file naming.
- Added export repository query to include full request fields.
- Added CSV exporter with escaping logic plus unit test coverage.
- Updated CLI help and README usage examples.
