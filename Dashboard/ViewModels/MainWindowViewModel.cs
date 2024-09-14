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
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Observable variables that utilize ObservableObject for reactive commands.
        /// </summary>
        [ObservableProperty]
        private object? _currentPage;
        [ObservableProperty]
        private ListItemTemplate _selectedListItem;
        /// <summary>
        /// Dictionary of the Views with their ViewModel and shorthand names.
        /// </summary>
        private readonly Dictionary<string, (UserControl View, ViewModelBase? ViewModel)> _views;

        /// <summary>
        /// Service Provider so the ViewModels can be created with their appropriate services required.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///  Main constructor, creates a Dict with the ViewModels and their names,
        /// _serviceProvider to allow dependency injection for ViewModels as needed,
        /// and sets the first page
        /// </summary>
        public MainWindowViewModel()
        {
            _views = new Dictionary<string, (UserControl, ViewModelBase?)>();
            _serviceProvider = DependencyInjection.ConfigureServices();
            Items = new ObservableCollection<ListItemTemplate>(_templates);
            SelectedListItem = Items.First();
        }


        /// <summary>
        /// Command to react to navigation button presses.
        /// Checks if the view has already been activated,
        /// If not created then create and change.
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
        /// Simple list template to tie the shorten names to ViewModels and Views
        /// ViewModel can be Null as not all Views will require one e.g. Help/About
        /// Rendered in order left to right.
        /// </summary>
        private readonly List<ListItemTemplate> _templates =
        [
            // Currently a bunch of dummy views
            // When adding to the list use typeof(New Item View/ViewModel).
            new ListItemTemplate(typeof(ConnectionView), null, "Connection"),
            new ListItemTemplate(typeof(SetupView), null, "Setup"),
            new ListItemTemplate(typeof(StatusView), null, "Status"),
            new ListItemTemplate(typeof(ConsoleView), null, "Console"),
            new ListItemTemplate(typeof(AboutView), null, "About"),
            new ListItemTemplate(typeof(HelpView), null, "Help"),
            new ListItemTemplate(typeof(ScrutineeringView), null, "Scrutineering"),
            new ListItemTemplate(typeof(DataView), typeof(DataViewModel), "Data"),
            new ListItemTemplate(typeof(TestWindowView), null, "Test"),
        ];
        public ObservableCollection<ListItemTemplate> Items { get; }

        /// <summary>
        /// Iterates through _views to dispose of the ViewModels as needed.
        /// </summary>
        public override void Dispose()
        {
            Console.WriteLine("Dispose for MainViewModel Triggered");
            foreach (var entry in _views)
            {
                var viewModel = entry.Value.ViewModel;
                if (viewModel is IDisposable disposable)
                {
                    disposable.Dispose();
                    Console.WriteLine("Dispose for {0} Triggered", viewModel.GetType());
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}