# PinkSea.AtProto.Server

This is an additional library for PinkSea.AtProto adding a MediatR-like system for handling XRPC calls in ASP.NET Core.

# Usage

## Setup

When configuring your services, add

```cs
builder.Services.AddXrpcHandlers();
```

and after building the service collection

```cs
app.UseXrpcHandler();
```

This will automatically map the XRPC endpoint at `/xrpc`.

## Constructing an XRPC handler

There are two types of handlers, `IXrpcQuery<TRequest, TResponse>` that map to XRPC queries and `IXrpcProcedure<TRequest, TResponse>` that map to XRPC procedures.

The difference is that queries receive their parameters from the query string, while procedures get it from a JSON body.

To construct a handler, all you have to do is add a class deriving from either `IXrpcXYZ` interface, and add an `[Xrpc("namespace")]` attribute to it.

As an example:

```cs
using PinkSea.AtProto.Server.Xrpc;

[Xrpc("tld.domain.handlerName")]
public class MyQueryHandler : IXrpcQuery<Request, Response>
{
	public async Task<Response> Handle(Request request)
	{
		// Do some processing on request
		return new Response(/* return something in response */);
	}
}
```

Your new query will now be available at `<your url>/xrpc/tld.domain.handlerName`.

# Licensing

Unlike the rest of the PinkSea repository, which is EUPL licensed, PinkSea.AtProto.Server is released under the Creative Commons Zero license, effectively putting it in the public domain. Please build on top of it!