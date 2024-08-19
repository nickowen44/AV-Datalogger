using Avalonia.Headless.NUnit;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class DemoTest
{
    private Mock<IDataStore> _dataStore;
    private Mock<IConnector> _connector;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
        _connector = new Mock<IConnector>();
    }

    [AvaloniaTest]
    public void TestMainWindow()
    {
        // Arrange
        var window = new MainWindow();
        window.DataContext = new MainViewModel(_dataStore.Object, _connector.Object);

        // Act
        window.Show();
        
        Console.WriteLine(window.find);

        // Assert
        Assert.IsNotNull(window);
        Assert.IsInstanceOf<MainWindow>(window);
        
    }
}