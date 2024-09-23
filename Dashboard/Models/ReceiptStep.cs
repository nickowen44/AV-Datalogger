namespace Dashboard.Models;

/// <summary>
///     Used to store which steps passed/failed the scrutineering checks and then display on the frontend.
/// </summary>
public class ReceiptStep
{
    public string Id { get; set; }
    public bool IsPassed { get; set; }
}