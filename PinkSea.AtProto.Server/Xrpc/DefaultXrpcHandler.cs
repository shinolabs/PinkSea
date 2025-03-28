using System.Text.Json;
using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Models;
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
        {
            return XrpcErrorOr<object>.Fail(
                "NotFound",
                $"Did not find a handler for {nsid}",
                404);
        }
        
        var service = serviceProvider.GetService(implementationType.HandlerType);
        if (service is null)
        {
            return XrpcErrorOr<object>.Fail(
                "NoHandler",
                $"No handler was registered for {nsid}.",
                500);
        }

        var requestObject = await GetObjectForType(implementationType, context);
        if (requestObject.IsError)
        {
            return XrpcErrorOr<object>.Fail(
                "FailedDeserialization",
                requestObject.Error!);
        }

        if (requestObject.Value is null)
        {
            return XrpcErrorOr<object>.Fail(
                "FailedDeserialization",
                "An unknown error has occurred while deserializing the call parameters.");
        }

        var method = service.GetType()
            .GetMethod("Handle")!;
        
        var task = (Task)method.Invoke(service, [requestObject.Value])!;
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
    private static async Task<ErrorOr<object?>> GetObjectForType(
        XrpcTypeMapping typeMapping,
        HttpContext context)
    {
        try
        {
            // If this is a procedure, we just have to parse JSON. Nice!
            if (!typeMapping.IsQuery)
            {
                var json = await JsonSerializer.DeserializeAsync(
                    context.Request.Body,
                    typeMapping.RequestType);

                return ErrorOr<object?>.Ok(json);
            }
        
            // Dirty hack :3
            var queryDict = context
                .Request
                .Query
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            var queryJson = JsonSerializer.Serialize(queryDict);
            return ErrorOr<object?>.Ok(JsonSerializer.Deserialize(
                queryJson,
                typeMapping.RequestType,
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            }));
        }
        catch (JsonException e)
        {
            return ErrorOr<object?>.Fail($"Failed deserializing parameters for XRPC call: {e.Message}");
        }
    }
}