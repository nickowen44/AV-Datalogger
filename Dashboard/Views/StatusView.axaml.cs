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
    public StatusView()
    {
        InitializeComponent();
        _RESTextBlock = this.FindControl<TextBlock>("RemoteEmergancyStopindicator");
        _RESIcon = this.FindControl<Image>("ResIcon");
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
        if (_RESTextBlock is null || _RESIcon is null) return;

        Dispatcher.UIThread.Post(() =>
        {
            if (RESState)
            {
                _RESTextBlock.Background = Brushes.Red;
                _RESIcon.Opacity = 1;
            }
            else
            {
                _RESTextBlock.Background = Brushes.Green;
                _RESIcon.Opacity = 90;
            }
        });
    }
}