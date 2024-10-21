using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Dashboard.Connectors.Serial;

public class SerialPortWrapper(ILogger<SerialPortWrapper> logger) : ISerialPort
{
    private const double ConnectionTimeout = 5.0;

    private readonly SerialPort _serialPort = new();
    private DateTime _lastMessageReceived = DateTime.Now;

    private bool _shouldRun = true;
    public event EventHandler<SerialPortData>? DataReceived;

    public void Open()
    {
        logger.LogInformation("Opening serial port");

        // Don't open the port if it's already open
        if (_serialPort.IsOpen)
        {
            logger.LogWarning("Serial port is already open, skipping opening");
            return;
        }

        _serialPort.Open();

        // Initialise a read thread so our main (UI) thread doesn't block
        var thread = new Thread(() =>
        {
            while (_shouldRun)
                try
                {
                    var data = ReadLine();
                    DataReceived?.Invoke(this, new SerialPortData
                    {
                        Buffer = data
                    });
                }
                catch (OperationCanceledException)
                {
                    // This exception is thrown when the thread is cancelled, this happens when the application is closed
                    // and the thread is stopped, we can safely ignore this exception.
                    logger.LogDebug("Serial port read thread cancelled");
                }
        });

        _shouldRun = true;
        thread.Start();
        logger.LogInformation("Serial port read thread started");
    }

    public void Close()
    {
        logger.LogInformation("Closing serial port");

        _shouldRun = false;

        if (_serialPort.IsOpen)
            _serialPort.Close();
    }

    public void Configure(string portName, int baudRate)
    {
        logger.LogInformation(
            "Configuring serial port to connect on port name: {portName} with a baud rate of {baudRate}", portName,
            baudRate);

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

            logger.LogInformation("Writing data to serial port: {data}", data);
            try
            {
                _serialPort.Write(data);

                // If the AV Logger doesn't respond OK then connection is dead.
                // TODO: This will not work for the actual logger as there could be multiple messages in the buffer.
                if (_serialPort.ReadLine() != "OK") return false;

                logger.LogInformation("Heartbeat acknowledged by AV Logger");
                return true;
            }
            catch (TimeoutException)
            {
                logger.LogWarning("Write to serial port timed out");
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