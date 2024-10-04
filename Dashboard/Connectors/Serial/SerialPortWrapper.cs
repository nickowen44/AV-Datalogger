using System;
using System.IO.Ports;
using System.Threading;

namespace Dashboard.Connectors.Serial;

public class SerialPortWrapper : ISerialPort
{
    public event EventHandler<SerialPortData>? DataReceived;

    private readonly SerialPort _serialPort = new();

    private bool _shouldRun = true;
    private const double ConnectionTimeout = 5.0;
    private DateTime _lastMessageReceived = DateTime.Now;

    public void Open()
    {
        // Don't open the port if it's already open
        if (_serialPort.IsOpen)
            return;

        _serialPort.Open();
        // Initialise a read thread so our main (UI) thread doesn't block
        var thread = new Thread(() =>
        {
            // Exception handling so we don't have unhandled exceptions on thread cancellation
            try
            {
                while (_shouldRun)
                {
                    var data = ReadLine();
                    DataReceived?.Invoke(this, new SerialPortData
                    {
                        Buffer = data
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        });
        _shouldRun = true;
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

    private string ReadLine()
    {
        _lastMessageReceived = DateTime.Now;
        return _serialPort.ReadLine();
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
                if (_serialPort.ReadLine() != "OK")
                {
                    return false;
                }
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
}