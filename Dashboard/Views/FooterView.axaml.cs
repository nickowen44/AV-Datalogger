using System;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class FooterView : UserControl
{
    private readonly TextBox? _consoleTextBox ;
    private readonly Ellipse? _heartBeart ;
    private readonly Ellipse? _connection ;
    public FooterView()
    {
        InitializeComponent();
        _consoleTextBox = this.FindControl<TextBox>("ConsoleTextBox");
        _heartBeart = this.FindControl<Ellipse>("HeartBeat");
        _connection = this.FindControl<Ellipse>("ConnectionIndicator");
        Loaded += FooterView_Loaded;
    }

    private void FooterView_Loaded(object? sender, EventArgs e)
    {
        if (DataContext is FooterViewModel viewModel)
        {
            viewModel.RawMessageUpdated += OnRawMessageUpdated;
            viewModel.RawMessageUpdateConnection += OnRawMessageUpdateConnection;
        }
    }
    private void OnRawMessageUpdateConnection(bool connectionStat)
    {
        Dispatcher.UIThread.Post(() =>
        {
            // Append the new message to the TextBox
            if (connectionStat)
            {
                _connection.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                _connection.Fill = new SolidColorBrush(Colors.Red);
            }
        });
    }
    private void OnRawMessageUpdated(string newMessage)
    {
        Dispatcher.UIThread.Post(() =>
        {
            // Append the new message to the TextBox
            _consoleTextBox.Text += $"{newMessage}\n";

            // Flash the HeartBeat Ellipse to Red
            if (_heartBeart != null)
            {
                _heartBeart.Fill = Brushes.OrangeRed;
            }
        });

        // Wait for 500 milliseconds before reverting the color
        Thread.Sleep(500);

        Dispatcher.UIThread.Post(() =>
        {
            // Revert the color back to Orange
            if (_heartBeart != null)
            {
                _heartBeart.Fill = Brushes.Orange;
            }

            // Scroll the TextBox if necessary
            var scrollViewer = _consoleTextBox?.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault();

            if (scrollViewer != null)
            {
                bool isAtBottom = scrollViewer.Offset.Y >= scrollViewer.ScrollBarMaximum.Y;
                if (isAtBottom)
                {
                    scrollViewer.ScrollToEnd();
                }
            }
        });
    }
}
    


