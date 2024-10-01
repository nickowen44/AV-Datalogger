using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Dashboard.ViewModels;

namespace Dashboard.Views;

public partial class StatusView : UserControl
{
    private readonly TextBlock? _RESTextBlock;
    private readonly Image? _RESIcon;
    private readonly TextBlock? _RESTabTextBlock;
    private readonly Image? _RESTabIcon;
    private readonly TextBlock? _GPSTabTextBlock;
    private readonly Image? _GPSTabIcon;
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
        if (DataContext is StatusViewModel viewModel)
        {
            viewModel.RESDataUpdated += OnRESUpdate;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRESUpdate(bool RESState)
    {

        Dispatcher.UIThread.Post(() =>
        {
            if (RESState)
            {
                _RESTextBlock.Background = Brushes.Red;
                _RESIcon.Opacity = 1;
                _RESTabTextBlock.Background = Brushes.Red;
                _RESTabIcon.Opacity = 1;
                _GPSTabTextBlock.Background = Brushes.Red;
                _GPSTabIcon.Opacity = 1;
            }
            else
            {
                _RESTextBlock.Background = Brushes.Green;
                _RESIcon.Opacity = 90;
                _RESTabTextBlock.Background = Brushes.Green;
                _RESTabIcon.Opacity = 90;
                _GPSTabTextBlock.Background = Brushes.Green;
                _GPSTabIcon.Opacity = 90;
            }
        });
    }
}