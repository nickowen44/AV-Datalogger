using System;

namespace Dashboard.Connectors.Serial;

public interface ISerialPort
{
    bool IsConnected { get; }

    /// <summary>
    ///     Called when data is received from the serial port.
    /// </summary>
    event EventHandler<SerialPortData> DataReceived;

    /// <summary>
    ///     Opens the serial port.
    /// </summary>
    void Open();

    /// <summary>
    ///     Close the serial port.
    /// </summary>
    void Close();

    /// <summary>
    ///     Configure the serial port.
    ///     This method will close the port if it's already open, configure it, and then reopen it if it was previously open.
    /// </summary>
    /// <param name="portName">The name of the port to connect to.</param>
    /// <param name="baudRate">The baudRate to listen at.</param>
    void Configure(string portName, int baudRate);

    bool Write(string data);
}