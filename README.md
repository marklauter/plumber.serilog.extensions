![plumber logo](https://raw.githubusercontent.com/marklauter/plumber/main/images/plumber.png)

[![.NET Tests](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.tests.yml/badge.svg)](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.tests.yml)

# Serilog Extensions for Plumber: Pipelines for AWS Lambda
Plumber.Serilog.Extensions provides Serilog middleware extensions for the [Plumber pipeline library](https://github.com/marklauter/plumber).

## IRequestHandler Extension Methods
```csharp
public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler)
    where TRequest : class
```

```csharp
public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler,
    Action<RequestLoggerOptions<TRequest, TResponse>> configureOptions)
    where TRequest : class
```

## Sample Usage
```csharp
﻿using Microsoft.Extensions.DependencyInjection;
using Plumber;
using Plumber.Serilog.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

var request = "Hello, World!";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var handlerBuilder = RequestHandlerBuilder.New<string, string>();

_ = handlerBuilder.Services
    .AddSerilog()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

var handler = handlerBuilder.Build();

_ = handler
    .UseSerilogRequestLogging(options =>
    {
        options.LogLevel = LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, context) =>
        {
            diagnosticContext.Set(nameof(context.Request), context.Request);
            diagnosticContext.Set(nameof(context.Response), context.Response);
        };
    })
    .Use<ToLowerMiddleware>();

var response = await handler.InvokeAsync(request);
```
