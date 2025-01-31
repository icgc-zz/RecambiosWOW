using RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;
using SQLite;

namespace RecambiosWOW.Infrastructure.Services.Database.Migrations;

// Infrastructure/Providers/Database/Sqlite/Migrations/AddIndexesMigration.cs
using System.Data;

public class AddIndexesMigration : IMigration<SQLiteAsyncConnection>
{
    public int Version => 2;
    public string Description => "Add additional indexes for performance";

    public async Task UpAsync(SQLiteAsyncConnection connection)
    {
        // Parts table indexes
        await connection.ExecuteAsync(@"
           CREATE INDEX IF NOT EXISTS IX_Parts_Price ON Parts(Price);
           CREATE INDEX IF NOT EXISTS IX_Parts_Condition ON Parts(Condition);
           CREATE INDEX IF NOT EXISTS IX_Parts_CreatedAt ON Parts(CreatedAt)");

        await connection.ExecuteAsync(@"
           CREATE INDEX IF NOT EXISTS IX_Parts_Search 
           ON Parts(Name, Description, Manufacturer)");

        // Vehicles table indexes
        await connection.ExecuteAsync(@"
           CREATE INDEX IF NOT EXISTS IX_Vehicles_Year ON Vehicles(Year);
           CREATE INDEX IF NOT EXISTS IX_Vehicles_Engine ON Vehicles(EngineCode);
           CREATE INDEX IF NOT EXISTS IX_Vehicles_Transmission ON Vehicles(TransmissionCode);
           CREATE INDEX IF NOT EXISTS IX_Vehicles_CreatedAt ON Vehicles(CreatedAt)");
    }

    public async Task DownAsync(SQLiteAsyncConnection connection)
    {
        // Drop Parts table indexes
        await connection.ExecuteAsync(@"
           DROP INDEX IF EXISTS IX_Parts_Price;
           DROP INDEX IF EXISTS IX_Parts_Condition;
           DROP INDEX IF EXISTS IX_Parts_CreatedAt;
           DROP INDEX IF EXISTS IX_Parts_Search");

        // Drop Vehicles table indexes
        await connection.ExecuteAsync(@"
           DROP INDEX IF EXISTS IX_Vehicles_Year;
           DROP INDEX IF EXISTS IX_Vehicles_Engine;
           DROP INDEX IF EXISTS IX_Vehicles_Transmission;
           DROP INDEX IF EXISTS IX_Vehicles_CreatedAt");
    }
}