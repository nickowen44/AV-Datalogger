using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DashboardTests;

public class FooterTest
{
    private Mock<IDataStore> _dataStore;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }

    [AvaloniaTest]
    public void TestFooterStartup()
    {
        // Arrange
        var window = new FooterView
        {
            DataContext = new FooterViewModel(_dataStore.Object, NullLogger<FooterViewModel>.Instance)
        };


        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<FooterView>());

        var carID = window.FindControl<TextBlock>("CarId");
        var utcTime = window.FindControl<TextBlock>("UTCTime");
        var localTime = window.FindControl<TextBlock>("LocalTime");
        var console = window.FindControl<TextBox>("ConsoleTextBox");

        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(localTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });
    }

    [AvaloniaTest]
    public void TestFooterCarIdCurTimeConsoleUpdate()
    {
        // Arrange
        const string consoleText =
            "ID=A46|UTC=P2024820T06:56:04.00|SA=###|ST=###|STA=###|STT=###|BRA=###|BRT=###|MMT=###|MMA=###|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###";
        _dataStore.SetupGet(x => x.RawData).Returns(new RawData
        {
            CarId = "A46",
            UTCTime = DateTime.Parse("2024-08-20 06:56:04")
        });

        var window = new Window
        {
            Content = new FooterView
            {
                DataContext = new FooterViewModel(_dataStore.Object, NullLogger<FooterViewModel>.Instance)
            }
        };

        // Act
        window.Show();

        _dataStore.Raise(x => x.RawDataUpdated += null, EventArgs.Empty);
        _dataStore.Raise(x => x.ConsoleMessageUpdated += null, this, consoleText);

        // Assert
        var carID = window.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "CarId");
        var utcTime = window.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "UTCTime");
        var localTime = window.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "LocalTime");
        var console = window.GetVisualDescendants()
            .OfType<TextBox>()
            .FirstOrDefault(tb => tb.Name == "ConsoleTextBox");

        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(localTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(carID.Text, Is.EqualTo("Car ID: A46"));
            Assert.That(utcTime.Text, Is.EqualTo("UTC Time: 2024-08-20 06:56:04"));
            Assert.That(localTime.Text,
                Is.EqualTo(DateTime.Parse(localTime.Text?.Substring(11) ?? "")
                    .ToString("'Local Time': yyyy-MM-dd HH:mm:ss")));
            Assert.That(console.Text, Is.EqualTo($"{consoleText}\n"));
        });
    }

    [AvaloniaTest]
    public void TestFooterConnectionHeatBeatUpdate()
    {
        // Arrange
        _dataStore.SetupGet(x => x.RawData).Returns(new RawData
        {
            CarId = "A46",
            UTCTime = DateTime.Parse("2024-08-20 06:56:04"),
            ConnectionStatus = false
        });

        var window = new Window
        {
            Content = new FooterView
            {
                DataContext = new FooterViewModel(_dataStore.Object, NullLogger<FooterViewModel>.Instance)
            }
        };

        // Act
        window.Show();

        _dataStore.Raise(x => x.RawDataUpdated += null, EventArgs.Empty);

        // Assert
        var connection = window.GetVisualDescendants()
            .OfType<Ellipse>()
            .FirstOrDefault(tb => tb.Name == "ConnectionIndicator");
        var heartBeat = window.GetVisualDescendants()
            .OfType<Ellipse>()
            .FirstOrDefault(tb => tb.Name == "HeartBeat");

        Assert.Multiple(() =>
        {
            Assert.That(connection, Is.Not.Null);
            Assert.That(heartBeat, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(connection.Fill, Is.EqualTo(Brushes.Red));
            Assert.That(heartBeat.Fill, Is.EqualTo(Brushes.Orange));
        });
    }
}