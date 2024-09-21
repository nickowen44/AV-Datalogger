using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class FooterView : UserControl
{
    private readonly TextBox? _consoleTextBox ;
    public FooterView()
    {
        InitializeComponent();
        _consoleTextBox = this.FindControl<TextBox>("ConsoleTextBox");
        Loaded += FooterView_Loaded;
    }

    private void FooterView_Loaded(object? sender, EventArgs e)
    {
        if (DataContext is FooterViewModel viewModel)
        {
            viewModel.RawMessageUpdated += OnRawMessageUpdated;
        }
    }

    private void OnRawMessageUpdated(string newMessage)
    {
        Dispatcher.UIThread.Post(() =>
        {
            {
                var scrollViewer = _consoleTextBox.GetVisualDescendants()
                    .OfType<ScrollViewer>()
                    .FirstOrDefault();
                _consoleTextBox.Text += $"{newMessage}\n";
               
                if (scrollViewer != null)
                {
                    // Check if the ScrollViewer is already at the bottom
                    bool isAtBottom = scrollViewer.Offset.Y >= scrollViewer.ScrollBarMaximum.Y;
                    // Scroll to the end if it was at the bottom
                    if (isAtBottom)
                    {
                        scrollViewer.ScrollToEnd();
                    }
                }
            }
        }, DispatcherPriority.Background);
    }
}
    


