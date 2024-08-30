using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;

// using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.ViewModels
{
    public partial class MainViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Obserable variables that utilize ObserableObject for reactive commands.
        /// </summary>

        [ObservableProperty] 
        private ViewModelBase _currentPage;
        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;
        private Dictionary<string,ViewModelBase> _vms;
        private  IServiceProvider _serviceProvider;
        
        /// <summary>
        ///  Main constructor, creates a Dict with the ViewModels and their names,
        /// _serviceProvider to allow dependency injection for ViewModels as needed,
        /// and sets the first page.
        /// Bug : When DataViewModel is the first page, results in memory leak? 
        /// </summary>
        public MainViewModel()
        {
            _vms = new Dictionary<string,ViewModelBase>();
            _serviceProvider = DependencyInjection.ConfigureServices();
            Items = new ObservableCollection<ListItemTemplate>(_templates);
            SelectedListItem = Items.First(vm => vm.ModelType == typeof(TestWindowViewModel));
        }

        
        /// <summary>
        /// Command to react to navigation button presses.
        /// Checks if the view has already been activated,
        /// If not created then create and change.
        /// </summary>
        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (!_vms.ContainsKey(value.Label)){
                _vms[value.Label] = (ViewModelBase)ActivatorUtilities.CreateInstance(_serviceProvider, value.ModelType);
            }
            CurrentPage = _vms[value.Label];
            
        }
        
        /// <summary>
        /// Simple list template to tie the shorten names to ViewModels.
        /// </summary>
        private readonly List<ListItemTemplate> _templates =
        [
            new ListItemTemplate(typeof(DataViewModel),  "Data"),
            new ListItemTemplate(typeof(TestWindowViewModel),  "Test"),
      
        ];
        public ObservableCollection<ListItemTemplate> Items { get; }
        
        /// <summary>
        /// Iterates through _vms to dispose of the ViewModels.
        /// </summary>
        public override void Dispose()
        {
            foreach(var item in _vms)
            {
                item.Value.Dispose();
            }
            GC.SuppressFinalize(this);
        }

    }
}