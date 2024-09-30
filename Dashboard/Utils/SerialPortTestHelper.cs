namespace Dashboard.Utils;

public interface ISerialPortService
{
    string[] GetPortNames();
}

public class SerialPortTestHelper : ISerialPortService
{
    public string[] GetPortNames()
    {
        return System.IO.Ports.SerialPort.GetPortNames();
    }
}