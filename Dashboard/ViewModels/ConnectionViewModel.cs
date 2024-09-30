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

    public ConnectionViewModel()
    {
        ConnectionTypes = new ObservableCollection<string>(_connectionTemplates);
        _currentConnectionType = ConnectionTypes.First();
        SerialPorts = new ObservableCollection<string>(GetSerialPorts());
        SelectedSerialPort = SerialPorts.FirstOrDefault();
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

    private readonly List<string> _connectionTemplates =
    [
        "Serial Port",
        "IP Address",
        "Log Replay"
    ];
}