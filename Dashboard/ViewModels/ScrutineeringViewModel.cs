using System;
using System.IO;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class ScrutineeringViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    private readonly FileSystemWatcher _fileWatcher;

    private string _yamlContent;

    public ScrutineeringViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.DataUpdated += OnDataChanged;
    }

    public ScrutineeringViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());

        // The folder we are in at runtime is net8.0 (AV-Datalogger/Dashboard/bin/Debug/net8.0/Dashboard.exe), as
        // our yaml file is in the resources folder, exit the current folder three times
        var pathDirectory = "../../../Resources/";

        // // Initialize file watcher
        _fileWatcher = new FileSystemWatcher
        {
            // Watch the project directory
            Path = pathDirectory,
            // Watch specifically for yaml files
            Filter = "*.yaml",
            // Trigger on file modifications
            NotifyFilter = NotifyFilters.LastWrite
        };

        // Link the event handler to react when the file changes
        _fileWatcher.Changed += OnFileChanged;

        // Setting this to true starts monitoring the file for changes.
        _fileWatcher.EnableRaisingEvents = true;
        
        // Load the initial YAML data when the ViewModel is created
        LoadYamlData(Path.Combine(pathDirectory, "AV_Inspection_Flow.yaml"));
    }

    // public int AutonomousState => _dataStore.AutonomousState;
    // public int ServiceBrakeState => _dataStore.ServiceBrakeState;
    // public int EmergencyBrakeState => _dataStore.EmergencyBrakeState;
    // public int AutonomousMissionIndicator => _dataStore.AutonomousMissionIndicator;
    public double SteeringAngle => _dataStore.SteeringAngle;
    
    /// <summary>
    ///  Getter and Setter for the yaml content that is read in from the file
    /// </summary>
    public string YamlContent
    {
        get => _yamlContent;
        set
        {
            if (_yamlContent != value)
            {
                _yamlContent = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Handles the cleanup when the view model is no longer needed.
    /// </summary>
    public void Dispose()
    {
        _dataStore.DataUpdated -= OnDataChanged;

        _dataStore.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Load in yaml data from specified file.
    /// </summary>
    /// <param name="filePath">The yaml's filepath</param>
    public void LoadYamlData(string filePath)
    {
        try
        {
            // Load the YAML file as a raw string
            var data = File.ReadAllText(filePath);
            // Set the string content to YamlContent property
            YamlContent = data;
        }
        catch (Exception ex)
        {
            YamlContent = $"Error loading file: {ex.Message}";
        }
    }

    /// <summary>
    ///     Reload the file when change is detected.
    /// </summary>
    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        LoadYamlData(e.FullPath);
    }

    /// <summary>
    ///     Notifies the view that the data has changed.
    /// </summary>
    private void OnDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SteeringAngle));
    }
}