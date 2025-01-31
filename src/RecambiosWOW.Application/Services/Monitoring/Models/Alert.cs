namespace RecambiosWOW.Application.Services.Monitoring.Models;

public class Alert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MetricName { get; set; }
    public double CurrentValue { get; set; }
    public double Threshold { get; set; }
    public string Message { get; set; }
    public AlertSeverity Severity { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Acknowledged { get; set; }
}
