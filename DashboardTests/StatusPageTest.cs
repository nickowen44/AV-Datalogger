using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class OverviewPageTest
{
    private Mock<IDataStore> _dataStore;

    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }


    [AvaloniaTest]
    public void TestThatStatusPageRendersCorrectly()
    {
        var window = new StatusView
        {
            DataContext = new StatusViewModel(_dataStore.Object)
        };

        Assert.IsNotNull(window);

        // Make sure there is a icon for RES
        var resIcon = window.FindControl<Image>("ResIcon");
        Assert.IsNotNull(resIcon);
    }

    [AvaloniaTest]
    public void TestStatusPageValues()
    {
        _dataStore.SetupGet(x => x.AvStatusData).Returns(new AvData
        {
            Speed = new ValuePair<double>
            {
                Actual = 50, Target = 20
            },
            SteeringAngle = new ValuePair<double>
            {
                Actual = 90, Target = 80
            },
            BrakeActuation = new ValuePair<double>
            {
                Actual = 100, Target = 80
            },
            MotorMoment = new ValuePair<double>
            {
                Actual = 100, Target = 80
            }
        });

        var window = new StatusView
        {
            DataContext = new StatusViewModel(_dataStore.Object)
        };
        Dispatcher.UIThread.RunJobs();
        Assert.IsNotNull(window);

        // Make sure there is a icon for RES
        var MotorSliderActual = window.FindControl<Slider>("MotorSliderActual");
        var MotorSliderTarget = window.FindControl<Slider>("MotorSliderTarget");
        var MotorTextActual = window.FindControl<TextBlock>("MotorTextActual");
        var MotorTextTarget = window.FindControl<TextBlock>("MotorTextTarget");
        var SpeedTextActual = window.FindControl<TextBlock>("SpeedTextActual");
        var SpeedTextTarget = window.FindControl<TextBlock>("SpeedTextTarget");

        Assert.Multiple(() =>
        {
            Assert.That(MotorSliderActual, Is.Not.Null);
            Assert.That(MotorSliderTarget, Is.Not.Null);
            Assert.That(MotorTextActual, Is.Not.Null);
            Assert.That(MotorTextTarget, Is.Not.Null);
            Assert.That(SpeedTextActual, Is.Not.Null);
            Assert.That(SpeedTextTarget, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(MotorSliderActual.Value, Is.EqualTo(100));
            Assert.That(MotorSliderTarget.Value, Is.EqualTo(80));
            Assert.That(MotorTextActual.Text, Is.EqualTo("100.0%"));
            Assert.That(MotorTextTarget.Text, Is.EqualTo("80.0%"));
            Assert.That(SpeedTextActual.Text, Is.EqualTo("50 km/h"));
            Assert.That(SpeedTextTarget.Text, Is.EqualTo("20 km/h"));
        });
    }

    [AvaloniaTest]
    public void TestGpsPageValues()
    {
        _dataStore.SetupGet(x => x.GpsData).Returns(new GpsData
        {
            // Set the values for the GPS Data
            Latitude = 10.020930129,
            Longitude = -128.1230802,
            AltitudeMetres = 10.23,
            AltitudeKilometres = 10.23,
            MetresPerSecond = 10.203,
            KilometresPerHour = 10.23,
            HdopFixAge = 6,
            Hdop = 12.32,
            SatFixAge = 6,
            SatCount = 3,
            SpeedFixAge = 3,
            AltFixAge = 1
        });

        var window = new StatusView
        {
            DataContext = new StatusViewModel(_dataStore.Object)
        };
        Dispatcher.UIThread.RunJobs();
        Assert.IsNotNull(window);
        
        // Find the TabControl in the UserControl
        var tabControl = window.FindControl<TabControl>("MainTabControl");

        // Assert that the TabControl is found
        Assert.NotNull(tabControl);

        // Select the GPS tab by index (assuming GPS is the second tab, index 1)
        tabControl.SelectedIndex = 1;

        var Latitude = window.FindControl<TextBlock>("Latitude");
        var Longitude = window.FindControl<TextBlock>("Longitude");
        var AltitudeMetres = window.FindControl<TextBlock>("AltitudeMetres");
        var AltitudeKilometres = window.FindControl<TextBlock>("AltitudeKilometres");
        var MetresPerSecond = window.FindControl<TextBlock>("MetresPerSecond");
        var KilometresPerHour = window.FindControl<TextBlock>("KilometresPerHour");
        var HdopFixAge = window.FindControl<TextBlock>("HdopFixAge");
        var Hdop = window.FindControl<TextBlock>("Hdop");
        var SatFixAge = window.FindControl<TextBlock>("SatFixAge");
        var SatCount = window.FindControl<TextBlock>("SatCount");
        var SpeedFixAge = window.FindControl<TextBlock>("SpeedFixAge");
        var AltFixAge = window.FindControl<TextBlock>("AltFixAge");

        Assert.Multiple(() =>
        {
            Assert.That(Latitude, Is.Not.Null);
            Assert.That(Longitude, Is.Not.Null);
            Assert.That(AltitudeMetres, Is.Not.Null);
            Assert.That(AltitudeKilometres, Is.Not.Null);
            Assert.That(MetresPerSecond, Is.Not.Null);
            Assert.That(KilometresPerHour, Is.Not.Null);
            Assert.That(HdopFixAge, Is.Not.Null);
            Assert.That(Hdop, Is.Not.Null);
            Assert.That(SatFixAge, Is.Not.Null);
            Assert.That(SatCount, Is.Not.Null);
            Assert.That(SpeedFixAge, Is.Not.Null);
            Assert.That(AltFixAge, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(Latitude.Text, Is.EqualTo("10.020930129"));
            Assert.That(Longitude.Text, Is.EqualTo("-128.1230802"));
            Assert.That(AltitudeMetres.Text, Is.EqualTo("10.23"));
            Assert.That(AltitudeKilometres.Text, Is.EqualTo("10.23"));
            Assert.That(MetresPerSecond.Text, Is.EqualTo("10.203"));
            Assert.That(KilometresPerHour.Text, Is.EqualTo("10.23"));
            Assert.That(HdopFixAge.Text, Is.EqualTo("6"));
            Assert.That(Hdop.Text, Is.EqualTo("12.32"));
            Assert.That(SatFixAge.Text, Is.EqualTo("6"));
            Assert.That(SatCount.Text, Is.EqualTo("3"));
            Assert.That(SpeedFixAge.Text, Is.EqualTo("3"));
            Assert.That(AltFixAge.Text, Is.EqualTo("1"));
        });
    }

    [AvaloniaTest]
    public void TestResPageValues()
    {
        _dataStore.SetupGet(x => x.ResData).Returns(new ResData
        {
            // Set the values for the RES data
            K2State = true,
            K3State = true,
            ResRadioQuality = 3,
            ResNodeId = 3,
            ResState = true
        });

        var window = new StatusView
        {
            DataContext = new StatusViewModel(_dataStore.Object)
        };
        Dispatcher.UIThread.RunJobs();
        Assert.IsNotNull(window);
        
        // Find the TabControl in the UserControl
        var tabControl = window.FindControl<TabControl>("MainTabControl");

        // Assert that the TabControl is found
        Assert.NotNull(tabControl);

        // Select the GPS tab by index (assuming GPS is the second tab, index 1)
        tabControl.SelectedIndex = 2;

        var K2State = window.FindControl<TextBlock>("K2State");
        var K3State = window.FindControl<TextBlock>("K3State");
        var ResNodeId = window.FindControl<TextBlock>("ResNodeId");
        var ResRadioQuality = window.FindControl<TextBlock>("ResRadioQuality");
        var ResState = window.FindControl<TextBlock>("ResTab");
        
        Assert.Multiple(() =>
        {
            Assert.That(K2State, Is.Not.Null);
            Assert.That(K3State, Is.Not.Null);
            Assert.That(ResNodeId, Is.Not.Null);
            Assert.That(ResRadioQuality, Is.Not.Null);
            Assert.That(ResState, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(K2State.Text, Is.EqualTo("True"));
            Assert.That(K3State.Text, Is.EqualTo("True"));
            Assert.That(ResNodeId.Text, Is.EqualTo("3"));
            Assert.That(ResRadioQuality.Text, Is.EqualTo("3"));
            Assert.That(ResState.Text, Is.EqualTo("True"));
        });
    }
}