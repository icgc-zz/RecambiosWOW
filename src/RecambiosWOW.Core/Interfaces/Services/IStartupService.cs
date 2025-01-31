namespace RecambiosWOW.Core.Interfaces.Services;

public interface IStartupService
{
    Task InitializeAsync();
    int Order { get; } // Add ordering to control initialization sequence
}
