namespace RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;

public interface IMigration<TConnection>
{
    int Version { get; }
    string Description { get; }
    Task UpAsync(TConnection connection);
    Task DownAsync(TConnection connection);
}