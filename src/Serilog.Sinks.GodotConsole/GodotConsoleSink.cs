using System.Collections.Concurrent;
using Godot;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.GodotConsole;

public class GodotConsoleSink : ILogEventSink
{
    private readonly ITextFormatter? _formatter;
    private readonly Func<LogEvent, string>? _templateSelector;
    private readonly IFormatProvider? _formatProvider;
    private readonly ConcurrentDictionary<string, MessageTemplateTextFormatter> _formatterCache;

    public GodotConsoleSink(
        ITextFormatter? formatter = null,
        Func<LogEvent, string>? templateSelector = null,
        IFormatProvider? formatProvider = null)
    {
        _formatter = formatter;
        _templateSelector = templateSelector;
        _formatProvider = formatProvider;
        _formatterCache = new ConcurrentDictionary<string, MessageTemplateTextFormatter>();

        // Default formatter if nothing provided
        if (_formatter == null && _templateSelector == null)
        {
            _formatter = new MessageTemplateTextFormatter(
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
    }

    public void Emit(LogEvent logEvent)
    {
        string message;

        if (_templateSelector != null)
        {
            // Use template selector - cache formatters by template string
            var template = _templateSelector(logEvent);
            var formatter = _formatterCache.GetOrAdd(
                template,
                t => new MessageTemplateTextFormatter(t, _formatProvider));
            
            using var writer = new StringWriter();
            formatter.Format(logEvent, writer);
            message = writer.ToString().TrimEnd();
        }
        else
        {
            // Use default formatter
            using var writer = new StringWriter();
            _formatter!.Format(logEvent, writer);
            message = writer.ToString().TrimEnd();
        }

        // Split into lines and print each line separately
        // This prevents Godot console from adding extra blank lines
        var lines = message.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        foreach (var line in lines)
        {
            GD.PrintRich(line);
        }
    }
}