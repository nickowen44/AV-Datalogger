﻿using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
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
}