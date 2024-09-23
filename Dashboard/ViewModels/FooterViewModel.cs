using System;
using System.Globalization;
using System.Timers;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class FooterViewModel : ViewModelBase
{
    public bool? HeartBeat => _dataStore.HeartBeat;
    public event Action<bool>? HeartbeatStatusUpdated;
    public event Action<string>? RawMessageUpdated;
    public event Action<bool>? ConnectionUpdate;
    private readonly IDataStore _dataStore;
    public bool ConnectionStatus => _dataStore.RawData?.ConnectionStatus ?? false;
    public string CarId => _dataStore.RawData?.CarId ?? "0";
    public string RawMessage => _dataStore.RawData?.RawMessage ?? "";
    public string UTcTime
    {
        // Convert the UTC value from data logger to proper UTC.
        get
        {
            var utcTimeString = _dataStore.RawData?.UTCTime;
            if (string.IsNullOrEmpty(utcTimeString))
            {
                return "Invalid time";
            }
            try
            {
                string datePart = "";
                // Check if the UTC value includes the leading zero for months, if not add.
                if (utcTimeString.Length == 20)
                {
                    datePart = utcTimeString.Substring(1, 4) + "0" + utcTimeString.Substring(5, 3); // Extracts "2024820"
                }
                else
                {
                    datePart = utcTimeString.Substring(1, 8);
                }
                string timePart = utcTimeString.Substring(9, 8);

                DateTime parsedDate = DateTime.ParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture);
                TimeSpan parsedTime = TimeSpan.ParseExact(timePart, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
                DateTime localDateTime = parsedDate.Add(parsedTime);

                // Convert local time to UTC (assuming local time zone)
                DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

                return utcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                return "Invalid format";
            }
        }
    }

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

    /// <summary>
    ///     Notifies the view that the Footer data has changed.
    /// </summary>
    private void OnRawDataChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("Data Updated in FooterViewModel");
        OnPropertyChanged(nameof(CarId));
        OnPropertyChanged(nameof(UTcTime));
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
        // _dataStore.Dispose();

        GC.SuppressFinalize(this);
    }

}