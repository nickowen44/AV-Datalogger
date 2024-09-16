using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dashboard.Models;
using Dashboard.Utils;
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
    public void TestDataStore()
    {
        // Arrange
        _dataStore.SetupGet(x => x.AvStatusData).Returns(new AvData
        {
            Speed = new ValuePair<double>
            {
                Actual = 50, Target = 50
            },
            SteeringAngle = new ValuePair<double>
            {
                Actual = 90, Target = 90
            },
            BrakeActuation = new ValuePair<double>
            {
                Actual = 100, Target = 100
            }
        });

        var window = new MainWindow
        {
            DataContext = new MainViewModel(_dataStore.Object)
        };

        // Act
        window.Show();

        // Assert
        var speed = window.FindControl<TextBlock>("SpeedDisplay");
        var steeringAngle = window.FindControl<TextBlock>("SteeringAngleDisplay");
        var brakePressure = window.FindControl<TextBlock>("BrakeActuationDisplay");

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
            Assert.That(brakePressure.Text, Is.EqualTo("Brake Actuation: 100"));
        });
    }

    [AvaloniaTest]
    [Ignore("Waiting on ADL-32 PR as currently the main window is scurtineering view")]
    public void TestNullAvData()
    {
        // Arrange
        var window = new MainWindow
        {
            DataContext = new MainViewModel(_dataStore.Object)
        };

        // Act
        window.Show();

        // Assert
        var speed = window.FindControl<TextBlock>("SpeedDisplay");
        var steeringAngle = window.FindControl<TextBlock>("SteeringAngleDisplay");
        var brakePressure = window.FindControl<TextBlock>("BrakeActuationDisplay");

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
            Assert.That(brakePressure.Text, Is.EqualTo("Brake Actuation: 0"));
        });
    }
}