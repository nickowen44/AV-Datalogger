using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private object _selectedPage;
        private string _selectedPageName;
        public ObservableCollection<string> Pages { get; }
        public object SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (_selectedPage != value)
                {
                    _selectedPage = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SelectedPageName
        {
            get => _selectedPageName;
            set
            {
                if (_selectedPageName != value)
                {
                    _selectedPageName = value;
                    OnPropertyChanged();
                    ChangePage(value);
                }
            }
        }
        public MainViewModel()
        {
            Pages = new ObservableCollection<string> { "Data", "Test"};
            SelectedPageName = "Data"; // Default page
        }

        public void ChangePage(string pageName)
        {
            var services = DependencyInjection.ConfigureServices();
            switch (pageName)
            {
                case "Test":
                    SelectedPage = new TestWindowView();
                    break;
                case "Data":
                    var datapage = new DataView()
                    {
                        DataContext = services.GetRequiredService<DataViewModel>()
                    };
                    SelectedPage = datapage;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Console.WriteLine($"Property Changed: {name}"); // Log property changes
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name.ToString()));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }
                // Dispose unmanaged resources if any

                _disposed = true;
            }
        }
        ~MainViewModel()
        {
            Dispose(false);
        }
    }
}