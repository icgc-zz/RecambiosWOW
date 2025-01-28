namespace RecambiosWOW.Core.Interfaces;

public interface IStartupService
{
    Task InitializeAsync();
    int Order { get; } // Add ordering to control initialization sequence
}
