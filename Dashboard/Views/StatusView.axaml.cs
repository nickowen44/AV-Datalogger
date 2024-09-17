using Avalonia.Controls;
using Dashboard.ViewModels;
using Avalonia.Markup.Xaml;

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
}