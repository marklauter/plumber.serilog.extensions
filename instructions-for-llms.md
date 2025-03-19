# Plumber.Serilog.Extensions Reference

## Definition

Plumber.Serilog.Extensions: A .NET library that provides Serilog middleware extensions for the Plumber pipeline library. Target Framework: .NET 8.0.

## Plumber Core Concepts

### Components

- **RequestHandlerBuilder<TRequest, TResponse>**: Factory for creating pipeline handlers.
- **IRequestHandler<TRequest, TResponse>**: Pipeline processor for requests.
- **RequestContext<TRequest, TResponse>**: Container for request/response data and contextual information.
- **RequestMiddleware<TRequest, TResponse>**: Middleware delegate for request processing.

### Implementation Pattern

```csharp
// 1. Create a builder
var builder = RequestHandlerBuilder.Create<TRequest, TResponse>();

// 2. Configure services
builder.Services.AddScoped<IMyService, MyService>();

// 3. Build the handler and add middleware
var handler = builder.Build()
    .Use<ValidationMiddleware>()
    .Use<ProcessingMiddleware>();

// 4. Invoke the pipeline
var response = await handler.InvokeAsync(request);
```

## Serilog Extensions API

### RequestLoggerOptions<TRequest, TResponse>

- **EnrichDiagnosticContext**: Action<IDiagnosticContext, RequestContext<TRequest, TResponse>>?
  - Action to add data to diagnostic context per request
  - Default: null

- **Logger**: ILogger?
  - Custom logger instance
  - Default: null

- **LogLevel**: LogEventLevel
  - Log event level
  - Default: Information

- **MessageTemplate**: string
  - Template for log messages
  - Default: "Request {RequestId} completed in {Elapsed:0.0000} ms"

- **GetMessageTemplateProperties**: Func<RequestContext<TRequest, TResponse>, IEnumerable<LogEventProperty>>
  - Function returning properties for message template
  - Default: Implementation that returns RequestId and Elapsed properties

- **ThrowOnException**: bool
  - Whether to rethrow exceptions from downstream middleware
  - Default: true

### Extension Methods

```csharp
// Basic registration
public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler)
    where TRequest : class

// With options configuration
public static IRequestHandler<TRequest, TResponse> UseSerilogRequestLogging<TRequest, TResponse>(
    this IRequestHandler<TRequest, TResponse> handler,
    Action<RequestLoggerOptions<TRequest, TResponse>> configureOptions)
    where TRequest : class
```

## Implementation Examples

### Basic Setup

```csharp
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Register services
handlerBuilder.Services
    .AddSerilog()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

// Register middleware
var handler = handlerBuilder.Build()
    .UseSerilogRequestLogging()
    .Use<BusinessLogicMiddleware>();
```

### Custom Configuration

```csharp
handler.UseSerilogRequestLogging(options =>
{
    options.LogLevel = LogEventLevel.Debug;
    options.EnrichDiagnosticContext = (diagnosticContext, context) =>
    {
        diagnosticContext.Set("UserId", context.Request.UserId);
        diagnosticContext.Set("OperationName", context.Request.Operation);
    };
    options.MessageTemplate = "Request {RequestId} [{OperationName}] completed in {Elapsed:0.0000} ms";
    options.ThrowOnException = false;
});
```

### Error Handling Pattern

```csharp
handler
    .UseSerilogRequestLogging(options =>
    {
        options.LogLevel = LogEventLevel.Error;
        options.ThrowOnException = false;
    })
    .Use(async (context, next) =>
    {
        throw new Exception("Something went wrong"); // logged by request logging middleware
    });
```

### Multiple Context Loggers

```csharp
handler
    .UseSerilogRequestLogging()
    .Use(async (context, next) =>
    {
        await next(context);
        
        // Additional context-specific logging
        if (context.Request is SensitiveOperation)
        {
            Log.ForContext<SensitiveOperation>()
               .Warning("Sensitive operation {Id} executed", context.Id);
        }
    });
```

## Best Practices

### Service Registration

- Register Serilog with DI container:
  ```csharp
  handlerBuilder.Services
      .AddSerilog()
      .AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
  ```

- Add middleware at beginning of pipeline to log all downstream operations

### Logging Patterns

- **Sensitive Data**: Never log PII, passwords, or security tokens
  ```csharp
  diagnosticContext.Set("RequestType", context.Request.GetType().Name); // Safe
  // diagnosticContext.Set("Password", context.Request.Password); // Unsafe!
  ```

- **Performance**: Check log level before expensive operations
  ```csharp
  if (Log.IsEnabled(LogEventLevel.Debug))
  {
      var serializedData = JsonSerializer.Serialize(complexObject);
      Log.Debug("Complex data: {Data}", serializedData);
  }
  ```

- **Message Templates**: Use semantic parameter names with consistent templates
  ```csharp
  "Request {RequestId} for {Operation} completed in {ElapsedMs}ms"
  ```

- **Context**: Include sufficient context for effective diagnostics
  ```csharp
  diagnosticContext.Set("CorrelationId", context.Request.CorrelationId);
  diagnosticContext.Set("RequestTimestamp", DateTimeOffset.UtcNow);
  ```

## Common Errors

- **Middleware Order**: Serilog middleware must be registered before middleware it should log

- **Configuration**: MessageTemplate cannot be null (throws ArgumentException)

- **Property Names**: GetMessageTemplateProperties must return properties matching MessageTemplate placeholders

- **Service Registration**: Both AddSerilog() and AddLogging() with AddSerilog() are required
