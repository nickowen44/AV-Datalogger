using System;
using Avalonia.Controls;
using Dashboard.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace Dashboard.Views;

public partial class StatusWindow : Window
{
    public StatusWindow()
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