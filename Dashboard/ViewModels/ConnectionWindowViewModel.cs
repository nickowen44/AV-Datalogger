using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.ComponentModel;

namespace Dashboard.ViewModels 
{
    internal class ConnectionWindowViewModel : ReactiveObject
    {

        private string _selectedConnectionType;

        public string SelectedConnectionType
        {
            get => _selectedConnectionType;
            set => this.RaiseAndSetIfChanged(ref _selectedConnectionType, value);
        }

        public ConnectionWindowViewModel()
        {
            // Default to no selection
            SelectedConnectionType = string.Empty;
        }
    }
}

