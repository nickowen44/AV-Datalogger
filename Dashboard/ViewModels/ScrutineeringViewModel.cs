﻿using System;
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Dashboard.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dashboard.ViewModels;

public partial class ScrutineeringViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    private readonly FileSystemWatcher _fileWatcher;
    private readonly string _yamlFilePath;

    [ObservableProperty] public YamlData _yamlData;

    public ScrutineeringViewModel(IDataStore dataStore)
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = dataStore;
        _dataStore.AvDataUpdated += OnDataChanged;

        // Dynamically locate the folder where the app is running and read the YAML file.
        // This works as the yaml file has been included in the output directory
        // Follow this to do so for another file:
        // (https://stackoverflow.com/questions/16785369/how-to-include-other-files-to-the-output-directory-in-c-sharp-upon-build)
        var appDirectory = AppContext.BaseDirectory;
        _yamlFilePath = Path.Combine(appDirectory, "Resources", "AV_Inspection_Flow.yaml");

        var directory = Path.GetDirectoryName(_yamlFilePath);
        var fileName = Path.GetFileName(_yamlFilePath);

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
        LoadYamlData(_yamlFilePath);
    }

    public int AutonomousSystemState => _dataStore.AvStatusData?.AutonomousSystemState ?? 0;
    public bool ServiceBrakeState => _dataStore.AvStatusData?.ServiceBrakeState ?? false;
    
    public int EmergencyBrakeState => _dataStore.AvStatusData?.EmergencyBrakeState ?? 1;
    
    public int AutonomousMissionIndicator => _dataStore.AvStatusData?.MissionIndicator ?? 2;
    
    public double SteeringAngle => _dataStore.AvStatusData?.SteeringAngle.Actual ?? 3;

    /// <summary>
    ///     Handles the cleanup when the view model is no longer needed.
    /// </summary>
    public void Dispose()
    {
        _dataStore.AvDataUpdated -= OnDataChanged;

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
            YamlData = yamlData;
            Console.WriteLine("The number of autonomous inspection steps loaded from YAML: " + YamlData.Steps.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);

            YamlData = new YamlData
            {
                Steps = new List<StepData>
                {
                    new()
                    {
                        Step = "Error loading the yaml file please check logs.", Measurements = new List<string>(),
                        Id = "0", Inspection = ""
                    }
                },
                Top = "",
                Bottom = ""
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
        Console.WriteLine("AV Data Updated in ScrutineeringViewModel");

        OnPropertyChanged(nameof(AutonomousSystemState));
        OnPropertyChanged(nameof(EmergencyBrakeState));
        OnPropertyChanged(nameof(AutonomousMissionIndicator));
        OnPropertyChanged(nameof(ServiceBrakeState));
        OnPropertyChanged(nameof(SteeringAngle));
    }
}