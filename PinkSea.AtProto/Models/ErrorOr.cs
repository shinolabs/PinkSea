namespace PinkSea.AtProto.Models;

/// <summary>
/// A type that represents either an error or a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class ErrorOr<T>
{
    /// <summary>
    /// The value.
    /// </summary>
    public T? Value { get; init; }
    
    /// <summary>
    /// The error.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Is this an error?
    /// </summary>
    public bool IsError => Error is not null;
    
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The new ErrorOr</returns>
    public static ErrorOr<T> Ok(T value)
    {
        return new ErrorOr<T>
        {
            Value = value
        };
    }
    
    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="reason">The reason.</param>
    /// <returns>The new ErrorOr</returns>
    public static ErrorOr<T> Fail(string reason)
    {
        return new ErrorOr<T>
        {
            Error = reason
        };
    }
}