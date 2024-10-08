using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Connectors;
using Dashboard.Connectors.Csv;
using Dashboard.Connectors.Serial;
using Dashboard.Models;
using Dashboard.Serialisation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dashboard.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{

    [ObservableProperty] private string? _currentConnectionType;
    [ObservableProperty] private string? _selectedSerialPort;
    [ObservableProperty] private bool _saveToFile;

    public string? SelectedFilePath { get; set; }
    public ObservableCollection<string> ConnectionTypes { get; }
    public ObservableCollection<string> SerialPorts { get; }
    private readonly IDataStore _dataStore;
    public event Action<bool>? ConnectionChanged;

    [ActivatorUtilitiesConstructor]
    public ConnectionViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        SerialPorts = new ObservableCollection<string>(GetSerialPorts());
        SelectedSerialPort = SerialPorts.FirstOrDefault();

        ConnectionTypes = new ObservableCollection<string>(_connectionTemplates);
        _currentConnectionType = ConnectionTypes.First();
    }

    public ConnectionViewModel()
    {
        _dataStore = new DataStore(new NullConnectorFactory(), NullLogger<DataStore>.Instance,
            NullDataSerialisationFactory.Instance);

        SerialPorts = new ObservableCollection<string>(GetSerialPorts());
        SelectedSerialPort = SerialPorts.FirstOrDefault();

        ConnectionTypes = new ObservableCollection<string>(_connectionTemplates);
        _currentConnectionType = ConnectionTypes.First();
    }

    private static string[] GetSerialPorts()
    {
        return SerialPort.GetPortNames();
    }

    [RelayCommand]
    private void RefreshSerialPorts()
    {
        SerialPorts.Clear();
        foreach (var port in GetSerialPorts())
        {
            SerialPorts.Add(port);
        }
        SelectedSerialPort = SerialPorts.FirstOrDefault();
    }

    [RelayCommand]
    private void Connect()
    {
        IConnectorArgs args = CurrentConnectionType switch
        {
            "Serial Port" => new SerialConnectorArgs
            {
                PortName = SelectedSerialPort ?? string.Empty,
                SaveToCsv = SaveToFile
            },
            "IP Address" => throw new NotImplementedException(),
            "Log Replay" => new CsvConnectorArgs
            {
                FilePath = SelectedFilePath ?? string.Empty
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        if (_dataStore.Connect(args)) ConnectionChanged?.Invoke(true);
    }

    [RelayCommand]
    private void Disconnect()
    {
        _dataStore.Disconnect();

        ConnectionChanged?.Invoke(false);
    }

    private readonly List<string> _connectionTemplates =
    [
        "Serial Port",
        "IP Address",
        "Log Replay"
    ];
}