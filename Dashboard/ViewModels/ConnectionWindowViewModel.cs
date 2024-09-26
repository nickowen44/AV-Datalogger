using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Reactive;
using Avalonia.Layout;
using ReactiveUI;

namespace Dashboard.ViewModels;

public class ConnectionWindowViewModel : ReactiveObject
{
        // Properties for visibility of the sections
        private bool _isSerialPortVisible;
        public bool IsSerialPortVisible
        {
            get => _isSerialPortVisible;
            set => this.RaiseAndSetIfChanged(ref _isSerialPortVisible, value);
        }

        private bool _isTCPVisible;
        public bool IsTCPVisible
        {
            get => _isTCPVisible;
            set => this.RaiseAndSetIfChanged(ref _isTCPVisible, value);
        }

        private bool _isFileVisible;
        public bool IsFileVisible
        {
            get => _isFileVisible;
            set => this.RaiseAndSetIfChanged(ref _isFileVisible, value);
        }

        // Available Ports list for the Serial Port combo box
        private ObservableCollection<string> _availablePorts = new ObservableCollection<string>();
        public ObservableCollection<string> AvailablePorts
        {
            get => _availablePorts;
            set => this.RaiseAndSetIfChanged(ref _availablePorts, value);
        }

        // Selected connection type
        private string _selectedConnectionType;
        public string SelectedConnectionType
        {
            get => _selectedConnectionType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedConnectionType, value);
                UpdateVisibility();
            }
        }

        // Command to refresh the list of available serial ports
        public ReactiveCommand<Unit, Unit> RefreshPortsCommand { get; }

        public ConnectionWindowViewModel()
        {
            // Initialize the command to refresh serial ports
            RefreshPortsCommand = ReactiveCommand.Create(RefreshPorts);

            // Initial state: all sections hidden
            IsSerialPortVisible = false;
            IsTCPVisible = false;
            IsFileVisible = false;

            // Load the available serial ports at startup
            RefreshPorts();
        }

        // Method to update visibility based on the selected connection type
        private void UpdateVisibility()
        {
            IsSerialPortVisible = SelectedConnectionType == "Serial Port";
            IsTCPVisible = SelectedConnectionType == "TCP";
            IsFileVisible = SelectedConnectionType == "File (.CSV)";
        }

        // Method to refresh the available serial ports
        private void RefreshPorts()
        {
            AvailablePorts.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                AvailablePorts.Add(port);
            }
        }
    }
