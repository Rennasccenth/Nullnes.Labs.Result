namespace Nullnes.Labs.Result.Abstractions;

/// <summary>
/// Represents a no meaningful value. It's often refereed as Unit, () or empty on some functional programming languages. 
/// Since C# doesn't allow us to pass <see cref="Void"/> as a Type argument, this comes handy in such cases.
/// </summary>
public readonly struct None
{
    /// <inheritdoc cref="None" />
    public static readonly None Value;
}