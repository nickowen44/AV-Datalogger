using System;
using Dashboard.Models;
using Microsoft.Extensions.Logging;

namespace Dashboard.Connectors.Csv;

public class CsvConnector(ILogger<CsvConnector> logger) : IConnector
{
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;
    public event EventHandler<RawData>? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;

    public void Start(IConnectorArgs args)
    {
        if (args is not CsvConnectorArgs csvArgs)
        {
            logger.LogError("Invalid arguments passed to Csv Connector");
        }
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}