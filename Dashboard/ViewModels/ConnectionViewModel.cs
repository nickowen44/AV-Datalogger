using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Connectors.Serial;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    
    [ObservableProperty] private string? _currentConnectionType;
    [ObservableProperty] private string? _selectedSerialPort;
    public ObservableCollection<string> ConnectionTypes { get; }
    public ObservableCollection<string> SerialPorts { get; }
    private readonly IDataStore _dataStore;
    public event Action<bool>? ConnectionChanged;

    public ConnectionViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

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
    public void RefreshSerialPorts()
    {
        SerialPorts.Clear();
        foreach (var port in GetSerialPorts())
        {
            SerialPorts.Add(port);
        }
        SelectedSerialPort = SerialPorts.FirstOrDefault();
    }

    [RelayCommand]
    public void ConnectToSerialPort(string portName)
    {
        // TODO: Add checkbox for save to file or not
        var args = new SerialConnectorArgs
        {
            PortName = portName,
            SaveToCsv = false
        };

        if (_dataStore.Connect(args))
        {
            ConnectionChanged?.Invoke(true);
        }
    }

    [RelayCommand]
    public void DisconnectFromSerialPort()
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