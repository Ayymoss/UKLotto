using Serilog;
namespace UKLotto;

public class Logging
{
    internal static void Enable()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/UKLotto.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        Log.Debug("[Logger] Enabled");
    }

    internal static void Disable()
    {
        Log.Debug("[Logger] Disabled");
        Log.CloseAndFlush();
    }
}