using System.Reflection;

namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// Ew yucky static class for holding the type mapping for Xrpc endpoints.
/// </summary>
public static class XrpcTypeResolver
{
    /// <summary>
    /// The type mappings from namespace IDs.
    /// </summary>
    private static readonly Dictionary<string, XrpcTypeMapping> _typeMappings = new();
    
    /// <summary>
    /// Register a type within the XRPC type resolver.
    /// </summary>
    /// <param name="type">The type to register.</param>
    public static void Register(Type type)
    {
        var attrib = type.GetCustomAttribute<XrpcAttribute>();
        if (attrib is null)
            return;

        var maybeQuery = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType
                                 && i.GetGenericTypeDefinition() == typeof(IXrpcQuery<,>));

        var maybeProcedure = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType
                                 && i.GetGenericTypeDefinition() == typeof(IXrpcProcedure<,>));

        if (maybeQuery is null && maybeProcedure is null)
            return;

        var actualImplType = maybeQuery ?? maybeProcedure!;
        var typeMapping = new XrpcTypeMapping(
            actualImplType.GenericTypeArguments[0],
            actualImplType.GenericTypeArguments[1],
            type,
            maybeQuery is not null);

        _typeMappings[attrib.Namespace] = typeMapping;
    }

    /// <summary>
    /// Gets the XRPC type mapping for a namespace id.
    /// </summary>
    /// <param name="nsid">The namespace id.</param>
    /// <returns>The XRPC type mapping.</returns>
    public static XrpcTypeMapping? GetTypeMappingFor(string nsid)
    {
        _typeMappings.TryGetValue(nsid, out var xrpcTypeMapping);
        return xrpcTypeMapping;
    }
}