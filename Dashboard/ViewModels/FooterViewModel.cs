using System;
using System.Timers;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.Serialisation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dashboard.ViewModels;

public partial class FooterViewModel : ViewModelBase
{
    private readonly IDataStore _dataStore;
    private readonly Timer? _heartBeatTimer;
    private readonly ILogger<FooterViewModel> _logger;
    private bool _heartBeatFlicker;

    [ObservableProperty] private string _logMessage = string.Empty;

    public FooterViewModel(IDataStore dataStore, ILogger<FooterViewModel> logger)
    {
        _dataStore = dataStore;
        _logger = logger;

        _dataStore.RawDataUpdated += OnRawDataChanged;
        _dataStore.HeartBeatUpdated += OnHeartbeatStatusChanged;
        _dataStore.ConsoleMessageUpdated += OnLogMessageReceived;

        _logger.LogDebug("FooterViewModel created");

        _heartBeatTimer = new Timer(500);
        _heartBeatTimer.Elapsed += HeartBeatTimerElapsed;
        _heartBeatTimer.AutoReset = false;

        // Finally, flush the log buffer to ensure we display any messages that were logged before the view was created
        _dataStore.FlushLogBuffer();
    }

    public FooterViewModel()
    {
        _dataStore = new DataStore(new NullConnectorFactory(), NullLogger<DataStore>.Instance,
            NullDataSerialisationFactory.Instance);
        _logger = NullLogger<FooterViewModel>.Instance;
    }

    public string CarId => _dataStore.RawData?.CarId ?? "0";
    public string UTCTime => _dataStore.RawData?.UTCTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";

    public string LocalTime =>
        _dataStore.RawData?.UTCTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";

    public IBrush ConnectionColor => ConnectionStatus ? Brushes.Green : Brushes.Red;
    public IBrush HeartBeatColor => HeartBeat && _heartBeatFlicker ? Brushes.OrangeRed : Brushes.Orange;

    private bool HeartBeat => _dataStore.HeartBeat ?? false;
    private bool ConnectionStatus => _dataStore.RawData?.ConnectionStatus ?? false;

    /// <summary>
    ///     Notifies the view that the Footer data has changed.
    /// </summary>
    private void OnRawDataChanged(object? sender, EventArgs e)
    {
        _logger.LogDebug("Data Updated");

        OnPropertyChanged(nameof(CarId));
        OnPropertyChanged(nameof(UTCTime));
        OnPropertyChanged(nameof(LocalTime));
        OnPropertyChanged(nameof(ConnectionColor));
    }

    private void OnHeartbeatStatusChanged(object? sender, bool isReceived)
    {
        if (HeartBeat)
        {
            _logger.LogDebug("Heartbeat received");

            _heartBeatFlicker = true;
            _heartBeatTimer?.Start();
        }

        OnPropertyChanged(nameof(HeartBeatColor));
    }

    private void HeartBeatTimerElapsed(object? sender, EventArgs e)
    {
        _heartBeatFlicker = false;

        OnPropertyChanged(nameof(HeartBeatColor));
    }


    private void OnLogMessageReceived(object? sender, string message)
    {
        LogMessage += $"{message}\n";
    }

    public override void Dispose()
    {
        _dataStore.RawDataUpdated -= OnRawDataChanged;
        _dataStore.HeartBeatUpdated -= OnHeartbeatStatusChanged;
        _dataStore.ConsoleMessageUpdated -= OnLogMessageReceived;

        GC.SuppressFinalize(this);
    }
}