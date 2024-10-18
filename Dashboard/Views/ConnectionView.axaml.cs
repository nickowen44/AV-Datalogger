using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
        ConnectionTypeCombo.SelectionChanged += ConnectionTypeUpdated;
        Loaded += ConnectionViewLoaded;
    }

    private static FilePickerFileType CSV { get; } = new("CSV Files")
    {
        Patterns = new[] { "*.csv" }
    };

    private void ConnectionViewLoaded(object? sender, EventArgs e)
    {
        if (DataContext is ConnectionViewModel viewModel) viewModel.ConnectionChanged += OnConnectionStarted;
    }

    private void OnConnectionStarted(bool connected)
    {
        // Disable the connect button if the serial connection started properly.
        if (connected)
        {
            ConnectButton.Content = "Connected";
            ConnectButton.IsEnabled = false;
            DisconnectButton.IsEnabled = true;
            SaveToFile.IsEnabled = false;
            ConnectionTypeCombo.IsEnabled = false;
            SerialPortCombo.IsEnabled = false;
        }
        else
        {
            ConnectButton.Content = "Connect";
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
            SaveToFile.IsEnabled = true;
            ConnectionTypeCombo.IsEnabled = true;
            SerialPortCombo.IsEnabled = true;
        }
    }

    private async void FileSelectionClicked(object sender, RoutedEventArgs args)
    {
        // Get the current app dir
        var currentDir = Environment.CurrentDirectory;

        // Combine the current app dir with the runs directory
        var runsDir = Path.Combine(currentDir, "runs");

        // Check if the runs directory exists, if not then open the file picker in the current directory
        if (!Directory.Exists(runsDir)) runsDir = currentDir;

        // Get top level from the current control.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Log File",
            AllowMultiple = false,
            FileTypeFilter = new[] { CSV },
            // Open the dialog in the current apps folder
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(runsDir)
        });

        if (files.Count >= 1)
        {
            var filePath = files[0].Path.LocalPath;
            LogFileSelected.Text = filePath;

            if (DataContext is ConnectionViewModel viewModel) viewModel.SelectedFilePath = filePath;

            ConnectButton.IsEnabled = true;
        }
        else
        {
            ConnectButton.IsEnabled = false;
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