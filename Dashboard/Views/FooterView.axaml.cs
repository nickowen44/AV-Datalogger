using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            viewModel.ConnectionUpdate += OnConnectionUpdate;
            viewModel.HeartbeatStatusUpdated += OnHeartbeatStatusChanged;
        }
    }
    private void OnConnectionUpdate(bool connectionStat)
    {
        Dispatcher.UIThread.Post(() =>
        {

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
    private async void OnHeartbeatStatusChanged(bool outcome)
    {
        // Ensure you are on the UI thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (outcome)
            {
                _heartBeart.Fill = Brushes.OrangeRed;
            }
        });

        // Delay for a brief moment without blocking the UI
        await Task.Delay(500);

        // Return to the UI thread to update the color again
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _heartBeart.Fill = Brushes.Orange;
        });
    }
}
    


