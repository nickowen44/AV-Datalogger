using System;
using Dashboard.Connectors;
using Dashboard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dashboard.ViewModels;

public partial class FooterViewModel : ViewModelBase
{
    public bool? HeartBeat => _dataStore.HeartBeat;
    public event Action<bool>? HeartbeatStatusUpdated;
    public event Action<string>? RawMessageUpdated;
    public event Action<bool>? ConnectionUpdate;
    public bool ConnectionStatus => _dataStore.RawData?.ConnectionStatus ?? false;
    public string CarId => _dataStore.RawData?.CarId ?? "0";
    public string RawMessage => _dataStore.RawData?.RawMessage ?? "";
    public string UTCTime => _dataStore.RawData?.UTCTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";
    public string LocalTime => _dataStore.RawData?.UTCTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";

    private readonly IDataStore _dataStore;
    private readonly ILogger<FooterViewModel> _logger;

    public FooterViewModel(IDataStore dataStore, ILogger<FooterViewModel> logger)
    {
        _dataStore = dataStore;
        _logger = logger;

        _dataStore.RawDataUpdated += OnRawDataChanged;
        _dataStore.HeartBeatUpdated += OnHeartbeatStatusChanged;
    }

    public FooterViewModel()
    {
        _dataStore = new DataStore(new DummyConnector(), NullLogger<DataStore>.Instance);
        _logger = NullLogger<FooterViewModel>.Instance;
    }

    /// <summary>
    ///     Notifies the view that the Footer data has changed.
    /// </summary>
    private void OnRawDataChanged(object? sender, EventArgs e)
    {
        _logger.LogDebug("Data Updated");
        
        OnPropertyChanged(nameof(CarId));
        OnPropertyChanged(nameof(UTCTime));
        OnPropertyChanged(nameof(LocalTime));
        OnPropertyChanged(nameof(RawMessage));
        RawMessageUpdated?.Invoke(RawMessage);
        ConnectionUpdate?.Invoke(ConnectionStatus);
    }

    private void OnHeartbeatStatusChanged(object? sender, bool isReceived)
    {
        _logger.LogDebug("Heart Beat change triggered");
        
        OnPropertyChanged(nameof(HeartBeat));
        HeartbeatStatusUpdated?.Invoke(isReceived);
    }

    public override void Dispose()
    {
        _dataStore.RawDataUpdated -= OnRawDataChanged;
        _dataStore.HeartBeatUpdated -= OnHeartbeatStatusChanged;

        GC.SuppressFinalize(this);
    }

}