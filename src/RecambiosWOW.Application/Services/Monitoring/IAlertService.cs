using RecambiosWOW.Application.Services.Monitoring.Models;

namespace RecambiosWOW.Application.Services.Monitoring;

public interface IAlertService
{
    Task CheckAndSendAlertsAsync();
    Task SetAlertThresholdAsync(string metricName, double threshold);
    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
}