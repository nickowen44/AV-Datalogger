using System;
using System.Threading;

namespace Dashboard.Connectors;

public class DummyConnector : IConnector
{
    public event EventHandler<DataUpdatedEventArgs>? DataUpdated;

    private bool _shouldStop;

    public void Start()
    {
        // Dummy implementation, lets pretend we're getting data from a car, triggering the DataUpdated event every second
        var random = new Random();

        // Start a new thread to simulate the data coming in
        new Thread(() =>
        {
            while (!_shouldStop)
            {
                DataUpdated?.Invoke(this,
                    new DataUpdatedEventArgs(random.NextDouble() * 10, random.NextDouble() * 10,
                        random.NextDouble() * 10));
                Thread.Sleep(1000);
            }
        }).Start();
    }

    public void Stop()
    {
        _shouldStop = true;
    }
}
