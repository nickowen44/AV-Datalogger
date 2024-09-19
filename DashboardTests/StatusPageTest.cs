using Dashboard.Views;
using Dashboard.Models;
using Dashboard.ViewModels;

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
        Assert.AreEqual(10, viewModel.SpeedActual);
    }

    [Test]
    public void TestSpeedTarget()
    {
        _dataStore.SetupGet(x => x.SpeedTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(10, viewModel.SpeedTarget);
    }

    [Test]
    public void TestSteeringAngleActual()
    {
        _dataStore.SetupGet(x => x.SteeringAngleActual).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(10, viewModel.SteeringAngleActual);
    }

    [Test]
    public void TestSteeringAngleTarget()
    {
        _dataStore.SetupGet(x => x.SteeringAngleTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(10, viewModel.SteeringAngleTarget);
    }

    [Test]
    public void TestBrakePressureActual()
    {
        _dataStore.SetupGet(x => x.BrakePressureActual).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(10, viewModel.BrakePressureActual);
    }

    [Test]
    public void TestBrakePressureTarget()
    {
        _dataStore.SetupGet(x => x.BrakePressureTarget).Returns(10);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(10, viewModel.BrakePressureTarget);
    }

    [Test]
    public void TestRemoteEmergencyStopStatus()
    {
        _dataStore.SetupGet(x => x.RemoteEmergencyStopStatus).Returns(true);
        var viewModel = new MainViewModel(_dataStore.Object);
        Assert.AreEqual(true, viewModel.RemoteEmergencyStopStatus);
    }

    [Test]
    public void TestDispose()
    {
        var viewModel = new MainViewModel(_dataStore.Object);
        viewModel.Dispose();
        _dataStore.Verify(x => x.Dispose(), Times.Once);
    }
}

