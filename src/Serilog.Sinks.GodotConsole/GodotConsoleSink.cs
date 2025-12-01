using Godot;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.GodotConsole;

public class GodotConsoleSink : ILogEventSink
{
    private readonly ITextFormatter _formatter;

    public GodotConsoleSink(ITextFormatter? formatter = null)
    {
        _formatter =
            formatter
            ?? new MessageTemplateTextFormatter(
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            );
    }

    public void Emit(LogEvent logEvent)
    {
        using var writer = new StringWriter();
        _formatter.Format(logEvent, writer);
        var message = writer.ToString().TrimEnd();

        // Split into lines and print each line separately
        // This prevents Godot console from adding extra blank lines
        var lines = message.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        foreach (var line in lines)
        {
            GD.PrintRich(line);
        }
    }
}