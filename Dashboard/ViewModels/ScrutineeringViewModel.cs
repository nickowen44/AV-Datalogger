using System;
using System.Collections.Generic;
using System.IO;
using Dashboard.Connectors;
using Dashboard.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dashboard.ViewModels;

public class ScrutineeringViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    private readonly FileSystemWatcher _fileWatcher;

    private List<StepData> _steps;

    public ScrutineeringViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.DataUpdated += OnDataChanged;
    }

    public ScrutineeringViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());

        Steps = new List<StepData>();

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
    ///     Getter and Setter for the flow of steps for autonomous vehicle inspection
    /// </summary>
    public List<StepData> Steps
    {
        get => _steps;
        set
        {
            if (_steps != value)
            {
                _steps = value;
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
            // Read file as a raw string
            var yamlContent = File.ReadAllText(filePath);

            // Parse into a list of steps
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yamlData = deserializer.Deserialize<YamlData>(yamlContent);

            // Update the Steps collection with parsed steps
            // Note: We need to use a thread here to prevent weirdness if the update were to happen on the thread used by
            //       the file watcher.
            // Dispatcher.UIThread.Post(() => { Steps = yamlData.Steps; });
            // TODO here
            Steps = yamlData.Steps;
            Console.WriteLine("The number of autonomous inspection steps loaded from YAML: " + Steps.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Steps = new List<StepData>
            {
                new() { Step = "Error loading the yaml file please check logs.", Measurements = new List<string>() }
            };
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