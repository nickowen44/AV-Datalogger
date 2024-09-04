using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class ScrutineeringWindow : Window
{
    public ScrutineeringWindow()
    {
        InitializeComponent();
    }

    public void Next(object source, RoutedEventArgs args)
    {
        // TODO
        // if (checks)
        // {
        slides.Next();
        // }
    }

    public void Previous(object source, RoutedEventArgs args)
    {
        slides.Previous();
    }

    // public void CheckPassFail(object sender, RoutedEventArgs args)
    // {
    //     message.Text = "Passed checks!";
    // }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Dispose the view model when the window is closed, to clean up the data store and connector
        // Otherwise, the connector thread will keep running in the background
        if (DataContext is ScrutineeringViewModel model) model.Dispose();
    }
}