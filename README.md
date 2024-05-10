![plumber logo](https://raw.githubusercontent.com/marklauter/plumber/main/images/plumber.png)
# Serilog Extensions for Plumber: Pipelines for AWS Lambda
Plumber.Serilog.Extensions provides Serilog middleware extensions for the [Plumber pipeline library](https://github.com/marklauter/plumber).

## IRequestHandler Extension Methods
- `public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler)
    where TRequest : class`
- `public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler,
    Action<RequestLoggerOptions<TRequest, TResponse>> configureOptions)
    where TRequest : class`  

