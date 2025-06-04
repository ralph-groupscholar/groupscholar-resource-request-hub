using Npgsql;

namespace ResourceRequestHub;

internal sealed class RequestRepository
{
    private const string SchemaName = "gs_resource_request_hub";
    private const string TableName = "requests";

    private readonly DbConfig _config;

    public RequestRepository(DbConfig config)
    {
        _config = config;
    }

    public async Task InitializeAsync()
    {
        await using var connection = await OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            create schema if not exists {SchemaName};
            create table if not exists {SchemaName}.{TableName} (
                id uuid primary key,
                scholar_name text not null,
                request_type text not null,
                priority text not null,
                status text not null,
                needed_by date,
                owner text,
                channel text,
                notes text,
                created_at timestamptz not null,
                updated_at timestamptz not null
            );
            create index if not exists requests_status_idx on {SchemaName}.{TableName}(status);
            create index if not exists requests_priority_idx on {SchemaName}.{TableName}(priority);
            """;
        await command.ExecuteNonQueryAsync();
    }

    public async Task SeedAsync()
    {
        await InitializeAsync();
        await using var connection = await OpenAsync();
        await using var countCommand = connection.CreateCommand();
        countCommand.CommandText = $"select count(*) from {SchemaName}.{TableName};";
        var count = (long?)await countCommand.ExecuteScalarAsync() ?? 0;
        if (count > 0)
        {
            return;
        }

        var seedRows = SeedData.SampleRequests();
        foreach (var row in seedRows)
        {
            await AddAsync(row);
        }
    }

    public async Task<Guid> AddAsync(ResourceRequestInput input)
    {
        await using var connection = await OpenAsync();
        await using var command = connection.CreateCommand();
        var id = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        command.CommandText = $"""
            insert into {SchemaName}.{TableName}
            (id, scholar_name, request_type, priority, status, needed_by, owner, channel, notes, created_at, updated_at)
            values (@id, @scholar_name, @request_type, @priority, @status, @needed_by, @owner, @channel, @notes, @created_at, @updated_at);
            """;

        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("scholar_name", input.ScholarName ?? string.Empty);
        command.Parameters.AddWithValue("request_type", input.RequestType ?? string.Empty);
        command.Parameters.AddWithValue("priority", input.Priority);
        command.Parameters.AddWithValue("status", input.Status);
        command.Parameters.AddWithValue("needed_by", input.NeededBy.HasValue ? input.NeededBy.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value);
        command.Parameters.AddWithValue("owner", (object?)input.Owner ?? DBNull.Value);
        command.Parameters.AddWithValue("channel", (object?)input.Channel ?? DBNull.Value);
        command.Parameters.AddWithValue("notes", (object?)input.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("created_at", now);
        command.Parameters.AddWithValue("updated_at", now);

        await command.ExecuteNonQueryAsync();
        return id;
    }

    public async Task<IReadOnlyList<RequestRecord>> ListAsync(RequestFilter filter)
    {
        await using var connection = await OpenAsync();
        await using var command = connection.CreateCommand();

        var clauses = new List<string>();
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            clauses.Add("status = @status");
            command.Parameters.AddWithValue("status", filter.Status!);
        }

        if (!string.IsNullOrWhiteSpace(filter.Priority))
        {
            clauses.Add("priority = @priority");
            command.Parameters.AddWithValue("priority", filter.Priority!);
        }

        var whereClause = clauses.Count > 0 ? "where " + string.Join(" and ", clauses) : string.Empty;
        command.CommandText = $"""
            select id, scholar_name, request_type, priority, status, needed_by, owner, updated_at
            from {SchemaName}.{TableName}
            {whereClause}
            order by updated_at desc
            limit @limit;
            """;

        command.Parameters.AddWithValue("limit", filter.Limit);

        var results = new List<RequestRecord>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new RequestRecord(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : DateOnly.FromDateTime(reader.GetDateTime(5)),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.GetFieldValue<DateTimeOffset>(7)
            ));
        }

        return results;
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        if (!Guid.TryParse(id, out var parsed))
        {
            return false;
        }

        await using var connection = await OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            update {SchemaName}.{TableName}
            set status = @status,
                updated_at = @updated_at
            where id = @id;
            """;
        command.Parameters.AddWithValue("status", status);
        command.Parameters.AddWithValue("updated_at", DateTimeOffset.UtcNow);
        command.Parameters.AddWithValue("id", parsed);

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<RequestStats> GetStatsAsync()
    {
        await using var connection = await OpenAsync();
        var statusCounts = await GetStatusCounts(connection);
        var priorityCounts = await GetPriorityCounts(connection);

        await using var avgCommand = connection.CreateCommand();
        avgCommand.CommandText = $"""
            select avg(extract(epoch from (now() - created_at)) / 86400.0)
            from {SchemaName}.{TableName}
            where status in ('open', 'in_progress');
            """;
        var avgValue = await avgCommand.ExecuteScalarAsync();
        double? avgDays = avgValue == DBNull.Value || avgValue is null
            ? null
            : Convert.ToDouble(avgValue);

        return new RequestStats(statusCounts, priorityCounts, avgDays);
    }

    private async Task<IReadOnlyList<StatusStat>> GetStatusCounts(NpgsqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            select status, count(*)
            from {SchemaName}.{TableName}
            group by status
            order by count desc;
            """;

        var results = new List<StatusStat>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new StatusStat(reader.GetString(0), reader.GetInt32(1)));
        }

        return results;
    }

    private async Task<IReadOnlyList<PriorityStat>> GetPriorityCounts(NpgsqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            select priority, count(*)
            from {SchemaName}.{TableName}
            group by priority
            order by count desc;
            """;

        var results = new List<PriorityStat>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new PriorityStat(reader.GetString(0), reader.GetInt32(1)));
        }

        return results;
    }

    private async Task<NpgsqlConnection> OpenAsync()
    {
        var connection = new NpgsqlConnection(_config.ConnectionString());
        await connection.OpenAsync();
        return connection;
    }
}
