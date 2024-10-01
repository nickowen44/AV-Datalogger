using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.Utils;

namespace Dashboard.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    
    [ObservableProperty] private string? _currentConnectionType;
    [ObservableProperty] private string? _selectedSerialPort;
    public ObservableCollection<string> ConnectionTypes { get; }
    public ObservableCollection<string> SerialPorts { get; }
    private IDataStore _dataStore;
    public event Action<bool>? ConnectionChanged;
    public ConnectionViewModel(IDataStore dataStore)
    {
        ConnectionTypes = new ObservableCollection<string>(_connectionTemplates);
        _currentConnectionType = ConnectionTypes.First();
        SerialPorts = new ObservableCollection<string>(GetSerialPorts());
        SelectedSerialPort = SerialPorts.FirstOrDefault();
        _dataStore = dataStore;
    }
    
    public ConnectionViewModel()
    {
        ConnectionTypes = new ObservableCollection<string>(_connectionTemplates);
        _currentConnectionType = ConnectionTypes.First();
        SerialPorts = new ObservableCollection<string>(GetSerialPorts());
        SelectedSerialPort = SerialPorts.FirstOrDefault();
        _dataStore = new DataStore(new DummyConnector());
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
        if (_dataStore.startConnection(portName))
        {
            ConnectionChanged?.Invoke(true);
        }
    }

    [RelayCommand]
    public void DisconnectFromSerialPort()
    {
        _dataStore.disconnect();
        ConnectionChanged?.Invoke(false);
        
    }
    private readonly List<string> _connectionTemplates =
    [
        "Serial Port",
        "IP Address",
        "Log Replay"
    ];
}