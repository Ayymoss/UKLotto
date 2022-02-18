using System.Diagnostics;
using Serilog;

namespace UKLotto;

// TODO: 45m Rolls no winner trigger rollover - x4 - 5th shared with 5 Match + 1 Bonus

internal static class UkLotto
{
    private static void Main()
    {
        Logging.Enable();

        var stopwatch = new Stopwatch();

        //const int ticketSales = 45_000_000;
        const int ticketSales = 1_000_000;
        var spent = ticketSales * 2;
        var paid = 0;
        var profit = 0;

        var lottoNum = Generation.GenerateLotto();
        var lottoBonus = Generation.GenerateBonus();
        var ballMatchZero = 0;
        var ballMatchTwo = 0;
        var ballMatchThree = 0;
        var ballMatchFour = 0;
        var ballMatchFive = 0;
        var ballMatchFiveBonus = 0;
        var ballMatchSix = 0;
        
        Log.Debug($"Lotto Numbers: {string.Join("-", lottoNum)}");
        Log.Debug($"Lotto Bonus: {lottoBonus}");
        Log.Information($"Processing {ticketSales:N0} Games");
        Thread.Sleep(100);
        
        stopwatch.Restart();

        Parallel.For(1, ticketSales, new ParallelOptions {MaxDegreeOfParallelism = 200}, i =>
        {
            var genLuckyDip = new Generation().GenerateLuckyDip();
            switch (Rules.GlobalMatching(genLuckyDip, lottoNum, lottoBonus))
            {
                case 20:
                    Interlocked.Increment(ref ballMatchTwo);
                    break;
                case 30:
                    Interlocked.Increment(ref ballMatchThree);
                    break;
                case 40:
                    Interlocked.Increment(ref ballMatchFour);
                    break;
                case 50:
                    Interlocked.Increment(ref ballMatchFive);
                    break;
                case 55:
                    Interlocked.Increment(ref ballMatchFiveBonus);
                    break;
                case 60:
                    Interlocked.Increment(ref ballMatchSix);
                    Log.Information($"Jackpot Winner: {string.Join("-", genLuckyDip)} | TicketID: {i:N0}");
                    break;
                default:
                    Interlocked.Increment(ref ballMatchZero);
                    break;
            }
        });

        Console.WriteLine("\nLotto Numbers: \t\t{0}, Bonus Ball: {1}", string.Join("-", lottoNum), lottoBonus);
        Console.WriteLine("Lotto Ticket Sales: \t{0:N0}\n", ticketSales);
        Console.WriteLine("0  Matches: \t{0:N0}", ballMatchZero);
        Console.WriteLine("2  Matches: \t{0:N0}", ballMatchTwo);
        Console.WriteLine("3  Matches: \t{0:N0}", ballMatchThree);
        Console.WriteLine("4  Matches: \t{0:N0}", ballMatchFour);
        Console.WriteLine("5  Matches: \t{0:N0}", ballMatchFive);
        Console.WriteLine("5B Matches: \t{0:N0}", ballMatchFiveBonus);
        Console.WriteLine("6  Matches: \t{0:N0}\n", ballMatchSix);
        
        Log.Information($"Application Time: {stopwatch.ElapsedMilliseconds/1000f}s");
        stopwatch.Stop();
        
        Logging.Disable();
    }
}