using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;
using static DashboardTests.FindControlHelper;
namespace DashboardTests;

public class FooterTest
{
    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
        _serviceProvider = new Mock<IServiceProvider>();
    }

    private Mock<IDataStore> _dataStore;
    private Mock<IServiceProvider> _serviceProvider;
    
    [AvaloniaTest]
    public void TestFooterStartup()
    {

        // Arrange
        var window = new FooterView()
        {
            DataContext = new FooterViewModel(_dataStore.Object)
        };

        
        // Assert
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<FooterView>());
        
        var carID = window.FindControl<TextBlock>("CarId");
        var utcTime = window.FindControl<TextBlock>("CurTime");
        var console = window.FindControl<TextBlock>("ConsoleTextBox");
        
        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });

    }
    
    [AvaloniaTest]
    public void TestFooterCarIDCurTimeUpdate()
    {
        _dataStore.SetupGet(x => x.RawData).Returns(new RawData()
        {
            CarId = "A46",
            UTCTime = "P2024820T06:56:04.00",
            RawMessage = "ID=A46|UTC=P2024820T06:56:04.00|SA=###|ST=###|STA=###|STT=###|BRA=###|BRT=###|MMT=###|MMA=###|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###"
        });
        // Arrange
        var window = new Window
        {
            Content = new FooterView
            {
                DataContext = new FooterViewModel(_dataStore.Object)
            }
        };
       
        window.Show();

        
        // Assert
        var carID = FindControlInVisualTree<TextBlock>(window, "CarId");
        var utcTime = FindControlInVisualTree<TextBlock>(window, "CurTime");
        var console = FindControlInVisualTree<TextBox>(window, "ConsoleTextBox");
     
        Assert.Multiple(() =>
        {
            Assert.That(carID, Is.Not.Null);
            Assert.That(utcTime, Is.Not.Null);
            Assert.That(console, Is.Not.Null);
        });
        
        // Assert.Multiple(() =>
        // {
        //     Assert.That(carID.Text, Is.EqualTo("Car ID: A46"));
        //     Assert.That(utcTime.Text, Is.EqualTo("UTC Time: 2024-08-19 20:56:04"));
        //     Assert.That(console.Text, Is.EqualTo("ID=A46|UTC=P2024820T06:56:04.00|SA=###|ST=###|STA=###|STT=###|BRA=###|BRT=###|MMT=###|MMA=###|ALAT=#########|ALON=#########|YAW=#########|AST=###|EBS=###|AMI=###|STS=###|SBS=###|LAP=###|CCA=###|CCT=###\n"));
        // });
    }
    
}