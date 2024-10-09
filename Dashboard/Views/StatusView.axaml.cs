using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class StatusView : UserControl
{
    private readonly Image? _GPSTabIcon;
    private readonly TextBlock? _GPSTabTextBlock;
    private readonly Image? _RESIcon;
    private readonly Image? _RESTabIcon;
    private readonly TextBlock? _RESTabTextBlock;
    private readonly TextBlock? _RESTextBlock;

    public StatusView()
    {
        InitializeComponent();
        _RESTextBlock = this.FindControl<TextBlock>("RemoteEmergencyStopIndicator");
        _RESIcon = this.FindControl<Image>("ResIcon");
        _RESTabTextBlock = this.FindControl<TextBlock>("ResTab");
        _RESTabIcon = this.FindControl<Image>("ResTabIcon");
        _GPSTabTextBlock = this.FindControl<TextBlock>("GPSTab");
        _GPSTabIcon = this.FindControl<Image>("GPSTabIcon");
        Loaded += StatusViewLoaded;
    }

    private void StatusViewLoaded(object? sender, EventArgs e)
    {
        if (DataContext is StatusViewModel viewModel) viewModel.RESDataUpdated += OnRESUpdate;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRESUpdate(bool RESState)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var passBrush = (SolidColorBrush)Application.Current?.Resources["GrayColour"]!;
            var failBrush = (SolidColorBrush)Application.Current?.Resources["GoldColour"]!;
            if (RESState)
            {
                _RESTextBlock.Background = failBrush;
                _RESIcon.Opacity = 1;
                _RESTabTextBlock.Background = failBrush;
                _RESTabIcon.Opacity = 1;
                _GPSTabTextBlock.Background = failBrush;
                _GPSTabIcon.Opacity = 1;
            }
            else
            {
                _RESTextBlock.Background = passBrush;
                _RESIcon.Opacity = 90;
                _RESTabTextBlock.Background = passBrush;
                _RESTabIcon.Opacity = 90;
                _GPSTabTextBlock.Background = passBrush;
                _GPSTabIcon.Opacity = 90;
            }
        });
    }
}