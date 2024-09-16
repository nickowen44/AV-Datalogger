using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Dashboard.Views;

public partial class ScrutineeringView : UserControl
{
    public ScrutineeringView()
    {
        InitializeComponent();
    }

    public void Next(object source, RoutedEventArgs args)
    {
        Slides.Next();
    }

    public void Previous(object source, RoutedEventArgs args)
    {
        Slides.Previous();
    }
}