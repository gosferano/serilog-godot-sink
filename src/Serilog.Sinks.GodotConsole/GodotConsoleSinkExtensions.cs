using Serilog.Configuration;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.GodotConsole;

public static class GodotConsoleSinkExtensions
{
    public static LoggerConfiguration GodotConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        IFormatProvider? formatProvider = null
    )
    {
        var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
        return sinkConfiguration.Sink(new GodotConsoleSink(formatter));
    }
}