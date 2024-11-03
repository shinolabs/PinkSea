namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// The XRPC attribute, determining the namespace.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class XrpcAttribute : Attribute
{
    /// <summary>
    /// The namespace.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Sets the XRPC attribute.
    /// </summary>
    /// <param name="namespace">The namespace to use.</param>
    public XrpcAttribute(string @namespace)
    {
        Namespace = @namespace;
    }
}