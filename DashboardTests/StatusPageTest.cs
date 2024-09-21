using Dashboard.Models;
using Dashboard.ViewModels;
using Moq;

namespace DashboardTests;

[TestFixture]

public class OverviewPageTest
{
    [SetUp]
    public void SetUp()
    {
        _dataStore = new Mock<IDataStore>();
    }

    private Mock<IDataStore> _dataStore;

    [Test]
    public void TestSpeedActual()
    {
        _dataStore.SetupGet(x => x.SpeedActual).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.SpeedActual, Is.EqualTo(10));
    }

    [Test]
    public void TestSpeedTarget()
    {
        _dataStore.SetupGet(x => x.SpeedTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.SpeedTarget, Is.EqualTo(10));
    }

    [Test]
    public void TestSteeringAngleActual()
    {
        _dataStore.SetupGet(x => x.SteeringAngleActual).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.SteeringAngleActual, Is.EqualTo(10));
    }

    [Test]
    public void TestSteeringAngleTarget()
    {
        _dataStore.SetupGet(x => x.SteeringAngleTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.SteeringAngleTarget, Is.EqualTo(10));
    }

    [Test]
    public void TestBrakePressureActual()
    {
        _dataStore.SetupGet(x => x.BrakePressureActual).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.BrakePressureActual, Is.EqualTo(10));
    }

    [Test]
    public void TestBrakePressureTarget()
    {
        _dataStore.SetupGet(x => x.BrakePressureTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.BrakePressureTarget, Is.EqualTo(10));
    }

    [Test]
    public void TestRemoteEmergencyStopStatus()
    {
        _dataStore.SetupGet(x => x.RemoteEmergencyStopStatus).Returns(true);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.That(viewModel.RemoteEmergencyStopStatus, Is.EqualTo(true));
    }

    [Test]
    public void TestDispose()
    {
        var viewModel = new MainViewModel(_dataStore.Object);
        viewModel.Dispose();
        _dataStore.Verify(x => x.Dispose(), Times.Once);
    }
}

