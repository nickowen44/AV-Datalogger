using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DashboardTests;

[TestFixture]
public class DemoTest
{
    private Mock<IDataStore> _dataStore;
    private Mock<IServiceProvider> _serviceProvider;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
        _serviceProvider = new Mock<IServiceProvider>();
        var footerViewModel = new FooterViewModel(_dataStore.Object, NullLogger<FooterViewModel>.Instance);
        _serviceProvider.Setup(sp => sp.GetService(typeof(FooterViewModel)))
            .Returns(footerViewModel);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IDataStore)))
            .Returns(_dataStore.Object);
    }



    [AvaloniaTest]
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

        var window = new DataView()
        {
            DataContext = new DataViewModel(_dataStore.Object, NullLogger<DataViewModel>.Instance)
        };

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
    public void TestNullAvData()
    {
        // Arrange
        var window = new DataView
        {
            DataContext = new DataViewModel(_dataStore.Object, NullLogger<DataViewModel>.Instance)
        };

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

    [AvaloniaTest]
    public void TestNavigation()
    {
        // Arrange
        var window = new MainWindowView()
        {
            DataContext = new MainWindowViewModel(_serviceProvider.Object)
        };

        // Act
        window.Show();

        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<MainWindowView>());

        var naviBar = window.FindControl<ListBox>("NaviBar");
        var mainContent = window.FindControl<ContentControl>("MainContent");
        var defaultSelectedItem = new ListItemTemplate(typeof(ConnectionView), typeof(ConnectionViewModel), "Connection");

        Assert.Multiple(() =>
        {
            Assert.That(naviBar, Is.Not.Null);
            Assert.That(mainContent, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(naviBar.SelectedItem, Is.EqualTo(defaultSelectedItem));
            Assert.That(mainContent.Content, Is.InstanceOf(defaultSelectedItem.View));
        });

        var changedSelectedItem = new ListItemTemplate(typeof(SetupView), null, "Setup");

        // Click on Setup button.
        window.MouseDown(new Point(135, 70), MouseButton.Left);
        window.MouseUp(new Point(135, 70), MouseButton.Left);
        Assert.Multiple(() =>
        {
            Assert.That(naviBar.SelectedItem, Is.EqualTo(changedSelectedItem));
            Assert.That(mainContent.Content, Is.InstanceOf(changedSelectedItem.View));
        });

        window.Close();
    }
}