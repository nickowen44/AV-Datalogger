using Avalonia;
using Avalonia.Markup.Xaml;

namespace DashboardTests;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}