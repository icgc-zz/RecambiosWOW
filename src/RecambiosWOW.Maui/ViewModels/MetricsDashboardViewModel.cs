using System.Collections.ObjectModel;
using System.Diagnostics;
using RecambiosWOW.Maui.Models;

namespace RecambiosWOW.Maui.ViewModels;

public partial class MetricsDashboardViewModel : ObservableObject
{
    private readonly ISearchMetricsService _metricsService;
    private readonly IConnectivity _connectivity;
    private readonly IDispatcherTimer _refreshTimer;
    private CancellationTokenSource _cts;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _hasMigrationAlert;

    [ObservableProperty]
    private ObservableCollection<string> _migrationReasons = new();

    [ObservableProperty]
    private ObservableCollection<ChartDataPoint> _responseTimes = new();

    [ObservableProperty]
    private ObservableCollection<ChartDataPoint> _p95ResponseTimes = new();

    [ObservableProperty]
    private ObservableCollection<ChartDataPoint> _concurrentSearches = new();

    [ObservableProperty]
    private ObservableCollection<ChartDataPoint> _popularSearches = new();

    [ObservableProperty]
    private double _indexSize;

    [ObservableProperty]
    private int _totalRecords;

    [ObservableProperty]
    private double _cacheHitRate;

    [ObservableProperty]
    private double _avgResponseTime;

    public MetricsDashboardViewModel(
        ISearchMetricsService metricsService,
        IConnectivity connectivity)
    {
        _metricsService = metricsService;
        _connectivity = connectivity;

        _refreshTimer = Application.Current.Dispatcher.CreateTimer();
        _refreshTimer.Interval = TimeSpan.FromSeconds(30);
        _refreshTimer.Tick += RefreshTimer_Tick;
    }

    private async void RefreshTimer_Tick(object sender, EventArgs e)
    {
        await RefreshMetricsAsync();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            HasError = false;
            _cts = new CancellationTokenSource();

            await RefreshMetricsAsync();
            _refreshTimer.Start();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Failed to load metrics data";
            Debug.WriteLine($"Error loading metrics: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshMetricsAsync()
    {
        if (!_connectivity.NetworkAccess.HasFlag(NetworkAccess.Internet))
        {
            HasError = true;
            ErrorMessage = "No internet connection";
            return;
        }

        try
        {
            var metrics = await _metricsService.GetMetricsAsync(TimeSpan.FromHours(24), _cts.Token);

            // Update migration alert
            HasMigrationAlert = metrics.ShouldMigrate;
            MigrationReasons.Clear();
            foreach (var reason in metrics.MigrationReasons)
            {
                MigrationReasons.Add(reason);
            }

            // Update charts
            UpdateChartData(ResponseTimes, metrics.ResponseTimes);
            UpdateChartData(P95ResponseTimes, metrics.P95ResponseTimes);
            UpdateChartData(ConcurrentSearches, metrics.ConcurrentSearches);
            UpdateChartData(PopularSearches, metrics.PopularSearches);

            // Update stats
            IndexSize = metrics.IndexSizeBytes / (1024.0 * 1024.0);
            TotalRecords = metrics.TotalRecords;
            CacheHitRate = metrics.CacheHitRate;
            AvgResponseTime = metrics.AverageResponseTimeMs;
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Failed to refresh metrics";
            Debug.WriteLine($"Error refreshing metrics: {ex}");
        }
    }

    private void UpdateChartData<T>(ObservableCollection<ChartDataPoint> collection, IEnumerable<T> newData)
    {
        collection.Clear();
        foreach (var point in newData)
        {
            collection.Add(new ChartDataPoint(point));
        }
    }

    public void OnAppearing()
    {
        _refreshTimer.Start();
    }

    public void OnDisappearing()
    {
        _refreshTimer.Stop();
        _cts?.Cancel();
    }
}