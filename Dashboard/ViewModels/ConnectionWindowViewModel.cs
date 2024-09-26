using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace Dashboard.ViewModels;

public enum ConnectionType
{
    SerialPort,
    TCP,
    File
}

public class ConnectionWindowViewModel : ReactiveObject
{
    private bool _isFileVisible;

    // Visibility for each section
    private bool _isSerialPortVisible;

    private bool _isTCPVisible;

    // Enum for Connection Types
    private ConnectionType _selectedConnectionType;

    // Ports
    private string _selectedPort;

    public ConnectionWindowViewModel()
    {
        // Initialize available connection types
        ConnectionTypes =
            new ObservableCollection<ConnectionType>(Enum.GetValues(typeof(ConnectionType)).Cast<ConnectionType>());

        // Initialize the available ports and set the selected port
        AvailablePorts = new ObservableCollection<string>();
        LoadAvailablePorts();

        // Command to refresh available ports
        RefreshPortsCommand = ReactiveCommand.Create(RefreshPorts);

        // Initialize to hide all sections
        IsSerialPortVisible = false;
        IsTCPVisible = false;
        IsFileVisible = false;
        UpdateVisibility();
    }

    public ConnectionType SelectedConnectionType
    {
        get => _selectedConnectionType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedConnectionType, value);
            UpdateVisibility();
        }
    }

    // Property to hold the available connection types
    public ObservableCollection<ConnectionType> ConnectionTypes { get; }

    public bool IsSerialPortVisible
    {
        get => _isSerialPortVisible;
        set => this.RaiseAndSetIfChanged(ref _isSerialPortVisible, value);
    }

    public bool IsTCPVisible
    {
        get => _isTCPVisible;
        set => this.RaiseAndSetIfChanged(ref _isTCPVisible, value);
    }

    public bool IsFileVisible
    {
        get => _isFileVisible;
        set => this.RaiseAndSetIfChanged(ref _isFileVisible, value);
    }

    public string SelectedPort
    {
        get => _selectedPort;
        set => this.RaiseAndSetIfChanged(ref _selectedPort, value);
    }

    public ObservableCollection<string> AvailablePorts { get; }

    // Command to refresh ports
    public ReactiveCommand<Unit, Unit> RefreshPortsCommand { get; }

    private void LoadAvailablePorts()
    {
        // Populate the available ports
        AvailablePorts.Clear();
        foreach (var port in SerialPort.GetPortNames()) AvailablePorts.Add(port);

        // Set the first port as default if available
        if (AvailablePorts.Any()) SelectedPort = AvailablePorts.First();
    }

    private void RefreshPorts()
    {
        LoadAvailablePorts();
    }

    private void UpdateVisibility()
    {
        // Reset all visibilities
        IsSerialPortVisible = false;
        IsTCPVisible = false;
        IsFileVisible = false;

        // Set the visibility based on the selected connection type
        switch (SelectedConnectionType)
        {
            case ConnectionType.SerialPort:
                IsSerialPortVisible = true;
                break;
            case ConnectionType.TCP:
                IsTCPVisible = true;
                break;
            case ConnectionType.File:
                IsFileVisible = true;
                break;
        }
    }
}