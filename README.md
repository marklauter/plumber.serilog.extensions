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
    where TRequest : class`  
```
