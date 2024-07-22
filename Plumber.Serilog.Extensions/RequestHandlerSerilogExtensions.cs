using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plumber.Serilog.Extensions;

/// <summary>
/// SerilogRequestHandlerExtensions provides extension methods for registering the Serilog middleware with the <see cref="IRequestHandler{TRequest, TResponse}"/>.
/// </summary>
public static class RequestHandlerSerilogExtensions
{
    /// <summary>
    /// UseSerilogRequestLogging registers the Serilog middleware with the <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="handler"><see cref="IRequestHandler{TRequest, TResponse}"/></param>
    /// <returns><see cref="IRequestHandler{TRequest, TResponse}"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
        this IRequestHandler<TRequest, TResponse> handler)
        where TRequest : class
    {
        ArgumentNullException.ThrowIfNull(handler);

        var options = GetOrInitializeOptions(handler);
        return options.MessageTemplate == null
            ? throw new ArgumentException($"{nameof(options.MessageTemplate)} cannot be null.")
            : handler.Use<RequestLoggerMiddleware<TRequest, TResponse>>(options);
    }

    /// <summary>
    /// UseSerilogRequestLogging registers the Serilog middleware with the <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// This overload allows you to configure the Serilog middleware.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="handler"><see cref="IRequestHandler{TRequest, TResponse}"/></param>
    /// <param name="configureOptions"><see cref="RequestLoggerOptions{TRequest, TResponse}"/></param>
    /// <returns><see cref="IRequestHandler{TRequest, TResponse}"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
        this IRequestHandler<TRequest, TResponse> handler,
        Action<RequestLoggerOptions<TRequest, TResponse>> configureOptions)
        where TRequest : class
    {
        ArgumentNullException.ThrowIfNull(handler);

        var options = GetOrInitializeOptions(handler);

        configureOptions.Invoke(options);
        return options.MessageTemplate == null
            ? throw new ArgumentException($"{nameof(options.MessageTemplate)} cannot be null.")
            : handler.Use<RequestLoggerMiddleware<TRequest, TResponse>>(options);
    }

    private static RequestLoggerOptions<TRequest, TResponse> GetOrInitializeOptions<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        where TRequest : class
    {
        return handler
            .Services
            .GetService<IOptions<RequestLoggerOptions<TRequest, TResponse>>>()?.Value
            ?? new RequestLoggerOptions<TRequest, TResponse>();
    }
}
