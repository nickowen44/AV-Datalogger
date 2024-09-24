using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class OverviewPageTest
{
    [SetUp]
    public void SetUp()
    {
        _dataStore = new Mock<IDataStore>();
    }

    private Mock<IDataStore> _dataStore;

    [AvaloniaTest]
    public void TestThatStatusPageRendersCorrectly()
    {
        var window = new StatusView
        {
            DataContext = new StatusViewModel(_dataStore.Object)
        };

        Assert.IsNotNull(window);

        // Make sure there is a icon for RES
        var resIcon = window.FindControl<Image>("ResIcon");
        Assert.IsNotNull(resIcon);
        
    }
}

// More tests to be implemented when Data is connected to StatusView Page