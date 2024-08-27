namespace Dashboard.Utils;

/// <summary>
///     This class represents a pair of values, such as an actual value and a target value.
/// </summary>
public class ValuePair<T>
{
    /// <summary>
    ///     The actual value.
    /// </summary>
    public T Actual { get; }

    /// <summary>
    ///     The target value.
    /// </summary>
    public T Target { get; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="ValuePair{T}" /> class.
    /// </summary>
    /// <param name="actual">The actual value.</param>
    /// <param name="target">The target value.</param>
    public ValuePair(T actual, T target)
    {
        Actual = actual;
        Target = target;
    }
}