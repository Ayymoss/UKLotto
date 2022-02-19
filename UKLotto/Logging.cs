using Serilog;
namespace UKLotto;

internal static class Logging
{
    internal static void Enable()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
        Log.Debug("[Logger] Enabled");
    }

    internal static void Disable()
    {
        Log.Debug("[Logger] Disabled");
        Log.CloseAndFlush();
    }
}