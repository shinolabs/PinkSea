using System.Text.Json;
using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// The default XRPC handler.
/// </summary>
public class DefaultXrpcHandler(
    IServiceProvider serviceProvider) : IXrpcHandler
{
    /// <inheritdoc />
    public async Task<IXrpcErrorOr?> HandleXrpc(
        string nsid,
        HttpContext context)
    {
        var implementationType = XrpcTypeResolver.GetTypeMappingFor(nsid);
        if (implementationType is null)
            return null;
        
        var service = serviceProvider.GetService(implementationType.HandlerType);
        if (service is null)
            return null;

        var requestObject = await GetObjectForType(implementationType, context);
        if (requestObject is null)
            return null;

        var method = service.GetType()
            .GetMethod("Handle")!;
        
        var task = (Task)method.Invoke(service, [requestObject])!;
        await task.ConfigureAwait(false);

        var resultProperty = task.GetType()
            .GetProperty("Result")!;

        var errorOr = resultProperty.GetValue(task) as IXrpcErrorOr;
        return errorOr;
    }

    /// <summary>
    /// Gets the object from the HTTP context for a given type mapping.
    /// </summary>
    /// <param name="typeMapping">The XRPC type mapping.</param>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The deserialized object.</returns>
    private static async Task<object?> GetObjectForType(
        XrpcTypeMapping typeMapping,
        HttpContext context)
    {
        // If this is a procedure, we just have to parse JSON. Nice!
        if (!typeMapping.IsQuery)
        {
            var json = await JsonSerializer.DeserializeAsync(
                context.Request.Body,
                typeMapping.RequestType);

            return json;
        }
        
        // Dirty hack :3
        var queryDict = context
            .Request
            .Query
            .ToDictionary(k => k.Key, v => v.Value.ToString());

        var queryJson = JsonSerializer.Serialize(queryDict);
        return JsonSerializer.Deserialize(queryJson, typeMapping.RequestType, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        });
    }
}