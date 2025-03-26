using System.Security.Cryptography;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// An AT protocol time-based ID.
/// </summary>
public readonly struct Tid
{
    /// <summary>
    /// The Base32 sort alphabet.
    /// </summary>
    private const string Base32SortAlphabet = "234567abcdefghijklmnopqrstuvwxyz";
    
    /// <summary>
    /// The TID value.
    /// </summary>
    public readonly string Value;

    /// <summary>
    /// Constructs a TID from an integer representation of the TID.
    /// </summary>
    /// <param name="tid">The TID value.</param>
    private Tid(ulong tid)
    {
        Value = NumericTidToStringTid(tid);
    }
    
    /// <summary>
    /// Constructs a new TID for the current time.
    /// </summary>
    /// <returns>The current time.</returns>
    public static Tid NewTid()
    {
        var microseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000;
        var randomness = (uint)RandomNumberGenerator.GetInt32(int.MaxValue);

        var value = (ulong)(((microseconds & 0x1F_FFFF_FFFF_FFFF) << 10) | (randomness & 0x3FF));
        
        return new Tid(value);
    }

    /// <summary>
    /// An empty TID.
    /// </summary>
    public static Tid Empty => new Tid(0);

    /// <summary>
    /// Converts a numeric TID to a string TID.
    /// </summary>
    /// <param name="tid">The numeric TID.</param>
    /// <returns>The string TID.</returns>
    private static string NumericTidToStringTid(ulong tid)
    {
        var value = 0x7FFF_FFFF_FFFF_FFFF & tid;
        var output = "";
        for (var i = 0; i < 13; i++)
        {
            output = Base32SortAlphabet[(int)(value & 0x1F)] + output;
            value >>= 5;
        }

        return output;
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
}