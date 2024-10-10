using System.Reflection;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.Connectors.Serial;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using DashboardTests.Utils;
using Moq;

namespace DashboardTests;

public class ConnectionTest
{
    private Mock<IDataStore> _dataStore;
    
    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }

    [AvaloniaTest]
    public void TestConnectionPageStartup()
    {
        // Arrange  
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var connectionType = window.FindControl<ComboBox>("ConnectionTypeCombo");
        var serialPortSection = window.FindControl<StackPanel>("SerialPortSection");
        var tcpPortSection = window.FindControl<StackPanel>("TCPSection");
        var filePortSection = window.FindControl<StackPanel>("FileSection");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(connectionType, Is.Not.Null);
            Assert.That(serialPortSection, Is.Not.Null);
            Assert.That(tcpPortSection, Is.Not.Null);
            Assert.That(filePortSection, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(connectionType.SelectedItem, Is.EqualTo("Serial Port"));
            Assert.That(serialPortSection.IsVisible, Is.EqualTo(true));
            Assert.That(tcpPortSection.IsVisible, Is.EqualTo(false));
            Assert.That(filePortSection.IsVisible, Is.EqualTo(false));
        });
    }

    [AvaloniaTest]
    public void TestConnectionPageChangeConnectionType()
    {
        // Arrange
        var window = new Window()
        {
            Height = 450,
            Width = 800,
            Content = new ConnectionView()
            {

                DataContext = new ConnectionViewModel(_dataStore.Object)
            }
        };

        window.Show();

        var connectionType = window.GetVisualDescendants()
            .OfType<ComboBox>()
            .FirstOrDefault(tb => tb.Name == "ConnectionTypeCombo");
        var serialPortSection = window.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(tb => tb.Name == "SerialPortSection");
        var tcpPortSection = window.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(tb => tb.Name == "TCPSection");
        var filePortSection = window.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(tb => tb.Name == "FileSection");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(connectionType, Is.Not.Null);
            Assert.That(serialPortSection, Is.Not.Null);
            Assert.That(tcpPortSection, Is.Not.Null);
            Assert.That(filePortSection, Is.Not.Null);
        });

        // Change Connection Type to IP Address
        connectionType.SelectedItem = "IP Address";

        Assert.Multiple(() =>
        {
            Assert.That(connectionType.SelectedItem, Is.EqualTo("IP Address"));
            Assert.That(serialPortSection.IsVisible, Is.EqualTo(false));
            Assert.That(tcpPortSection.IsVisible, Is.EqualTo(true));
            Assert.That(filePortSection.IsVisible, Is.EqualTo(false));
        });

        window.Close();
    }
    
    [AvaloniaTest]
    public void TestConnectionViewInitialState()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        // Act
        var connectionTypeComboBox = window.FindControl<ComboBox>("ConnectionTypeCombo");
        var serialPortSection = window.FindControl<StackPanel>("SerialPortSection");
        var tcpSection = window.FindControl<StackPanel>("TCPSection");
        var fileSection = window.FindControl<StackPanel>("FileSection");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(connectionTypeComboBox, Is.Not.Null);
            Assert.That(serialPortSection.IsVisible, Is.EqualTo(true));
            Assert.That(tcpSection.IsVisible, Is.EqualTo(false));
            Assert.That(fileSection.IsVisible, Is.EqualTo(false));
        });
    }
    
    
    [AvaloniaTest]
    public void TestConnectCommandWithSerialPort()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var viewModel = (ConnectionViewModel)window.DataContext;
        var connectButton = window.FindControl<Button>("ConnectButton");

        // Act
        viewModel.SelectedSerialPort = "COM21";
        connectButton.Command.Execute(null);

        // Assert
        _dataStore.Verify(ds => ds.Connect(It.Is<SerialConnectorArgs>(
            args => args.PortName == "COM21"
        )));
    }
    
    [AvaloniaTest]
    public void TestConnectCommandExecution()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var connectButton = window.FindControl<Button>("ConnectButton");
        var disconnectButton = window.FindControl<Button>("DisconnectButton");

        // Act - Simulate connection
        connectButton.Command?.Execute(null);

        // Assert - After connection
        Assert.Multiple(() =>
        {
            Assert.That(connectButton.IsEnabled, Is.EqualTo(true));
            Assert.That(disconnectButton.IsEnabled, Is.EqualTo(false));
        });
    }
    
    [AvaloniaTest]
    public void TestDisconnectCommandExecution()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var connectButton = window.FindControl<Button>("ConnectButton");
        var disconnectButton = window.FindControl<Button>("DisconnectButton");

        // Act - Simulate connection
        disconnectButton.Command?.Execute(null);

        // Assert - After connection
        Assert.Multiple(() =>
        {
            Assert.That(connectButton.IsEnabled, Is.EqualTo(true));
            Assert.That(disconnectButton.IsEnabled, Is.EqualTo(false));
        });
    }
    
    
    [AvaloniaTest]
    public void TestDisconnectCommand()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var disconnectButton = window.FindControl<Button>("DisconnectButton");

        // Act
        disconnectButton.Command.Execute(null);

        // Assert
        _dataStore.Verify(ds => ds.Disconnect(), Times.Once);
    }
    
    
    [AvaloniaTest]
    public void TestRefreshSerialPortsCommand()
    {
        // Arrange
        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel(_dataStore.Object)
        };

        var serialPortCombo = window.FindControl<ComboBox>("SerialPortCombo");
        var refreshButton = window.FindControl<Button>("RefreshPortsButton");

        // Act - Simulate refreshing serial ports
        refreshButton.Command?.Execute(null);

        // Assert - Serial ports should be refreshed
        Assert.That(serialPortCombo.Items.Count, Is.GreaterThan(0));
    }
    
}