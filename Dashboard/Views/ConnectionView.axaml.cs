using System;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class ConnectionView : UserControl
{
    private static FilePickerFileType CSV { get; } = new("CSV Files")
    {
        Patterns = new[] { "*.csv", },
    };
    public ConnectionView()
    {
        InitializeComponent();
        ConnectionTypeCombo.SelectionChanged += ConnectionTypeUpdated;
        Loaded += ConnectionViewLoaded;
    }
    private void ConnectionViewLoaded(object? sender, EventArgs e)
    {
        if (DataContext is ConnectionViewModel viewModel)
        {
            viewModel.ConnectionChanged += OnConnectionStarted;
        }
    }
    private void OnConnectionStarted(bool connected)
    {
        // Disable the connect button if the serial connection started properly.
        if (connected)
        {
            ConnectButton.Content = "Connected";
            ConnectButton.IsEnabled = false;
            DisconnectButton.IsEnabled = true;
        }
        else
        {
            ConnectButton.Content = "Connect";
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
        }
    }
    private async void FileSelectionClicked(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select Log File",
            AllowMultiple = false,
            FileTypeFilter = new[] { CSV, }
        });

        if (files.Count >= 1)
        {
            var filePath = files[0].Path.LocalPath;
            LogFileSelected.Text = filePath;
        }
    }

    private void ConnectionTypeUpdated(object? sender, RoutedEventArgs args)
    {
        var comboBox = sender as ComboBox;
        if (comboBox?.SelectedItem is string selectedType)
        {
            // Set visibility of sections based on the selected connection type
            SerialPortSection.IsVisible = false;
            TCPSection.IsVisible = false;
            FileSection.IsVisible = false;
            ConnectButton.IsEnabled = false;
            DisconnectButton.IsEnabled = false;
            if (selectedType == "IP Address")
            {
                TCPSection.IsVisible = true;
            }
            else if (selectedType == "Log Replay")
            {
                FileSection.IsVisible = true;
            }
            else if (selectedType == "Serial Port")
            {
                SerialPortSection.IsVisible = true;
                if (ConnectButton.Content != "Connected")
                {
                    DisconnectButton.IsEnabled = false;
                    ConnectButton.IsEnabled = true;
                }
                else
                {
                    DisconnectButton.IsEnabled = true;
                    ConnectButton.IsEnabled = false;
                }
            }
        }
    }
}