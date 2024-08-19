using System;
using Avalonia.Controls;
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

        if (DataContext is MainViewModel model) model.Dispose();
    }
}