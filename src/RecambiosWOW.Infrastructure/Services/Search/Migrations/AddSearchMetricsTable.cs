using RecambiosWOW.Infrastructure.Services.Database.Migrations;
using SQLite;

namespace RecambiosWOW.Infrastructure.Services.Search.Migrations;

public class AddSearchMetricsTable : IMigration
{
    public int Version => 3;
    public string Description => "Add search metrics table";

    public async Task UpAsync(SQLiteAsyncConnection connection)
    {
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS SearchMetrics (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QueryText TEXT,
                ResultCount INTEGER,
                ExecutionTimeMs INTEGER,
                PageSize INTEGER,
                PageNumber INTEGER,
                Timestamp DATETIME,
                CacheHit BOOLEAN,
                IndexSizeBytes INTEGER,
                ConcurrentSearches INTEGER
            );
            
            CREATE INDEX IX_SearchMetrics_Timestamp ON SearchMetrics(Timestamp);
        ");
    }

    public async Task DownAsync(SQLiteAsyncConnection connection)
    {
        await connection.ExecuteAsync(@"
            DROP INDEX IF EXISTS IX_SearchMetrics_Timestamp;
            DROP TABLE IF EXISTS SearchMetrics;
        ");
    }
}