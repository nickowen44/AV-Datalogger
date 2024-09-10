using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;



namespace Dashboard.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Obserable variables that utilize ObserableObject for reactive commands.
        /// </summary>
        [ObservableProperty] 
        private object _currentPage;
        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;
        
        /// <summary>
        /// Dictionary of the Views with their ViewModel and shorthand names.
        /// </summary>
        private Dictionary<string, (object View, object? ViewModel)> _views;
        
        /// <summary>
        /// Service Provider so the ViewModels can be created with their appropriate services required.
        /// </summary>
        private  IServiceProvider _serviceProvider;
        
        /// <summary>
        ///  Main constructor, creates a Dict with the ViewModels and their names,
        /// _serviceProvider to allow dependency injection for ViewModels as needed,
        /// and sets the first page.
        /// Bug : When DataViewModel is the first page, results in memory leak? 
        /// </summary>
        public MainViewModel()
        {
            _views = new Dictionary<string, (object, object?)>();
            _serviceProvider = DependencyInjection.ConfigureServices();
            Items = new ObservableCollection<ListItemTemplate>(_templates);
            SelectedListItem = Items.First(vm => vm.View == typeof(ConnectionView));
        }

        
        /// <summary>
        /// Command to react to navigation button presses.
        /// Checks if the view has already been activated,
        /// If not created then create and change.
        /// </summary>
        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            
            // Check if the view already exist in the dict, if it does not then create it and the appropriate ViewModel as needed.
            if (!_views.ContainsKey(value.Label))
            {
                // Create the view and an object to hold the ViewModel if there is one.
                var viewInstance = Activator.CreateInstance(value.View);
                object? viewModelInstance = null;
                if (value.ViewModel != null)
                {
                    // Create the ViewModel with their required services and set the DataContext.
                    viewModelInstance = (ViewModelBase)ActivatorUtilities.CreateInstance(_serviceProvider, value.ViewModel);
                    (viewInstance as Control).DataContext = viewModelInstance;
                }
                _views[value.Label] = (viewInstance, viewModelInstance);
            }

            CurrentPage = _views[value.Label].View; 
            
        }
        
        /// <summary>
        /// Simple list template to tie the shorten names to ViewModels and Views
        /// ViewModel can be Null as not all Views will require one e.g Help/About
        /// </summary>
        private readonly List<ListItemTemplate> _templates =
        [
            // Currently a bunch of dummy views 
            new ListItemTemplate(typeof(ConnectionView), null, "Connection"),
            new ListItemTemplate(typeof(SetupView), null, "Setup"),
            new ListItemTemplate(typeof(StatusView), typeof(DataViewModel), "Status"),
            new ListItemTemplate(typeof(ConsoleView), null, "Console"),
            new ListItemTemplate(typeof(AboutView), null, "About"),
            new ListItemTemplate(typeof(HelpView), null, "Help"),
            new ListItemTemplate(typeof(DataView), typeof(DataViewModel), "Data"),
            new ListItemTemplate(typeof(TestWindowView), null, "Test"),
            
        ];
        public ObservableCollection<ListItemTemplate> Items { get; }
        
        /// <summary>
        /// Iterates through _views to dispose of the ViewModels as needed.
        /// </summary>
        public override void Dispose()
        {
            foreach (var entry in _views)
            {
                var viewModel = entry.Value.ViewModel;
                if (viewModel is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            GC.SuppressFinalize(this);
        }

    }
}