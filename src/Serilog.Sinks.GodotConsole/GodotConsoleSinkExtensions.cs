using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.GodotConsole;

public static class GodotConsoleSinkExtensions
{
    /// <summary>
    /// Write log events to the Godot console using a single template for all log levels.
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration.</param>
    /// <param name="outputTemplate">A message template describing the format used to write to the sink.
    /// Supports Serilog template syntax: {Timestamp}, {Level}, {Message}, {Exception}, {Properties}, etc.</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null to use the current culture.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    /// <example>
    /// <code>
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole()  // Uses default template
    ///     .CreateLogger();
    /// 
    /// // Custom template
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole("{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    ///     .CreateLogger();
    /// </code>
    /// </example>
    public static LoggerConfiguration GodotConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        IFormatProvider? formatProvider = null)
    {
        // Create a MessageTemplateTextFormatter from the string template
        // This will be used to format all log events with the same template
        var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
        
        // Pass the formatter to the sink constructor
        // templateSelector and formatProvider will be null since we're using a static formatter
        return sinkConfiguration.Sink(new GodotConsoleSink(formatter: formatter));
    }

    /// <summary>
    /// Write log events to the Godot console using a template selector function.
    /// The function receives each LogEvent and returns a template string, allowing different
    /// templates for different log events (e.g., different colors per level, different formats
    /// based on properties). Templates are cached for performance.
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration.</param>
    /// <param name="templateSelector">Function that receives a LogEvent and returns a template string.
    /// The returned template supports Serilog template syntax: {Timestamp}, {Level}, {Message}, etc.
    /// Templates are cached by their string value, so returning the same template string will reuse
    /// the parsed formatter for better performance.</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null to use the current culture.
    /// This will be used when creating formatters from the template strings.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    /// <example>
    /// <code>
    /// // Different colors for different log levels using Godot BBCode
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole(templateSelector: logEvent => logEvent.Level switch
    ///     {
    ///         LogEventLevel.Verbose => "[color=#949494]{Timestamp:HH:mm:ss} [VRB] {Message}[/color]",
    ///         LogEventLevel.Debug => "[color=#DADADA]{Timestamp:HH:mm:ss} [DBG] {Message}[/color]",
    ///         LogEventLevel.Information => "[color=#AFD7AF]{Timestamp:HH:mm:ss} [INF] {Message}[/color]",
    ///         LogEventLevel.Warning => "[color=#FFAF87]{Timestamp:HH:mm:ss} [WRN] {Message}[/color]",
    ///         LogEventLevel.Error => "[color=#FF6B9D]{Timestamp:HH:mm:ss} [ERR] {Message}{Exception}[/color]",
    ///         LogEventLevel.Fatal => "[color=#FF005F][b]{Timestamp:HH:mm:ss} [FTL] {Message}{Exception}[/b][/color]",
    ///         _ => "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}"
    ///     })
    ///     .CreateLogger();
    /// 
    /// // Different templates based on log properties
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole(templateSelector: logEvent =>
    ///     {
    ///         if (logEvent.Properties.ContainsKey("SourceContext"))
    ///             return "{Timestamp:HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}";
    ///         return "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}";
    ///     })
    ///     .CreateLogger();
    /// </code>
    /// </example>
    public static LoggerConfiguration GodotConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        Func<LogEvent, string> templateSelector,
        IFormatProvider? formatProvider = null)
    {
        // Pass the template selector function and format provider to the sink constructor
        // The sink will call templateSelector for each log event to get the template,
        // then create/retrieve a cached MessageTemplateTextFormatter for that template
        // formatter will be null since we're using dynamic template selection
        return sinkConfiguration.Sink(new GodotConsoleSink(
            templateSelector: templateSelector,
            formatProvider: formatProvider));
    }

    /// <summary>
    /// Write log events to the Godot console using a custom formatter.
    /// Use this overload when you need complete control over formatting logic,
    /// such as conditional formatting, stateful formatting, or custom rendering.
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration.</param>
    /// <param name="formatter">A custom formatter implementing ITextFormatter to control the output format.
    /// The formatter's Format method will be called for each log event.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    /// <example>
    /// <code>
    /// // Using a custom formatter that colors output by level
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole(formatter: new ColoredLevelFormatter())
    ///     .CreateLogger();
    /// 
    /// // Using Serilog's built-in formatters
    /// Log.Logger = new LoggerConfiguration()
    ///     .WriteTo.GodotConsole(formatter: new CompactJsonFormatter())
    ///     .CreateLogger();
    /// </code>
    /// </example>
    public static LoggerConfiguration GodotConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        ITextFormatter formatter)
    {
        // Pass the user's custom formatter directly to the sink constructor
        // The formatter has complete control over how log events are rendered
        // templateSelector and formatProvider will be null
        return sinkConfiguration.Sink(new GodotConsoleSink(formatter: formatter));
    }
}