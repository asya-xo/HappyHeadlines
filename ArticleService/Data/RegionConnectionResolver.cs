namespace ArticleService.Data;

public class RegionConnectionResolver
{
    // regions: global, eu, na, sa, af, as, oc, an
    private readonly Dictionary<string, string?> _map;

    public RegionConnectionResolver(IConfiguration cfg)
    {
        _map = new()
        {
            ["global"] = Environment.GetEnvironmentVariable("DB_CONN_GLOBAL"),
            ["eu"] = Environment.GetEnvironmentVariable("DB_CONN_EU"),
            ["na"] = Environment.GetEnvironmentVariable("DB_CONN_NA"),
            ["sa"] = Environment.GetEnvironmentVariable("DB_CONN_SA"),
            ["af"] = Environment.GetEnvironmentVariable("DB_CONN_AF"),
            ["as"] = Environment.GetEnvironmentVariable("DB_CONN_AS"),
            ["oc"] = Environment.GetEnvironmentVariable("DB_CONN_OC"),
            ["an"] = Environment.GetEnvironmentVariable("DB_CONN_AN")
        };
    }

    public string GetConnectionString(string region)
    {
        var key = region.ToLowerInvariant();
        if (!_map.TryGetValue(key, out var conn) || string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException($"No connection string configured for region '{region}'.");
        return conn!;
    }
}
