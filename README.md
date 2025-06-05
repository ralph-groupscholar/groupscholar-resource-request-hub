# Group Scholar Resource Request Hub

A C# command-line tool that logs, triages, and summarizes scholar resource requests (equipment, emergency support, mentoring matches, and more). It keeps a single source of truth in Postgres so the team can quickly see what needs attention.

## Features
- Add and track resource requests with priority and status.
- Filter lists by status or priority.
- Update request status as work progresses.
- Generate summary stats for operational reporting.
- Seed realistic sample data for immediate demo value.

## Tech Stack
- C# (.NET 10)
- PostgreSQL (Npgsql)
- xUnit for tests

## Setup

Set environment variables (or use the PG* equivalents):

```bash
export GS_DB_HOST="db-acupinir.groupscholar.com"
export GS_DB_PORT="23947"
export GS_DB_USER="ralph"
export GS_DB_PASSWORD="<password>"
export GS_DB_NAME="postgres"
```

Initialize and seed the database:

```bash
dotnet run --project ResourceRequestHub -- init-db
dotnet run --project ResourceRequestHub -- seed
```

## Usage

Add a request:

```bash
dotnet run --project ResourceRequestHub -- add \
  --scholar "Aisha Thompson" \
  --type "Laptop replacement" \
  --priority high \
  --needed-by 2026-02-20 \
  --owner "Casework" \
  --channel email \
  --notes "Device failing during midterms"
```

List requests:

```bash
dotnet run --project ResourceRequestHub -- list --status open --limit 10
```

Triage requests due soon (default 7 days):

```bash
dotnet run --project ResourceRequestHub -- triage --days 5 --priority high
```

Update status:

```bash
dotnet run --project ResourceRequestHub -- update-status --id <uuid> --status fulfilled
```

Stats:

```bash
dotnet run --project ResourceRequestHub -- stats
```

## Testing

```bash
dotnet test
```
