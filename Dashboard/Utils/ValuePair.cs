namespace Dashboard.Utils;

/// <summary>
///     This record represents a pair of values, such as an actual value and a target value.
/// </summary>
public record ValuePair<T>
{
    /// <summary>
    ///     The actual value.
    /// </summary>
    public required T Actual { get; init; }

    /// <summary>
    ///     The target value.
    /// </summary>
    public required T Target { get; init; }
}