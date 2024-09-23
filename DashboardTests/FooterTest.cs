using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

public class FooterTest
{
    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
        _serviceProvider = new Mock<IServiceProvider>();
    }

    private Mock<IDataStore> _dataStore;
    private Mock<IServiceProvider> _serviceProvider;

    [AvaloniaTest]
    public void TestFooterStartup()
    {

        // Arrange
        var window = new FooterView()
        {
            DataContext = new FooterViewModel(_dataStore.Object)
        };


        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<FooterView>());

        var carID = window.FindControl<TextBlock>("CarId");
        var utcTime = window.FindControl<TextBlock>("CurTime");
        var console = window.FindControl<TextBox>("ConsoleTextBox");

        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });

    }

    [AvaloniaTest]
    public void TestFooterCarIDCurTimeConsoleUpdate()
    {

        // Arrange
        _dataStore.SetupGet(x => x.RawData).Returns(new RawData()
        {
            CarId = "A46",
            UTCTime = "P2024820T06:56:04.00",
            RawMessage = "ID=A46|UTC=P2024820T06:56:04.00|SA=###|ST=###|STA=###|STT=###|BRA=###|BRT=###|MMT=###|MMA=###|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###"
        });

        var window = new Window
        {
            Content = new FooterView
            {
                DataContext = new FooterViewModel(_dataStore.Object)
            }
        };

        window.Show();

        Dispatcher.UIThread.Post(() =>
        {
            _dataStore.Raise(x => x.RawDataUpdated += null, EventArgs.Empty);
        });

        Dispatcher.UIThread.RunJobs();

        // Assert
        var carID = window.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "CarId");
        var utcTime = window.GetVisualDescendants()
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "CurTime");
        var console = window.GetVisualDescendants()
            .OfType<TextBox>()
            .FirstOrDefault(tb => tb.Name == "ConsoleTextBox");

        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(carID.Text, Is.EqualTo("Car ID: A46"));
            Assert.That(utcTime.Text, Is.EqualTo("UTC Time: 2024-08-19 20:56:04"));
            Assert.That(console.Text, Is.EqualTo("ID=A46|UTC=P2024820T06:56:04.00|SA=###|ST=###|STA=###|STT=###|BRA=###|BRT=###|MMT=###|MMA=###|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###\n"));
        });
    }

    [AvaloniaTest]
    public void TestFooterConnectionHeatBeatUpdate()
    {

        // Arrange
        _dataStore.SetupGet(x => x.RawData).Returns(new RawData()
        {
            ConnectionStatus = false,
        });

        var window = new Window
        {
            Content = new FooterView
            {
                DataContext = new FooterViewModel(_dataStore.Object)
            }
        };

        window.Show();

        Dispatcher.UIThread.Post(() =>
        {
            _dataStore.Raise(x => x.RawDataUpdated += null, EventArgs.Empty);
        });

        Dispatcher.UIThread.RunJobs();

        // // Assert
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

        window.Close();
    }

}