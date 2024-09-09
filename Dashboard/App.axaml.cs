using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;

namespace Dashboard;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Setup dependency injection
            var services = DependencyInjection.ConfigureServices();

            // Create an instance of the MainViewModel
            // TODO the yaml doesnt load in w/ dependency injection services.GetRequiredService<ScrutineeringViewModel>()
            var viewModel = new ScrutineeringViewModel();

            desktop.MainWindow = new ScrutineeringWindow
            {
                // DataContext = services.GetRequiredService<ScrutineeringViewModel>()
                DataContext = viewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}