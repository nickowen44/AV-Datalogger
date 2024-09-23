using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Dashboard.Models;

namespace Dashboard.Views;

public partial class ScrutineeringView : UserControl
{
    private const int StepCount = 13;
    private List<ReceiptStep> _steps;

    public ScrutineeringView()
    {
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
        _steps = new List<ReceiptStep>();
        for (var i = 1; i <= count; i++)
        {
            _steps.Add(new ReceiptStep { Id = $"7.{i}", IsPassed = false });
        }
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
        step.IsPassed = isPassed;
        Console.WriteLine($"Step {id} {(isPassed ? "passed" : "failed")}");
        PopulateAllStepsList();
    }

    /// <summary>
    ///     Populate the list in the frontend so it updates dynamically.
    /// </summary>
    private void PopulateAllStepsList()
    {
        AllStepsList.ItemsSource = _steps.Select(s => new TextBlock
        {
            Text = $"Step {s.Id.ToString()} {(s.IsPassed ? "Passed" : "Failed")}",
            Foreground = s.IsPassed ? Brushes.White : Brushes.Red,
            Margin = new Thickness(0, 5, 0, 5),
            FontWeight = FontWeight.Light
        }).ToList();
    }
}