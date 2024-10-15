using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.Logging;
using Moq;

namespace DashboardTests;

[TestFixture]
public class DataViewModelTests
{
    [SetUp]
    public void Setup()
    {
        _dataStoreMock = new Mock<IDataStore>();
        var loggerMock = new Mock<ILogger<DataViewModel>>();

        // Initialize DataViewModel with mocked data store and logger
        _viewModel = new DataViewModel(_dataStoreMock.Object, loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _viewModel?.Dispose();
    }

    private Mock<IDataStore> _dataStoreMock;
    private DataViewModel _viewModel;

    [Test]
    public void Speed_ShouldReturnCorrectValue_WhenAvDataChanges()
    {
        // Arrange: Set up AvStatusData with Speed.Actual = 100
        var avStatusData = new AvData { Speed = new ValuePair<double> { Actual = 100, Target = 120 } };
        _dataStoreMock.SetupGet(ds => ds.AvStatusData).Returns(avStatusData);

        // Act: Trigger the AV data update
        _dataStoreMock.Raise(ds => ds.AvDataUpdated += null, EventArgs.Empty);
        var speed = _viewModel.Speed;

        // Assert: Speed should reflect the updated actual value from AvStatusData
        Assert.That(speed, Is.EqualTo(avStatusData.Speed.Actual));
    }

    [Test]
    public void BrakeActuation_ShouldReturnCorrectValue_WhenAvDataChanges()
    {
        // Arrange: Set up AvStatusData with BrakeActuation.Actual = 45
        var avStatusData = new AvData { BrakeActuation = new ValuePair<double> { Actual = 45, Target = 50 } };
        _dataStoreMock.SetupGet(ds => ds.AvStatusData).Returns(avStatusData);

        // Act: Trigger the AV data update
        _dataStoreMock.Raise(ds => ds.AvDataUpdated += null, EventArgs.Empty);
        var brakeActuation = _viewModel.BrakeActuation;

        // Assert: BrakeActuation should reflect the updated actual value from AvStatusData
        Assert.That(brakeActuation, Is.EqualTo(avStatusData.BrakeActuation.Actual));
    }

    [Test]
    public void SteeringAngle_ShouldReturnCorrectValue_WhenAvDataChanges()
    {
        // Arrange: Set up AvStatusData with SteeringAngle.Actual = 30
        var avStatusData = new AvData { SteeringAngle = new ValuePair<double> { Actual = 30, Target = 35 } };
        _dataStoreMock.SetupGet(ds => ds.AvStatusData).Returns(avStatusData);

        // Act: Trigger the AV data update
        _dataStoreMock.Raise(ds => ds.AvDataUpdated += null, EventArgs.Empty);
        var steeringAngle = _viewModel.SteeringAngle;

        // Assert: SteeringAngle should reflect the updated actual value from AvStatusData
        Assert.That(steeringAngle, Is.EqualTo(avStatusData.SteeringAngle.Actual));
    }

    [AvaloniaTest]
    public void UI_ShouldDisplayCorrectSpeed_WhenAvDataChanges()
    {
        // Arrange: Set up AvStatusData with Speed.Actual = 100
        var avStatusData = new AvData { Speed = new ValuePair<double> { Actual = 100, Target = 120 } };
        _dataStoreMock.SetupGet(ds => ds.AvStatusData).Returns(avStatusData);

        // Act: Create the window and simulate the UI environment
        var window = new DataView
        {
            DataContext = _viewModel
        };

        // Simulate AvDataUpdated event to update the UI
        _dataStoreMock.Raise(ds => ds.AvDataUpdated += null, EventArgs.Empty);

        // Assert: Check the SpeedDisplay TextBlock to ensure it reflects the correct value
        var speedTextBlock = window.FindControl<TextBlock>("SpeedDisplay");
        Assert.That(speedTextBlock?.Text, Is.EqualTo("Speed: 100"));
    }

    [AvaloniaTest]
    public void UI_ShouldDisplayCorrectBrakeActuation_WhenAvDataChanges()
    {
        // Arrange: Set up AvStatusData with BrakeActuation.Actual = 45
        var avStatusData = new AvData { BrakeActuation = new ValuePair<double> { Actual = 45, Target = 50 } };
        _dataStoreMock.SetupGet(ds => ds.AvStatusData).Returns(avStatusData);

        // Act: Create the window and simulate the UI environment
        var window = new DataView
        {
            DataContext = _viewModel
        };

        // Simulate AvDataUpdated event to update the UI
        _dataStoreMock.Raise(ds => ds.AvDataUpdated += null, EventArgs.Empty);

        // Assert: Check the BrakeActuationDisplay TextBlock to ensure it reflects the correct value
        var brakeActuationTextBlock = window.FindControl<TextBlock>("BrakeActuationDisplay");
        Assert.That(brakeActuationTextBlock?.Text, Is.EqualTo("Brake Actuation: 45"));
    }
}