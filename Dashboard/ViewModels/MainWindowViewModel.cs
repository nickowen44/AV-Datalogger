﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Dashboard.Models;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    ///     Service Provider so the ViewModels can be created with their appropriate services required.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Simple list template to tie the shorten names to ViewModels and Views
    ///     ViewModel can be Null as not all Views will require one e.g. Help/About
    ///     Rendered in order left to right.
    /// </summary>
    private readonly List<ListItemTemplate> _templates =
    [
        // Currently a bunch of dummy views
        // When adding to the list use typeof(New Item View/ViewModel).
        new(typeof(ConnectionView), null, "Connection"),
        new(typeof(SetupView), null, "Setup"),
        new(typeof(StatusView), typeof(StatusViewModel), "Status"),
        new(typeof(ConsoleView), null, "Console"),
        new(typeof(AboutView), typeof(AboutViewModel), "About"),
        new(typeof(HelpView), null, "Help"),
        new(typeof(ScrutineeringView), typeof(ScrutineeringViewModel), "Scrutineering"),
        new(typeof(DataView), typeof(DataViewModel), "Data")
    ];

    /// <summary>
    ///     Simple list template to tie the shorten names to ViewModels and Views
    ///     ViewModel can be Null as not all Views will require one e.g. Help/About
    ///     Rendered in order left to right.
    /// </summary>
    private readonly List<ListItemTemplate> _templates =
    [
        // Currently a bunch of dummy views
        // When adding to the list use typeof(New Item View/ViewModel).
        new(typeof(ConnectionView), typeof(ConnectionViewModel), "Connection"),
        new(typeof(SetupView), null, "Setup"),
        new(typeof(StatusView), typeof(StatusViewModel), "Status"),
        new(typeof(ConsoleView), null, "Console"),
        new(typeof(AboutView), typeof(AboutViewModel), "About"),
        new(typeof(HelpView), null, "Help"),
        new(typeof(ScrutineeringView), typeof(ScrutineeringViewModel), "Scrutineering"),
        new(typeof(DataView), typeof(DataViewModel), "Data")
    ];

    /// <summary>
    ///     Dictionary of the Views with their ViewModel and shorthand names.
    /// </summary>
    private readonly Dictionary<string, (UserControl View, ViewModelBase? ViewModel)> _views;

    /// <summary>
    ///     Observable variables that utilize ObservableObject for reactive commands.
    /// </summary>
    [ObservableProperty] private object? _currentPage;

    [ObservableProperty] private UserControl? _footer;

    [ObservableProperty] private ListItemTemplate _selectedListItem;

    /// <summary>
    ///     Main constructor, creates a Dict with the ViewModels and their names,
    ///     and sets up the first page
    /// </summary>
    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _views = new Dictionary<string, (UserControl, ViewModelBase?)>();
        Items = new ObservableCollection<ListItemTemplate>(_templates);
        SelectedListItem = Items.First();
        _footer = new FooterView
        {
            DataContext = _serviceProvider.GetRequiredService<FooterViewModel>()
        };
    }

    public ObservableCollection<ListItemTemplate> Items { get; }
    public ObservableCollection<ListItemTemplate> Items { get; }

    /// <summary>
    ///     Command to react to navigation button presses.
    ///     Checks if the view has already been activated,
    ///     If not created then create and change.
    /// </summary>
    partial void OnSelectedListItemChanged(ListItemTemplate value)
    {
        // Check if the view already exist in the dict, if it does not then create it and the appropriate ViewModel as needed.
        Console.WriteLine("Navigation Bar Item Selection Changed to {0}.", value.Label);
        if (!_views.ContainsKey(value.Label))
        {
            // Create the view.
            var viewInstance = (UserControl?)Activator.CreateInstance(value.View);
            if (viewInstance == null)
            {
                Console.WriteLine("Failed to load view instance {0}.", value.Label);
            }
            else
            {
                ViewModelBase? viewModelInstance = null;
                if (value.ViewModel != null)
                {
                    // Create the ViewModel with their required services and set the DataContext.
                    viewModelInstance =
                        (ViewModelBase)ActivatorUtilities.CreateInstance(_serviceProvider, value.ViewModel);
                    viewInstance.DataContext = viewModelInstance;
                    Console.WriteLine("Created ViewModel instance: {0}.", value.ViewModel);
                }

                Console.WriteLine("Added View {0} to _views dict.", value.Label);
                _views[value.Label] = (viewInstance, viewModelInstance);
            }
        }

        CurrentPage = _views[value.Label].View;
        Console.WriteLine("Changed CurrentPage to View {0}", value.Label);
    }

    /// <summary>
    ///     Iterates through _views to dispose of the ViewModels as needed.
    /// </summary>
    public override void Dispose()
    {
        Console.WriteLine("Dispose for MainViewModel Triggered");

        _serviceProvider.GetService<IDataStore>()?.Dispose();

        GC.SuppressFinalize(this);
    }
}