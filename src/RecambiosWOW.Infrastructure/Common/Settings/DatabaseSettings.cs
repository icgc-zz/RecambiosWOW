namespace RecambiosWOW.Infrastructure.Common.Settings;

public class DatabaseSettings
{
    public string DatabasePath { get; set; } = string.Empty;
    public bool AutoCreateTables { get; set; } = true;
}
