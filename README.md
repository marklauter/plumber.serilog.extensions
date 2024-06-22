## Build Status
[![.NET Tests](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.tests.yml/badge.svg)](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.tests.yml)
[![.NET Publish](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.publish.yml/badge.svg?event=release)](https://github.com/marklauter/plumber.serilog.extensions/actions/workflows/dotnet.publish.yml)
[![Nuget](https://img.shields.io/badge/Nuget-v1.1.2-blue)](https://www.nuget.org/packages/MSL.Plumber.Serilog.Extensions/)
[![Nuget](https://img.shields.io/badge/.NET-6.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
[![Nuget](https://img.shields.io/badge/.NET-7.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
[![Nuget](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/)

## 
![Plumber Logo](https://raw.githubusercontent.com/marklauter/plumber/main/images/plumber.png "Plumber Logo")

# Plumber Serilog Extensions
## Plumber: Pipelines for host-free projects like AWS Lambda, Console, etc.

The `Plumber.Serilog.Extensions` package provides Serilog middleware extensions for the [Plumber pipeline library](https://github.com/marklauter/plumber).

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
