using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Dashboard.Views;

public partial class FooterView : UserControl
{
    public FooterView()
    {
        InitializeComponent();
    }

    private ScrollViewer? ConsoleScrollViewer => ConsoleTextBox.FindDescendantOfType<ScrollViewer>();

    private void ConsoleTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ConsoleScrollViewer is null) return;

        // Check if we are at the bottom of the console, if so scroll to the end
        if (ConsoleScrollViewer.Offset.Y >= ConsoleScrollViewer.ScrollBarMaximum.Y) ConsoleScrollViewer.ScrollToEnd();
    }

    private void ConsoleTextBox_OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Ensure the console is scrolled to the bottom when it is loaded
        ConsoleScrollViewer?.ScrollToEnd();
    }
}