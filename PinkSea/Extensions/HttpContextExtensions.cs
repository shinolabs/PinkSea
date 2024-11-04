namespace PinkSea.Extensions;

/// <summary>
/// Extensions for the <see cref="HttpContext"/> class.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// The name of the state token.
    /// </summary>
    private const string StateTokenName = "PINKSEA_STATE_TOKEN";

    /// <summary>
    /// Sets the state token.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="value">The value to set it to.</param>
    public static void SetStateToken(this HttpContext context, string value)
        => context.Items[StateTokenName] = value;

    /// <summary>
    /// Gets the state token from the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The value of the token, if it exists.</returns>
    public static string? GetStateToken(this HttpContext context)
        => context.Items.TryGetValue(StateTokenName, out var token) ? (string)token! : null;
}