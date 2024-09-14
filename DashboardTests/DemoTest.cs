using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class DemoTest
{
    private Mock<IDataStore> _dataStore;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }

    [AvaloniaTest]
    public void TestMainWindow()
    {
        // Arrange
        var window = new MainWindowView()
        {
            DataContext = new MainWindowViewModel()
        };

        // Act
        window.Show();

        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<MainWindowView>());

        var naviBar = window.FindControl<ListBox>("NaviBar");
        var mainContent = window.FindControl<ContentControl>("MainContent");
        var selected = new ListItemTemplate(typeof(ConnectionView), null, "Connection");

        Assert.Multiple(() =>
        {
            Assert.That(naviBar, Is.Not.Null);
            Assert.That(mainContent, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(naviBar.SelectedItem, Is.EqualTo(selected));
            Assert.That(mainContent.Content, Is.InstanceOf(selected.View));
        });

        window.Close();
    }

    [AvaloniaTest]
    public void TestDataStore()
    {
        // Arrange
        _dataStore.SetupGet(x => x.Speed).Returns(50);
        _dataStore.SetupGet(x => x.SteeringAngle).Returns(90);
        _dataStore.SetupGet(x => x.BrakePressure).Returns(100);

        var window = new DataView()
        {
            DataContext = new DataViewModel(_dataStore.Object)
        };


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

    [AvaloniaTest]

    public void TestNavigation()
    {

        // Arrange
        var window = new MainWindowView()
        {
            DataContext = new MainWindowViewModel()
        };

        // Act
        window.Show();

        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<MainWindowView>());

        var naviBar = window.FindControl<ListBox>("NaviBar");
        var mainContent = window.FindControl<ContentControl>("MainContent");
        var defaultSelectedItem = new ListItemTemplate(typeof(ConnectionView), null, "Connection");

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