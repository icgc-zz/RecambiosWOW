﻿namespace RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;

public interface IMigrationService
{
    Task<int> GetCurrentVersionAsync();
    Task<bool> MigrateAsync(CancellationToken cancellationToken = default);
    Task<bool> RollbackAsync(int targetVersion, CancellationToken cancellationToken = default);
}