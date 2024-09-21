using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Moq;

namespace DashboardTests;

[TestFixture]
public class ScrutineeringTests
{
    [SetUp]
    public void Setup()
    {
        _dataStore = new Mock<IDataStore>();
    }

    private Mock<IDataStore> _dataStore;

    [AvaloniaTest]
    public void TestScrutineeringViewCorrectlyPopulatesCarouselWithYamlData()
    {
        // Arrange
        var window = new ScrutineeringView
        {
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        // Act
        var carousel = window.FindControl<Carousel>("Slides");

        // Assert the scrutineering voew renders correctly
        Assert.Multiple(() =>
        {
            Assert.That(carousel, Is.Not.Null);
        });


        // Assert the carousel has the correct number of slides that is in the yaml file
        var scurtineeringInspectionChecks = 13;
        Assert.That(scurtineeringInspectionChecks, Is.EqualTo(carousel.ItemCount));

        // Assert that each carousel slide contains the information from the yaml file.
        for (var i = 0; i < scurtineeringInspectionChecks; i++)
        {
            // Id must be asserted this way as double can not be "null" by definition sense however the slide should
            // still have the key.
            Assert.That(carousel.SelectedItem.ToString(), Does.Contain("Id"));
            Assert.That(((StepData)carousel.SelectedItem).Step, Is.Not.Null);
            Assert.That(((StepData)carousel.SelectedItem).Inspection, Is.Not.Null);
            // Measurements must be asserted this way as the current step may not have anything to measure with our tool
            // however the slide should still have the key.
            Assert.That(carousel.SelectedItem.ToString(), Does.Contain("Measurements"));
            carousel.Next();
        }
    }

    [AvaloniaTest]
    public void TestScrutineeringViewCorrectlyDisplaysDvDataIfMeasurmentsIsPresent()
    {
        // Arrange
        // Create a window with the ScrutineeringView as its content for rendering purposes.
        var window = new Window
        {
            Content = new ScrutineeringView(),
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        window.Show();
        // Get the Carousel
        // Have to do it in this odd fashion as Avalonia does not trigger the full rendering process with a UserControl View
        // and the Dv data was not being properly rendered so it was not being picked up.
        foreach (var carousel in window.GetVisualDescendants().OfType<Carousel>())
        {
            // We need to loop through each Item to get each container for the slide
            // to find the textblock.
            for (var i = 0; i < carousel.ItemCount; i++)
            {
                var container = carousel.ItemContainerGenerator.ContainerFromIndex(i);

                // First we need to traverse the visual tree and find the stack panel which has the texxtblock inside of it.
                // We need to do this because items inside a DataTemplate is not directly accessible using FindControl
                // on the Carousel itself. It is overly complicated for no reason.
                var stackPanel = container.GetVisualDescendants().OfType<StackPanel>()
                    .FirstOrDefault(panel => panel.Name == "StackPanel");
                var textBlock = stackPanel.GetVisualDescendants().OfType<TextBlock>()
                    .FirstOrDefault(textBlock => textBlock.Name == "DvData");

                // DV Data should only be displayed if the specific slide has measurments to be displayed.
                var slide = (StepData)carousel.Items[i];
                if (slide.Measurements != null)
                {
                    // Assert the textblock is visible.
                    Assert.IsTrue(textBlock.IsVisible);
                    Assert.That(textBlock.Text, Is.EqualTo("DV Data"));
                }
                else
                {
                    // Assert it is not visible.
                    Assert.IsFalse(textBlock.IsVisible);
                }

                // Change to next slide
                carousel.Next();
                // Update UI cause slide has changed.
                Dispatcher.UIThread.RunJobs();
            }
        }
    }
}