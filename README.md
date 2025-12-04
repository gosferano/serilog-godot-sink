# Serilog.Sinks.GodotConsole

[![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.GodotConsole.svg)](https://www.nuget.org/packages/Serilog.Sinks.GodotConsole)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.GodotConsole.svg)](https://www.nuget.org/packages/Serilog.Sinks.GodotConsole)
[![GitHub](https://img.shields.io/github/license/gosferano/serilog-godot-sink.svg)](https://github.com/gosferano/serilog-godot-sink/blob/main/LICENSE)

A Serilog sink that writes log events to the Godot console using `GD.PrintRich()` with proper line handling to prevent extra blank lines in stack traces.

## Features

- Customizable output templates
- Rich text formatting support via GD.PrintRich()
- Proper stack trace formatting without extra blank lines
- Simple integration with Serilog

## Installation

Install via NuGet:

```bash
dotnet add package Serilog.Sinks.GodotConsole
```

Or via Package Manager Console:

```powershell
Install-Package Serilog.Sinks.GodotConsole
```

## Usage

### Basic Setup

```csharp
using Serilog;
using Serilog.Sinks.GodotConsole;

Log.Logger = new LoggerConfiguration()
    .WriteTo.GodotConsole()
    .CreateLogger();

Log.Information("Hello, Godot!");
Log.Warning("This is a warning");
Log.Error("An error occurred");
```

### Custom Template

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.GodotConsole(
        outputTemplate: "[{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();
```

### With Additional Configuration

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.GodotConsole(
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
```

### With template selector

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.GodotConsole(templateSelector: logEvent => logEvent.Level switch
    {
        LogEventLevel.Verbose => "[color=#949494]{Timestamp:HH:mm:ss} [VRB] {Message}[/color]",
        LogEventLevel.Debug => "[color=#DADADA]{Timestamp:HH:mm:ss} [DBG] {Message}[/color]",
        LogEventLevel.Information => "[color=#AFD7AF]{Timestamp:HH:mm:ss} [INF] {Message}[/color]",
        LogEventLevel.Warning => "[color=#FFAF87]{Timestamp:HH:mm:ss} [WRN] {Message}[/color]",
        LogEventLevel.Error => "[color=#FF6B9D]{Timestamp:HH:mm:ss} [ERR] {Message}{Exception}[/color]",
        LogEventLevel.Fatal => "[color=#FF005F][b]{Timestamp:HH:mm:ss} [FTL] {Message}{Exception}[/b][/color]",
        _ => "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}"
    })
    .CreateLogger();
```

### With custom formatter

```csharp
using Serilog.Formatting.Display;
var formatter = new MessageTemplateTextFormatter(
    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
    null);
Log.Logger = new LoggerConfiguration()
    .WriteTo.GodotConsole(formatter: formatter)
    .CreateLogger();
```

## Output Examples

Example output:
```
2025-12-01 14:35:22.123 [INF] Application started
2025-12-01 14:35:23.456 [DBG] Player position updated
2025-12-01 14:35:24.789 [WRN] Low memory warning
2025-12-01 14:35:25.012 [ERR] Failed to load texture
System.IO.FileNotFoundException: Could not find file 'texture.png'
   at System.IO.File.OpenRead(String path)
   at MyGame.TextureLoader.Load(String path)
```

## Template Tokens

Common template tokens you can use:

- `{Timestamp:format}` - Event timestamp with custom format
- `{Level:format}` - Log level (use `:u3` for 3-letter uppercase)
- `{Message:lj}` - The log message (`:lj` for literal JSON-safe output)
- `{Exception}` - Exception details if present
- `{Properties}` - Additional properties
- `{NewLine}` - Platform-specific line break

## License

MIT License - see LICENSE file for details

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Links

- [GitHub Repository](https://github.com/gosferano/serilog-godot-sink)
- [NuGet Package](https://www.nuget.org/packages/Serilog.Sinks.GodotConsole)
- [Serilog Documentation](https://github.com/serilog/serilog/wiki)