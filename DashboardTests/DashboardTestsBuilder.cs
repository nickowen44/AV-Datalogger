using Avalonia;
using Avalonia.Headless;
using DashboardTests;

[assembly: AvaloniaTestApplication(typeof(DashboardTestsBuilder))]

namespace DashboardTests;

public class DashboardTestsBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}