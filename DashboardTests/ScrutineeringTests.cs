using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Dashboard.Models;
using Dashboard.Utils;
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

        // Assert the scrutineering view renders correctly
        Assert.Multiple(() => { Assert.That(carousel, Is.Not.Null); });


        // Assert the carousel has the correct number of slides that is in the yaml file
        const int scrutineeringInspectionChecks = 12;
        Assert.That(carousel.ItemCount, Is.EqualTo(scrutineeringInspectionChecks));

        // Assert that each carousel slide contains the information from the yaml file.
        for (var i = 0; i < scrutineeringInspectionChecks; i++)
        {
            // ID must be asserted this way as double can not be "null" by definition sense however the slide should
            // still have the key.
            Assert.That(carousel.SelectedItem?.ToString(), Does.Contain("Id"));
            Assert.That(((StepData)carousel.SelectedItem).Step, Is.Not.Null);
            Assert.That(((StepData)carousel.SelectedItem).Title, Is.Not.Null);
            // Caution and measurements must be asserted this way as the current step may not have anything to measure
            // with our tool however the slide should still have the key.
            Assert.That(carousel.SelectedItem.ToString(), Does.Contain("Caution"));
            Assert.That(carousel.SelectedItem.ToString(), Does.Contain("Measurements"));
            carousel.Next();
        }
    }

    [AvaloniaTest]
    public void TestScrutineeringViewCorrectlyDisplaysDvDataIfMeasurementsIsPresent()
    {
        // Arrange
        // Create a window with the ScrutineeringView as its content for rendering purposes.
        var window = new Window
        {
            Content = new ScrutineeringView(),
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        window.Show();
        var carousel = window.GetVisualDescendants().OfType<Carousel>().FirstOrDefault();

        // We need to loop through each Item to get each container for the slide
        // to find the text block.
        for (var i = 0; i < carousel.ItemCount; i++)
        {
            var container = carousel.ContainerFromIndex(i);

            // First we need to traverse the visual tree and find the stack panel which has the text block inside of it.
            // We need to do this because items inside a DataTemplate is not directly accessible using FindControl
            // on the Carousel itself. It is overly complicated for no reason.
            var stackPanel = container?.GetVisualDescendants().OfType<StackPanel>()
                .FirstOrDefault(panel => panel.Name == "StackPanel");
            var textBlock = stackPanel?.GetVisualDescendants().OfType<TextBlock>()
                .FirstOrDefault(textBlock => textBlock.Name == "DvData");

            // DV Data should only be displayed if the specific slide has measurements to be displayed.
            var slide = (StepData)carousel.Items[i]!;
            if (slide.Measurements != null)
            {
                // Assert the text block is visible.
                Assert.IsTrue(textBlock?.IsVisible);
                Assert.That(textBlock?.Text, Is.EqualTo("DV Data"));
            }
            else
            {
                // Assert it is not visible.
                Assert.IsFalse(textBlock?.IsVisible);
            }

            // Change to next slide
            carousel.Next();
            // Update UI cause slide has changed.
            Dispatcher.UIThread.RunJobs();
        }
    }

    [AvaloniaTest]
    public void TestScrutineeringViewSlideButtonsAndExpander()
    {
        // Arrange
        // Create a window with the ScrutineeringView as its content for rendering purposes.
        var window = new Window
        {
            Content = new ScrutineeringView(),
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        window.Show();

        // Get the expander and the items inside it prior to checking each step
        var expander = window.GetLogicalDescendants().OfType<Expander>()
            .FirstOrDefault(e => e.Name == "AllStepsExpander");
        Assert.That(expander, Is.Not.Null);

        // It's not expanded by default so to test the contents we need to expand it.
        expander.IsExpanded = true;
        var stepsList = window.GetLogicalDescendants().OfType<ItemsControl>()
            .FirstOrDefault(e => e.Name == "AllStepsList");
        Assert.That(stepsList, Is.Not.Null);

        var carousel = window.GetVisualDescendants().OfType<Carousel>().FirstOrDefault();

        // We need to loop through each Item to get each container for the slide
        // to find the text block.
        for (var i = 0; i < carousel.ItemCount; i++)
        {
            var container = carousel.ContainerFromIndex(i);

            // First we need to traverse the visual tree and find the stack panel which has the text block inside of it.
            // We need to do this because items inside a DataTemplate is not directly accessible using FindControl
            // on the Carousel itself. It is overly complicated for no reason.
            var stackPanel = container?.GetVisualDescendants().OfType<StackPanel>()
                .FirstOrDefault(panel => panel.Name == "StackPanel");
            var passButton = stackPanel?.GetVisualDescendants().OfType<Button>()
                .FirstOrDefault(button => button.Content?.ToString() == "Pass");
            var failButton = stackPanel?.GetVisualDescendants().OfType<Button>()
                .FirstOrDefault(button => button.Content?.ToString() == "Fail");

            Assert.That(passButton, Is.Not.Null);
            Assert.That(failButton, Is.Not.Null);

            // Test prior to button click that the step is "failed" by default
            var items = stepsList.Items.OfType<TextBlock>().ToList();
            Assert.That(items[i].Text, Is.EqualTo($"Step {i + 1} Failed"));

            // Click pass and update UI after button was clicked. Check expander shows passed.
            passButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            items = stepsList.Items.OfType<TextBlock>().ToList();
            Assert.That(items[i].Text, Is.EqualTo($"Step {i + 1} Passed"));

            // Click fail and update UI after button was clicked. Check expander shows failed.
            failButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            items = stepsList.Items.OfType<TextBlock>().ToList();
            Assert.That(items[i].Text, Is.EqualTo($"Step {i + 1} Failed"));

            // Change to next slide
            carousel.Next();
            // Update UI cause slide has changed.
            Dispatcher.UIThread.RunJobs();
        }
    }

    [AvaloniaTest]
    public void TestScrutineeringViewSlideHasVariablesFromSimulator()
    {
        // Arrange
        _dataStore.SetupGet(x => x.AvStatusData).Returns(new AvData
        {
            AutonomousSystemState = 1,
            SteeringAngle = new ValuePair<double>
            {
                Actual = 90, Target = 90
            },
            ServiceBrakeState = true,
            MissionIndicator = 2,
            EmergencyBrakeState = 3
        });

        // Create a window with the ScrutineeringView as its content for rendering purposes.
        var window = new Window
        {
            Content = new ScrutineeringView(),
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        window.Show();

        var carousel = window.GetVisualDescendants().OfType<Carousel>().FirstOrDefault();
        // We need to loop through each Item to get each container for the slide
        // to find the text block.
        for (var i = 0; i < carousel.ItemCount; i++)
        {
            // First we need to traverse the visual tree and find the stack panel which has the text block inside of it.
            // We need to do this because items inside a DataTemplate is not directly accessible using FindControl
            // on the Carousel itself. It is overly complicated for no reason.
            var container = carousel.ContainerFromIndex(i);
            var stackPanel = container?.GetVisualDescendants().OfType<StackPanel>()
                .FirstOrDefault(panel => panel.Name == "StackPanel");

            // Slide 10 covers all 5 measurements that are to be checked in the Inspection.
            if (i == 9)
            {
                var contentControl = stackPanel?.GetVisualDescendants().OfType<ContentControl>()
                    .FirstOrDefault(control => control.Name == "ContentControl");
                var textBlocks = contentControl?.GetVisualDescendants().OfType<TextBlock>().ToList();

                // 7.5 Contains Autonomous System State and Steering Angle
                var expectedTexts = new List<string>
                {
                    "Autonomous Mission Indicator: 2",
                    "Autonomous System State: 1",
                    "Emergency Brake State: 3",
                    "Service Brake State: true",
                    "Steering Angle: 90",
                };

                for (var j = 0; j < textBlocks?.Count; j++)
                    Assert.That(textBlocks[j].Text, Is.EqualTo(expectedTexts[j]));
            }

            // Change to next slide
            carousel.Next();
            // Update UI cause slide has changed.
            Dispatcher.UIThread.RunJobs();
        }
    }

    [AvaloniaTest]
    public void TestScrutineeringViewResetFeature()
    {
        // Arrange
        // Create a window with the ScrutineeringView as its content for rendering purposes.
        var window = new Window
        {
            Content = new ScrutineeringView(),
            DataContext = new ScrutineeringViewModel(_dataStore.Object)
        };

        window.Show();

        // Get the expander and the items inside it prior to checking each step
        var expander = window.GetLogicalDescendants().OfType<Expander>()
            .FirstOrDefault(e => e.Name == "AllStepsExpander");
        Assert.That(expander, Is.Not.Null);

        // It's not expanded by default so to test the contents we need to expand it.
        expander.IsExpanded = true;
        var stepsList = window.GetLogicalDescendants().OfType<ItemsControl>()
            .FirstOrDefault(e => e.Name == "AllStepsList");
        Assert.That(stepsList, Is.Not.Null);

        var resetButton = window?.GetVisualDescendants().OfType<Button>()
            .FirstOrDefault(button => button.Content?.ToString() == "Reset");
        Assert.That(resetButton, Is.Not.Null);

        var carousel = window.GetVisualDescendants().OfType<Carousel>().FirstOrDefault();
        // We need to loop through each Item to get each container for the slide
        // to find the text block.
        for (var i = 0; i < carousel.ItemCount; i++)
        {
            // Pass a couple of steps.
            if (i is 1 or 2 or 5)
            {
                var container = carousel.ContainerFromIndex(i);

                // First we need to traverse the visual tree and find the stack panel which has the text block inside of it.
                // We need to do this because items inside a DataTemplate is not directly accessible using FindControl
                // on the Carousel itself. It is overly complicated for no reason.
                var stackPanel = container?.GetVisualDescendants().OfType<StackPanel>()
                    .FirstOrDefault(panel => panel.Name == "StackPanel");
                var passButton = stackPanel?.GetVisualDescendants().OfType<Button>()
                    .FirstOrDefault(button => button.Content?.ToString() == "Pass");

                Assert.That(passButton, Is.Not.Null);

                // Click pass and update UI after button was clicked. Check expander shows passed.
                passButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();
                var items = stepsList.Items.OfType<TextBlock>().ToList();
                Assert.That(items[i].Text, Is.EqualTo($"Step {i + 1} Passed"));
            }

            // Change to next slide
            carousel.Next();
            // Update UI cause slide has changed.
            Dispatcher.UIThread.RunJobs();
        }

        // Check we are at the last slide (12 slides but 11 cause 0 indexed)
        var currentIndex = carousel.SelectedIndex;
        Assert.That(currentIndex, Is.EqualTo(11));

        // Click reset button, update UI that button was clicked
        resetButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();

        // Check the current slide is now the first slide
        currentIndex = carousel.SelectedIndex;
        Assert.That(currentIndex, Is.EqualTo(0));

        // Check all steps in the expander have been set to failed.
        var expanderItems = stepsList.Items.OfType<TextBlock>().ToList();
        for (var i = 0; i < carousel.ItemCount; i++)
            Assert.That(expanderItems[i].Text, Is.EqualTo($"Step {i + 1} Failed"));
    }
}