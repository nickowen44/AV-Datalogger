using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class DemoTest
{
    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }

    private Mock<IDataStore> _dataStore;


    [AvaloniaTest]
    [Ignore("Waiting on ADL-32 PR as currently the main window is scurtineering view")]
    public void TestMainWindow()
    {
        // Arrange
        var window = new MainWindow
        {
            DataContext = new MainViewModel(_dataStore.Object)
        };

        // Act
        window.Show();

        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<MainWindow>());

        var speed = window.FindControl<TextBlock>("SpeedDisplay");
        var steeringAngle = window.FindControl<TextBlock>("SteeringAngleDisplay");
        var brakePressure = window.FindControl<TextBlock>("BrakePressureDisplay");

        Assert.Multiple(() =>
        {
            Assert.That(speed, Is.Not.Null);
            Assert.That(steeringAngle, Is.Not.Null);
            Assert.That(brakePressure, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(speed.Text, Is.EqualTo("Speed: 0"));
            Assert.That(steeringAngle.Text, Is.EqualTo("Steering Angle: 0"));
            Assert.That(brakePressure.Text, Is.EqualTo("Brake Pressure: 0"));
        });
    }

    [AvaloniaTest]
    [Ignore("Waiting on ADL-32 PR as currently the main window is scurtineering view")]
    public void TestDataStore()
    {
        // Arrange
        _dataStore.SetupGet(x => x.Speed).Returns(50);
        _dataStore.SetupGet(x => x.SteeringAngle).Returns(90);
        _dataStore.SetupGet(x => x.BrakePressure).Returns(100);

        var window = new MainWindow
        {
            DataContext = new MainViewModel(_dataStore.Object)
        };

        // Act
        window.Show();

        // Assert
        var speed = window.FindControl<TextBlock>("SpeedDisplay");
        var steeringAngle = window.FindControl<TextBlock>("SteeringAngleDisplay");
        var brakePressure = window.FindControl<TextBlock>("BrakePressureDisplay");

        Assert.Multiple(() =>
        {
            Assert.That(speed, Is.Not.Null);
            Assert.That(steeringAngle, Is.Not.Null);
            Assert.That(brakePressure, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(speed.Text, Is.EqualTo("Speed: 50"));
            Assert.That(steeringAngle.Text, Is.EqualTo("Steering Angle: 90"));
            Assert.That(brakePressure.Text, Is.EqualTo("Brake Pressure: 100"));
        });
    }
}