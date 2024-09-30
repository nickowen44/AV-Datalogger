using System.IO;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace Dashboard.Views;

public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
        ConnectionTypeCombo.SelectionChanged += ConnectionTypeUpdated;
    }
    private async void FileSelectionClicked(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select Log File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var filePath = files[0].Path.LocalPath;
            LogFileSelected.Text = filePath;
        }

    }

    private void ConnectionTypeUpdated(object sender, RoutedEventArgs args)
    {
        var comboBox = sender as ComboBox;
        if (comboBox?.SelectedItem is string selectedType)
        {
            // Set visibility of sections based on the selected connection type
            SerialPortSection.IsVisible = false;
            TCPSection.IsVisible = false;
            FileSection.IsVisible = false;
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
            }
        }
    }
}