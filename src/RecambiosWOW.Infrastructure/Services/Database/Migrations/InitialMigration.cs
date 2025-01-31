using RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;
using SQLite;

namespace RecambiosWOW.Infrastructure.Services.Database.Migrations;

public class InitialMigration : IMigration<SQLiteAsyncConnection>
{
   public int Version => 1;
   public string Description => "Initial schema creation";

   public async Task UpAsync(SQLiteAsyncConnection connection)
   {
       await connection.ExecuteAsync(@"
           CREATE TABLE IF NOT EXISTS Parts (
               Id INTEGER PRIMARY KEY AUTOINCREMENT,
               Manufacturer TEXT NOT NULL,
               PartNumber TEXT NOT NULL,
               SerialNumber TEXT,
               Name TEXT NOT NULL,
               Description TEXT,
               Condition INTEGER NOT NULL,
               Price DECIMAL NOT NULL,
               Currency TEXT NOT NULL,
               Source INTEGER NOT NULL,
               CreatedAt DATETIME NOT NULL,
               UpdatedAt DATETIME
           )");

       await connection.ExecuteAsync(@"
           CREATE INDEX IF NOT EXISTS IX_Parts_SerialNumber ON Parts(SerialNumber);
           CREATE INDEX IF NOT EXISTS IX_Parts_PartNumber ON Parts(PartNumber);
           CREATE INDEX IF NOT EXISTS IX_Parts_Manufacturer ON Parts(Manufacturer)");

       await connection.ExecuteAsync(@"
           CREATE TABLE IF NOT EXISTS Vehicles (
               VIN TEXT PRIMARY KEY,
               LicensePlate TEXT,
               Make TEXT NOT NULL,
               Model TEXT NOT NULL,
               Year INTEGER NOT NULL,
               EngineCode TEXT,
               TransmissionCode TEXT,
               CreatedAt DATETIME NOT NULL,
               UpdatedAt DATETIME
           )");

       await connection.ExecuteAsync(@"
           CREATE INDEX IF NOT EXISTS IX_Vehicles_LicensePlate ON Vehicles(LicensePlate);
           CREATE INDEX IF NOT EXISTS IX_Vehicles_Make_Model ON Vehicles(Make, Model)");
   }

   public async Task DownAsync(SQLiteAsyncConnection connection)
   {
       await connection.ExecuteAsync("DROP TABLE IF EXISTS Parts");
       await connection.ExecuteAsync("DROP TABLE IF EXISTS Vehicles");
   }
}

