namespace Dashboard.Models;

/// <summary>
///     Used to store which steps passed/failed the scrutineering checks and then display on the frontend.
/// </summary>
public class ReceiptStep
{
    public required string Id { get; set; }
    public required bool IsPassed { get; set; }
    public required string Date { get; set; }
}