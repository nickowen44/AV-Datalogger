using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dashboard.ViewModels;

namespace Dashboard.Views
{
    public partial class MainView : UserControl
    {
        private MainView _mainViewModel;
        public MainView()
        {
            InitializeComponent();
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && DataContext is MainViewModel viewModel)
            {
                string selectedItem = listBox.SelectedItem as string;
                if (!string.IsNullOrEmpty(selectedItem))
                {
                    viewModel.ChangePage(selectedItem);
                    Console.WriteLine($"DataContext2: {DataContext}");
                }
                Console.WriteLine($"DataContext3: {DataContext}");
            }
        }
        
    }
    
}