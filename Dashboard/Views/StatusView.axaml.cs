using System;
using Avalonia.Controls;
using Dashboard.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace Dashboard.Views;

public partial class StatusView : UserControl
{
    public StatusView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public partial class DummySlider : UserControl
    {
        public DummySlider()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}