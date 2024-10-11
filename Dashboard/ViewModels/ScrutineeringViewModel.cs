using System;
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Dashboard.Models;
using Microsoft.Extensions.Logging;

namespace Dashboard.ViewModels;

public partial class ScrutineeringViewModel : ViewModelBase
{
    private readonly IDataStore _dataStore;
    private readonly FileSystemWatcher? _fileWatcher;
    private readonly ILogger<ScrutineeringViewModel> _logger;
    private readonly IYamlLoader _yamlLoader;

    [ObservableProperty]
    private YamlData _yamlData = new()
    {
        Steps = new List<StepData>
        {
            new()
            {
                Step = "Loading...", Measurements = new List<string>(),
                Id = "", Title = "", Caution = ""
            }
        },
        Top = "",
        Bottom = ""
    };

    public ScrutineeringViewModel(IDataStore dataStore, IYamlLoader yamlLoader, ILogger<ScrutineeringViewModel> logger)
    {
        _dataStore = dataStore;
        _yamlLoader = yamlLoader;
        _dataStore.AvDataUpdated += OnDataChanged;

        _logger = logger;

        // Dynamically locate the folder where the app is running and read the YAML file.
        // This works as the yaml file has been included in the output directory
        // Follow this to do so for another file:
        // (https://stackoverflow.com/questions/16785369/how-to-include-other-files-to-the-output-directory-in-c-sharp-upon-build)
        var appDirectory = AppContext.BaseDirectory;
        var yamlFilePath = Path.Combine(appDirectory, "Resources", "AV_Inspection_Flow.yaml");

        var directory = Path.GetDirectoryName(yamlFilePath);
        var fileName = Path.GetFileName(yamlFilePath);

        if (!Directory.Exists(directory))
        {
            _logger.LogError("Directory {directory} does not exist", directory);
            return;
        }

        // // Initialize file watcher
        _fileWatcher = new FileSystemWatcher(directory, fileName)
        {
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
        LoadYamlData(yamlFilePath);
    }

    public int AutonomousSystemState => _dataStore.AvStatusData?.AutonomousSystemState ?? 0;
    public bool ServiceBrakeState => _dataStore.AvStatusData?.ServiceBrakeState ?? false;
    public int EmergencyBrakeState => _dataStore.AvStatusData?.EmergencyBrakeState ?? 0;
    public int AutonomousMissionIndicator => _dataStore.AvStatusData?.MissionIndicator ?? 0;
    public double SteeringAngle => _dataStore.AvStatusData?.SteeringAngle.Actual ?? 0;

    /// <summary>
    ///     Load in yaml data from specified file.
    /// </summary>
    /// <param name="filePath">The yaml's filepath</param>
    public void LoadYamlData(string filePath)
    {
        YamlData = _yamlLoader.LoadYamlData(filePath, _logger);
        _logger.LogInformation("Loaded {count} autonomous inspection steps from YAML", YamlData.Steps.Count);
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
        _logger.LogDebug("AV Status data changed");

        OnPropertyChanged(nameof(AutonomousSystemState));
        OnPropertyChanged(nameof(EmergencyBrakeState));
        OnPropertyChanged(nameof(AutonomousMissionIndicator));
        OnPropertyChanged(nameof(ServiceBrakeState));
        OnPropertyChanged(nameof(SteeringAngle));
    }
}