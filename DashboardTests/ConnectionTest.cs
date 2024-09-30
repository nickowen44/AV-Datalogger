using System.IO.Ports;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DashboardTests;

public class ConnectionTest
{
    private Mock<IDataStore> _dataStore;
    private Mock<SerialPort> _serialPorts;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
        _serialPorts = new Mock<SerialPort>();

    }

    [AvaloniaTest]
    public void TestConnectionPage()
    {

        var window = new ConnectionView()
        {
            DataContext = new ConnectionViewModel()
        };

        // Assert
        var connectionType = window.FindControl<ComboBox>("ConnectionTypeCombo");
        var serialPortSection = window.FindControl<StackPanel>("SerialPortSection");
        var tcpPortSection = window.FindControl<StackPanel>("TCPSection");
        var filePortSection = window.FindControl<StackPanel>("FileSection");

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
        });
    }
}