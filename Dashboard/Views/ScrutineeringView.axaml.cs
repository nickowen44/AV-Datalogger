using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Dashboard.Models;
using Microsoft.Extensions.Logging;

namespace Dashboard.Views;

public partial class ScrutineeringView : UserControl
{
    private const int StepCount = 12;
    private readonly ILogger<ScrutineeringView> _logger;
    private readonly List<ReceiptStep> _steps;

    public ScrutineeringView(ILogger<ScrutineeringView> logger)
    {
        _logger = logger;
        _steps = new List<ReceiptStep>();

        InitializeComponent();
        InitializeSteps(StepCount);
        PopulateAllStepsList();
    }

    /// <summary>
    ///     Initialise each step as failed when the tool first runs.
    /// </summary>
    /// <param name="count">The number of steps in the AV_Inspection_flow yaml file.</param>
    private void InitializeSteps(int count)
    {
        _steps.Clear();

        for (var i = 1; i <= count; i++)
            // The "o" ensures the time is formatted as an ISO string
            _steps.Add(new ReceiptStep { Id = $"{i}", IsPassed = false, Date = DateTime.UtcNow.ToString("o") });
    }

    public void Next(object source, RoutedEventArgs args)
    {
        Slides.Next();
    }

    public void Previous(object source, RoutedEventArgs args)
    {
        Slides.Previous();
    }

    private void OnPass(object sender, RoutedEventArgs e)
    {
        OnStepClick(sender, true);
    }

    private void OnFail(object sender, RoutedEventArgs e)
    {
        OnStepClick(sender, false);
    }

    /// <summary>
    ///     When the Pass/Fail button is clicked, set that step to passed/failed and log to the console as well.
    /// </summary>
    private void OnStepClick(object sender, bool isPassed)
    {
        var button = sender as Button;
        var id = Convert.ToString(button?.Tag);
        var step = _steps.FirstOrDefault(s => s.Id == id);

        if (step == null) return;
        if (step.IsPassed == isPassed) return;
        step.IsPassed = isPassed;
        step.Date = DateTime.UtcNow.ToString("o");

        _logger.LogInformation("Step {id} {status} at {date}", id, isPassed ? "passed" : "failed", step.Date);

        PopulateAllStepsList();
    }

    /// <summary>
    ///     Reset all the data on the page to what it is initialised to on page reset.
    /// </summary>
    private void OnReset(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("Scrutineering inspection has been reset");

        Slides.SelectedIndex = 0;
        InitializeSteps(StepCount);
        PopulateAllStepsList();
    }

    /// <summary>
    ///     Populate the list in the frontend so it updates dynamically.
    /// </summary>
    private void PopulateAllStepsList()
    {
        // Retrieve the brushes from the resource dictionary
        var passBrush = (SolidColorBrush)Application.Current?.Resources["PassColour"]!;
        var failBrush = (SolidColorBrush)Application.Current?.Resources["FailColour"]!;

        AllStepsList.ItemsSource = _steps.Select(s => new TextBlock
        {
            Text = $"Step {s.Id.ToString()} {(s.IsPassed ? "Passed" : "Failed")}",
            Foreground = s.IsPassed ? passBrush : failBrush,
            Margin = new Thickness(0, 5, 0, 5)
        }).ToList();
    }
}