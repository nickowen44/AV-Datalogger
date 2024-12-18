﻿using Dashboard.Connectors.Serial;
using DashboardTests.Utils;
using Microsoft.Extensions.Logging;
using Moq;

namespace DashboardTests.Connectors;

[TestFixture]
public class SerialConnectorTests
{
    [SetUp]
    public void Setup()
    {
        _serialPortMock = new Mock<ISerialPort>();
        _logger = new ShimLogger<SerialConnector>();
        _serialConnector = new SerialConnector(_serialPortMock.Object, _logger);
    }

    private SerialConnector _serialConnector;
    private Mock<ISerialPort> _serialPortMock;
    private ShimLogger<SerialConnector> _logger;
    private readonly SerialConnectorArgs _args = new() { PortName = "COM1" };

    private void RaiseDataReceivedEvent(string input)
    {
        _serialPortMock.Raise(s => s.DataReceived += null, this, new SerialPortData { Buffer = input });
    }

    [Test]
    public void TestGpsDataUpdatedEvent()
    {
        // Arrange
        var gpsDataReceived = false;
        _serialConnector.GpsDataUpdated += (_, _) => gpsDataReceived = true;

        const string input =
            "ID=A46|UTC=P20240820T06:56:04.00|LAT=-37.738357|LNG=144.935502|HFA=244|HDOP=2.53|HVAL=253|SFA=244|NOS=6|SPFA=369|MPS=0.62|KMH=2.24|AFA=244|ALM=129.90|AKM=0.13\r\n";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input);
        _serialConnector.Stop();

        // Assert
        Assert.That(gpsDataReceived, Is.True);
    }

    [Test]
    public void TestAvDataUpdatedEvent()
    {
        // Arrange
        var avDataReceived = false;
        _serialConnector.AvDataUpdated += (_, _) => avDataReceived = true;

        const string input =
            "ID=A46|UTC=P20240820T06:56:04.00|SA=###|ST=###|STA=180|STT=128|BRA=###|BRT=###|MMT=125|MMA=70|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###\r\n";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input);
        _serialConnector.Stop();

        // Assert
        Assert.That(avDataReceived, Is.True);
    }

    [Test]
    public void TestResDataUpdatedEvent()
    {
        // Arrange
        var resDataReceived = false;
        _serialConnector.ResDataUpdated += (_, _) => resDataReceived = true;

        const string input = "ID=1|UTC=123456|RES=1|K2T=0|K3B=1|RRQ=75\r\n";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input);
        _serialConnector.Stop();

        // Assert
        Assert.That(resDataReceived, Is.True);
    }

    [Test]
    public void TestMultipleLinesInBuffer()
    {
        // Arrange
        var gpsDataReceived = false;
        var resDataReceived = false;
        var avDataReceived = false;
        _serialConnector.GpsDataUpdated += (_, _) => gpsDataReceived = true;
        _serialConnector.ResDataUpdated += (_, _) => resDataReceived = true;
        _serialConnector.AvDataUpdated += (_, _) => avDataReceived = true;

        // First input is Res data, second is AV data
        const string input1 = "ID=A46|UTC=P20240820T06:56:04.00|RES=0|K2T=0|K3B=0|RRQ=255\r\n";
        const string input2 =
            "ID=A46|UTC=P20240820T06:56:04.00|SA=###|ST=###|STA=180|STT=128|BRA=###|BRT=###|MMT=180|MMA=243|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###\r\n";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input1);
        RaiseDataReceivedEvent(input2);
        _serialConnector.Stop();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(gpsDataReceived, Is.False);
            Assert.That(resDataReceived, Is.True);
            Assert.That(avDataReceived, Is.True);
        });
    }

    [Test]
    public void TestInvalidKeyValuePair()
    {
        // Arrange
        const string input = "ID=A46|UTC=P20240820T06:56:04.00|BADINPUT\r\n";
        const string expectedExceptionMessage =
            "Invalid key-value pair: BADINPUT, Skipping value: ID=A46|UTC=P20240820T06:56:04.00|BADINPUT";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input);
        _serialConnector.Stop();

        // Assert
        _logger.AssertLog(LogLevel.Error, expectedExceptionMessage);
    }

    [Test]
    public void TestInvalidKey()
    {
        // Arrange
        const string input = "ID=A46|UTC=P20240820T06:56:04.00|BADKEY=123\r\n";
        const string expectedExceptionMessage =
            "Unknown message type received: ID=A46|UTC=P20240820T06:56:04.00|BADKEY=123";

        // Act
        _serialConnector.Start(_args);
        RaiseDataReceivedEvent(input);
        _serialConnector.Stop();

        // Assert
        _logger.AssertLog(LogLevel.Error, expectedExceptionMessage);
    }
}