using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class ScrutineeringView : UserControl
{
    public ScrutineeringWindow()
    {
        InitializeComponent();
    }

    public void Next(object source, RoutedEventArgs args)
    {
        slides.Next();
    }

    public void Previous(object source, RoutedEventArgs args)
    {
        slides.Previous();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Dispose the view model when the window is closed, to clean up the data store and connector
        // Otherwise, the connector thread will keep running in the background
        if (DataContext is ScrutineeringViewModel model) model.Dispose();
    }
}