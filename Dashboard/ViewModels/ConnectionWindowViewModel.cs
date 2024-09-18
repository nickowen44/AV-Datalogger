using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.ComponentModel;

namespace Dashboard.ViewModels 
{
    internal class ConnectionWindowViewModel : ReactiveObject
    {

        /*private string _selectedConnectionType;
        public string SelectedConnectionType
        {
            get => _selectedConnectionType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedConnectionType, value);
                UpdateVisibility();
            }
        }

        private Avalonia.Controls.Visibility _serialPortVisibility;
        public Avalonia.Controls.Visibility SerialPortVisibility
        {
            get => _serialPortVisibility;
            set => this.RaiseAndSetIfChanged(ref _serialPortVisibility, value);
        }

        private Avalonia.Controls.Visibility _tcpVisibility;
        public Avalonia.Controls.Visibility TCPVisibility
        {
            get => _tcpVisibility;
            set => this.RaiseAndSetIfChanged(ref _tcpVisibility, value);
        }

        private Avalonia.Controls.Visibility _fileVisibility;
        public Avalonia.Controls.Visibility FileVisibility
        {
            get => _fileVisibility;
            set => this.RaiseAndSetIfChanged(ref _fileVisibility, value);
        }

        public ConnectionWindowViewModel()
        {
            // Initial state: Hide all sections
            SerialPortVisibility = Avalonia.Controls.Visibility.Collapsed;
            TCPVisibility = Avalonia.Controls.Visibility.Collapsed;
            FileVisibility = Avalonia.Controls.Visibility.Collapsed;
        }

        private void UpdateVisibility()
        {
            // Hide all sections by default
            SerialPortVisibility = Avalonia.Controls.Visibility.Collapsed;
            TCPVisibility = Avalonia.Controls.Visibility.Collapsed;
            FileVisibility = Avalonia.Controls.Visibility.Collapsed;

            // Show the relevant section based on the connection type
            if (SelectedConnectionType == "Serial Port")
            {
                SerialPortVisibility = Avalonia.Controls.Visibility.Visible;
            }
            else if (SelectedConnectionType == "TCP")
            {
                TCPVisibility = Avalonia.Controls.Visibility.Visible;
            }
            else if (SelectedConnectionType == "File (.CSV)")
            {
                FileVisibility = Avalonia.Controls.Visibility.Visible;
            }
        }*/
    }
}
