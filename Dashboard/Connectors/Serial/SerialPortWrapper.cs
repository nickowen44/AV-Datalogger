using System;
using System.IO.Ports;
using System.Threading;

namespace Dashboard.Connectors.Serial;

public class SerialPortWrapper : ISerialPort
{
    private const double ConnectionTimeout = 5.0;

    private readonly SerialPort _serialPort = new();
    private DateTime _lastMessageReceived = DateTime.Now;

    private bool _shouldRun = true;
    public event EventHandler<SerialPortData>? DataReceived;

    public void Open()
    {
        // Don't open the port if it's already open
        if (_serialPort.IsOpen)
            return;

        _serialPort.Open();

        // Initialise a read thread so our main (UI) thread doesn't block
        var thread = new Thread(() =>
        {
            while (_shouldRun)
            {
                var data = ReadLine();
                DataReceived?.Invoke(this, new SerialPortData
                {
                    Buffer = data
                });
            }
        });

        thread.Start();
    }

    public void Close()
    {
        _shouldRun = false;

        if (_serialPort.IsOpen)
            _serialPort.Close();
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

    public bool IsConnected
    {
        get
        {
            var timeSinceLastMessage = DateTime.Now - _lastMessageReceived;
            return timeSinceLastMessage.TotalSeconds < ConnectionTimeout && _serialPort.IsOpen;
        }
    }

    public bool Write(string? data)
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.WriteTimeout = 50;
            try
            {
                _serialPort.Write(data);

                // If the AV Logger doesn't respond OK then connection is dead.
                if (_serialPort.ReadLine() != "OK") return false;
                Console.WriteLine("Wrote Heatbeat to SerialPort");
                return true;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Error writing to serial port.");
                return false;
            }
        }

        return false;
    }

    private string ReadLine()
    {
        _lastMessageReceived = DateTime.Now;
        return _serialPort.ReadLine();
    }
}