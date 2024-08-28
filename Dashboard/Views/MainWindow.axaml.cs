using System;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    
        // Dispose the view model when the window is closed, to clean up the data store and connector
        // Otherwise, the connector thread will keep running in the background
        if (DataContext is DataViewModel model) model.Dispose();
    }
}