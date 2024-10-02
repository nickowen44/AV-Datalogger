using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class FooterViewModel : ViewModelBase
{
    private readonly IDataStore _dataStore;

    public FooterViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.RawDataUpdated += OnRawDataChanged;
        _dataStore.HeartBeatUpdated += OnHeartbeatStatusChanged;
    }

    public FooterViewModel()
    {
        _dataStore = new DataStore(new DummyConnector());
    }

    public bool? HeartBeat => _dataStore.HeartBeat;
    public bool ConnectionStatus => _dataStore.RawData?.ConnectionStatus ?? false;
    public string CarId => _dataStore.RawData?.CarId ?? "0";
    public string RawMessage => _dataStore.RawData?.RawMessage ?? "";
    public string UTCTime => _dataStore.RawData?.UTCTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";

    public string LocalTime =>
        _dataStore.RawData?.UTCTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "Invalid Time";

    public event Action<bool>? HeartbeatStatusUpdated;
    public event Action<string>? RawMessageUpdated;
    public event Action<bool>? ConnectionUpdate;

    /// <summary>
    ///     Notifies the view that the Footer data has changed.
    /// </summary>
    private void OnRawDataChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("Data Updated in FooterViewModel");
        OnPropertyChanged(nameof(CarId));
        OnPropertyChanged(nameof(UTCTime));
        OnPropertyChanged(nameof(LocalTime));
        OnPropertyChanged(nameof(RawMessage));
        RawMessageUpdated?.Invoke(RawMessage);
        ConnectionUpdate?.Invoke(ConnectionStatus);
    }

    private void OnHeartbeatStatusChanged(object? sender, bool isReceived)
    {
        Console.WriteLine("Heart Beat change triggered");
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