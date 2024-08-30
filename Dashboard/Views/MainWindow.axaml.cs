using System;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
    }

    public MainWindow() : this(new MainViewModel()) { }
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        // Dispose the view model when the window is closed, to clean up the data store and connector
        // Otherwise, the connector thread will keep running in the background
        if (DataContext is MainViewModel model) model.Dispose();
    }
}