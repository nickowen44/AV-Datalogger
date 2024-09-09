using System;
using System.IO.Ports;

namespace Dashboard.Connectors.Serial;

public class SerialPortWrapper : ISerialPort
{
    private readonly SerialPort _serialPort = new();

    public event EventHandler<SerialPortData>? DataReceived;

    private void SerialPortDataReceived(object _, SerialDataReceivedEventArgs __)
    {
        DataReceived?.Invoke(this, new SerialPortData
        {
            Buffer = _serialPort.ReadExisting()
        });
    }

    public void Open()
    {
        _serialPort.Open();

        _serialPort.DataReceived += SerialPortDataReceived;
    }

    public void Close()
    {
        _serialPort.Close();

        _serialPort.DataReceived -= SerialPortDataReceived;
    }

    public void Configure(string portName, int baudRate)
    {
        var wasOpen = _serialPort.IsOpen;

        // Close the port if it's already open
        if (wasOpen)
            _serialPort.Close();
        
        _serialPort.PortName = portName;
        _serialPort.BaudRate = baudRate;

        // Reopen the port if it was previously open
        if (wasOpen)
            _serialPort.Open();
    }

    public string ReadExisting()
    {
        _lastMessageReceived = DateTime.Now;
        return _serialPort.ReadExisting();
    }

    private DateTime _lastMessageReceived = DateTime.Now;

    private const double ConnectionTimeout = 5.0;

    public bool IsConnected
    {
        get
        {
            var timeSinceLastMessage = DateTime.Now - _lastMessageReceived;
            return timeSinceLastMessage.TotalSeconds < ConnectionTimeout && _serialPort.IsOpen;
        }
    }
}