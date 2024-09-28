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
}

// More tests to be implemented when Data is connected to StatusView Page