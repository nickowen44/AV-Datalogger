using Dashboard.Connectors.Csv;
using Dashboard.Serialisation.Csv;
using DashboardTests.Utils;
using Microsoft.Extensions.Logging;

namespace DashboardTests.Connectors;

[TestFixture]
public class CsvConnectorTests
{
    [SetUp]
    public void Setup()
    {
        _logger = new ShimLogger<CsvConnector>();
        _csvConnector = new CsvConnector(_logger);

        // initialise the args with a unique file path
        _args = new CsvConnectorArgs { FilePath = $"test_{Guid.NewGuid()}.csv" };
    }

    private CsvConnector _csvConnector;
    private ShimLogger<CsvConnector> _logger;
    private CsvConnectorArgs _args;

    private void Cleanup()
    {
        for (var i = 0; i < 5; i++)
            try
            {
                File.Delete(_args.FilePath);
            }
            catch
            {
                Thread.Sleep(250);
            }
    }

    [Test]
    public void GpsDataUpdatedEvent()
    {
        // Arrange
        var gpsDataReceived = false;
        _csvConnector.GpsDataUpdated += (_, _) => gpsDataReceived = true;

        const string input =
            "A46,P2024820T06:56:04.00,True,-37.738357,144.935502,129.9,0.13,0.62,2.24,244,2.53,253,244,6,369,244,75.3644179635783,98.58397437561007,40.16896847870078,4.497529070623774,46.48539889053502,40.47376992382284,94.64806923974632,72.62086646406469,6.874100022541219,95.48849618187013,79.76717489225656,4,1,4,False,False,1,1,2,False,False,False,252,203445";

        File.WriteAllText(_args.FilePath, $"{CsvDataSerialiser.Header}\n{input}");

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(250);
        _csvConnector.Stop();

        // Assert
        Assert.That(gpsDataReceived, Is.True);

        Cleanup();
    }

    [Test]
    public void AvDataUpdatedEvent()
    {
        // Arrange
        var avDataReceived = false;
        _csvConnector.AvDataUpdated += (_, _) => avDataReceived = true;

        const string input =
            "A46,P2024820T06:56:04.00,True,-37.738357,144.935502,129.9,0.13,0.62,2.24,244,2.53,253,244,6,369,244,75.3644179635783,98.58397437561007,40.16896847870078,4.497529070623774,46.48539889053502,40.47376992382284,94.64806923974632,72.62086646406469,6.874100022541219,95.48849618187013,79.76717489225656,4,1,4,False,False,1,1,2,False,False,False,252,203445";

        File.WriteAllText(_args.FilePath, $"{CsvDataSerialiser.Header}\n{input}");

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(250);
        _csvConnector.Stop();

        // Assert
        Assert.That(avDataReceived, Is.True);

        Cleanup();
    }

    [Test]
    public void ResDataUpdatedEvent()
    {
        // Arrange
        var resDataReceived = false;
        _csvConnector.ResDataUpdated += (_, _) => resDataReceived = true;

        const string input =
            "A46,P2024820T06:56:04.00,True,-37.738357,144.935502,129.9,0.13,0.62,2.24,244,2.53,253,244,6,369,244,75.3644179635783,98.58397437561007,40.16896847870078,4.497529070623774,46.48539889053502,40.47376992382284,94.64806923974632,72.62086646406469,6.874100022541219,95.48849618187013,79.76717489225656,4,1,4,False,False,1,1,2,False,False,False,252,203445";

        File.WriteAllText(_args.FilePath, $"{CsvDataSerialiser.Header}\n{input}");

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(250);
        _csvConnector.Stop();

        // Assert
        Assert.That(resDataReceived, Is.True);

        Cleanup();
    }

    [Test]
    public void InvalidFileHeader()
    {
        // Arrange
        const string input = "INVALID_HEADER";

        File.WriteAllText(_args.FilePath, input);

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(250);
        _csvConnector.Stop();

        // Assert
        _logger.AssertLog(LogLevel.Error, "Invalid file header");

        Cleanup();
    }

    [Test]
    public void InvalidDataFormat()
    {
        // Arrange
        const string input = "A46,P2024820T06:56:04.00,True";

        File.WriteAllText(_args.FilePath, $"{CsvDataSerialiser.Header}\n{input}");

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(250);
        _csvConnector.Stop();

        // Assert
        _logger.AssertLog(LogLevel.Error, "Invalid data format");

        Cleanup();
    }

    [Test]
    public void LoopBackToStartOfFile()
    {
        // Arrange
        var gpsDataReceivedCount = 0;
        _csvConnector.GpsDataUpdated += (_, _) => gpsDataReceivedCount++;

        const string input =
            "A46,P2024820T06:56:04.00,True,-37.738357,144.935502,129.9,0.13,0.62,2.24,244,2.53,253,244,6,369,244,75.3644179635783,98.58397437561007,40.16896847870078,4.497529070623774,46.48539889053502,40.47376992382284,94.64806923974632,72.62086646406469,6.874100022541219,95.48849618187013,79.76717489225656,4,1,4,False,False,1,1,2,False,False,False,252,203445";

        File.WriteAllText(_args.FilePath, $"{CsvDataSerialiser.Header}\n{input}");

        // Act
        _csvConnector.Start(_args);
        Thread.Sleep(2000);
        _csvConnector.Stop();

        // Assert
        Assert.That(gpsDataReceivedCount, Is.EqualTo(2));

        Cleanup();
    }
}