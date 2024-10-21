using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless.NUnit;
using Dashboard.ViewModels;
using Dashboard.Views;

namespace DashboardTests;

[TestFixture]
public class AboutTests
{
    private AboutView _view;

    [SetUp]
    public void Setup()
    {
        _view = new AboutView
        {
            DataContext = new AboutViewModel()
        };
    }

    [AvaloniaTest]
    public void AboutViewShouldDisplayCorrectTeamMembers()
    {
        // Arrange
        var expectedLines = new[]
        {
            "Erin Lumsden",
            "Sarmad Wani",
            "Ariadne Ventura-Koumides",
            "Nicholas Dagher",
            "Richard Quach"
        };

        // Assert
        var teamMembersTextBlock = _view.FindControl<TextBlock>("TeamMembers");

        Assert.That(teamMembersTextBlock, Is.Not.Null);
        Assert.That(teamMembersTextBlock.Inlines, Is.Not.Null);

        // Get all the <Run> elements in the TextBlock, skipping the first one (which is the header)
        var lines = teamMembersTextBlock.Inlines.OfType<Run>().Skip(1).ToArray();
        Assert.That(lines, Has.Length.EqualTo(expectedLines.Length));

        // Iterate over each line and expected line as a zipped pair
        foreach (var (line, expectedLine) in lines.Zip(expectedLines))
            Assert.That(line.Text, Does.Contain(expectedLine));
    }

    [AvaloniaTest]
    public void AboutViewShouldDisplaySupervisor()
    {
        // Arrange
        const string expectedSupervisor = "Project Supervisor: Dr. Prabha Rajagopal";

        // Assert
        var supervisorTextBlock = _view.FindControl<TextBlock>("Supervisor");

        Assert.That(supervisorTextBlock, Is.Not.Null);
        Assert.That(supervisorTextBlock.Inlines, Is.Not.Null);

        var label = ((Span)supervisorTextBlock.Inlines.First()).Inlines.OfType<Run>().First().Text;
        var text = ((Run)supervisorTextBlock.Inlines.Last()).Text;

        Assert.That($"{label}{text}", Is.EqualTo(expectedSupervisor));
    }

    [AvaloniaTest]
    public void AboutViewShouldDisplayCorrectSoftwareInformation()
    {
        // Arrange
        var expectedCommitText = $"Version: {AboutViewModel.GitCommit}";
        var expectedBranchText = $"Branch: {AboutViewModel.GitBranch}";

        // Assert
        var gitCommitTextBlock = _view.FindControl<TextBlock>("GitCommit");
        var gitBranchTextBlock = _view.FindControl<TextBlock>("GitBranch");

        Assert.Multiple(() =>
        {
            Assert.That(gitCommitTextBlock, Is.Not.Null);
            Assert.That(gitBranchTextBlock, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(gitCommitTextBlock.Text, Is.EqualTo(expectedCommitText));
            Assert.That(gitBranchTextBlock.Text, Is.EqualTo(expectedBranchText));
        });
    }

    [AvaloniaTest]
    public void AboutViewShouldDisplayCurrentYear()
    {
        // Arrange
        var expectedCurrentYearText = $"© {AboutViewModel.CurrentYear} Nuvotion Pty Ltd. All rights reserved.";

        // Assert
        var currentYearTextBlock = _view.FindControl<TextBlock>("CurrentYear");

        Assert.That(currentYearTextBlock, Is.Not.Null);
        Assert.That(currentYearTextBlock.Text,
            Is.EqualTo(expectedCurrentYearText));
    }

    [AvaloniaTest]
    public void AboutViewShouldDisplayLogo()
    {
        // Assert
        var logoImage = _view.FindControl<Image>("Logo");

        Assert.That(logoImage, Is.Not.Null);
        Assert.That(logoImage.Source, Is.Not.Null);
    }
}