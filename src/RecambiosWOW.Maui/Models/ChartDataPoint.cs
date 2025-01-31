namespace RecambiosWOW.Maui.Models;

public class ChartDataPoint
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }

    public ChartDataPoint(object data)
    {
        // Convert data point based on type
        if (data is SearchMetricsPoint metric)
        {
            Timestamp = metric.Timestamp;
            Value = metric.Value;
        }
    }
}