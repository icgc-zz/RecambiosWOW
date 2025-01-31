using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Monitoring;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Infrastructure.Monitoring;

namespace RecambiosWOW.Application.Services.Monitoring.Models;

public class AlertService : IAlertService
{
    private readonly ISearchMetricsCollector _metricsCollector;
    private readonly ILogger<AlertService> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IDbProvider _dbProvider;

    public AlertService(
        ISearchMetricsCollector metricsCollector,
        IEmailService emailService,
        IConfiguration configuration,
        IDbProvider dbProvider,
        ILogger<AlertService> logger)
    {
        _metricsCollector = metricsCollector ?? throw new ArgumentNullException(nameof(metricsCollector));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task CheckAndSendAlertsAsync()
    {
        try
        {
            var report = await _metricsCollector.GenerateReportAsync(
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow);

            var alerts = new List<Alert>();

            // Check response time
            if (report.P95ResponseTimeMs > 500)
            {
                alerts.Add(new Alert
                {
                    MetricName = "ResponseTime",
                    CurrentValue = report.P95ResponseTimeMs,
                    Threshold = 500,
                    Message = $"P95 response time ({report.P95ResponseTimeMs:N0}ms) exceeds 500ms threshold",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check concurrent searches
            if (report.ConcurrentSearchesMax > 50)
            {
                alerts.Add(new Alert
                {
                    MetricName = "ConcurrentSearches",
                    CurrentValue = report.ConcurrentSearchesMax,
                    Threshold = 50,
                    Message = $"High concurrent searches detected ({report.ConcurrentSearchesMax})",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check index size
            var indexSizeGB = report.IndexSizeBytes / (1024.0 * 1024.0 * 1024.0);
            if (indexSizeGB > 0.8) // Alert at 80% of threshold
            {
                alerts.Add(new Alert
                {
                    MetricName = "IndexSize",
                    CurrentValue = indexSizeGB,
                    Threshold = 1,
                    Message = $"Search index size ({indexSizeGB:N2}GB) approaching 1GB limit",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTime.UtcNow
                });
            }

            foreach (var alert in alerts)
            {
                await SaveAlertAsync(alert);
                await SendAlertNotificationAsync(alert);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking and sending alerts");
            throw;
        }
    }

    public async Task SetAlertThresholdAsync(string metricName, double threshold)
    {
        if (string.IsNullOrWhiteSpace(metricName))
            throw new ArgumentNullException(nameof(metricName));

        try
        {
            await using var db = await _dbProvider.GetConnectionAsync();
            await db.ExecuteAsync(
                "INSERT OR REPLACE INTO AlertThresholds (MetricName, Threshold, UpdatedAt) VALUES (@MetricName, @Threshold, @UpdatedAt)",
                new { MetricName = metricName, Threshold = threshold, UpdatedAt = DateTime.UtcNow });

            _logger.LogInformation(
                "Updated threshold for {MetricName} to {Threshold}",
                metricName, threshold);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting alert threshold for {MetricName}", metricName);
            throw;
        }
    }
    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var alerts = await db.QueryAsync<Alert>(@"
                SELECT * FROM Alerts 
                WHERE Acknowledged = 0 
                ORDER BY Timestamp DESC");

            return alerts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active alerts");
            throw;
        }
    }

    private async Task SaveAlertAsync(Alert alert)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            await db.ExecuteAsync(@"
                INSERT INTO Alerts (
                    Id,
                    MetricName,
                    CurrentValue,
                    Threshold,
                    Message,
                    Severity,
                    Timestamp,
                    Acknowledged
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                alert.Id,
                alert.MetricName,
                alert.CurrentValue,
                alert.Threshold,
                alert.Message,
                (int)alert.Severity,
                alert.Timestamp,
                false);

            _logger.LogInformation(
                "Saved new alert: {MetricName} - {Message}", 
                alert.MetricName, alert.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving alert for {MetricName}", alert.MetricName);
            throw;
        }
    }

    private async Task SendAlertNotificationAsync(Alert alert)
    {
        var recipients = _configuration.GetSection("Monitoring:AlertRecipients").Get<string[]>();
        if (recipients == null || !recipients.Any())
        {
            _logger.LogWarning("No alert recipients configured");
            return;
        }

        var subject = $"Search Performance Alert: {alert.MetricName}";
        var body = GenerateAlertEmailBody(alert);

        try
        {
            await _emailService.SendEmailAsync(recipients, subject, body);
            _logger.LogInformation(
                "Sent alert notification for {MetricName} to {RecipientCount} recipients", 
                alert.MetricName, recipients.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send alert notification for {MetricName}", alert.MetricName);
            throw;
        }
    }

    private string GenerateAlertEmailBody(Alert alert)
    {
        return $@"
            Alert Details:
            - Metric: {alert.MetricName}
            - Current Value: {alert.CurrentValue}
            - Threshold: {alert.Threshold}
            - Message: {alert.Message}
            - Severity: {alert.Severity}
            - Time: {alert.Timestamp:yyyy-MM-dd HH:mm:ss UTC}

            View more details in the monitoring dashboard: {_configuration["ApplicationUrl"]}/monitoring

            This is an automated message. Please do not reply.
        ";
    }
}