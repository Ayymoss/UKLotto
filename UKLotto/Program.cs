using System.Diagnostics;
using Serilog;

namespace UKLotto;

// TODO: 45m Rolls no winner trigger rollover - x4 - 5th shared with 5 Match + 1 Bonus
// TODO: Implement Rollovers

internal static class UkLotto
{
    private static readonly Stopwatch Timer = new();
    private const int TicketSales = 1_000_000;

    private static void Main()
    {
        Logging.Enable();

        Console.WriteLine("1. Generate {0:N0} Tickets\n2. Find Games for 1 Ticket\n3. Find Games for 1 Custom Ticket",
            TicketSales);
        var uInput = "Value".PromptInt(minValue: 1, maxValue: 3);
        switch (uInput)
        {
            case 1:
                LottoProcessing();
                break;
            case 2:
                TicketProcessing(new Generation().GenerateNumbers());
                break;
            case 3:
                CustomTicket();
                break;
        }

        Logging.Disable();
    }

    private static void LottoProcessing()
    {
        Console.Clear();
        const int spent = TicketSales * 2;
        var payout = 0;

        var lottoNum = new Generation().GenerateNumbers();
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
        Log.Information($"Processing {TicketSales:N0} Games");
        Thread.Sleep(100);

        Timer.Restart();

        Parallel.For(1, TicketSales, new ParallelOptions {MaxDegreeOfParallelism = 200}, i =>
        {
            var genLuckyDip = new Generation().GenerateNumbers();
            switch (Rules.GlobalMatching(genLuckyDip, lottoNum, lottoBonus))
            {
                case 20:
                    Interlocked.Increment(ref ballMatchTwo);
                    // TODO: Figure out how to run the loop once for each match here...
                    break;
                case 30:
                    Interlocked.Increment(ref ballMatchThree);
                    Interlocked.Add(ref payout, 30);
                    break;
                case 40:
                    Interlocked.Increment(ref ballMatchFour);
                    Interlocked.Add(ref payout, 140);
                    break;
                case 50:
                    Interlocked.Increment(ref ballMatchFive);
                    Interlocked.Add(ref payout, 1_750);
                    break;
                case 55:
                    Interlocked.Increment(ref ballMatchFiveBonus);
                    Interlocked.Add(ref payout, 1_000_000);
                    break;
                case 60:
                    Interlocked.Increment(ref ballMatchSix);
                    //Log.Information($"Jackpot Winner: {string.Join("-", genLuckyDip)} | Ticket ID: {i:N0}");
                    break;
                default:
                    Interlocked.Increment(ref ballMatchZero);
                    break;
            }
        });

        Console.WriteLine("\nLotto Numbers: \t\t{0}, Bonus Ball: {1}", string.Join("-", lottoNum), lottoBonus);
        Console.WriteLine("Lotto Ticket Sales: \t{0:N0}\n", TicketSales);
        Console.WriteLine("0  Matches: \t{0:N0}", ballMatchZero);
        Console.WriteLine("2  Matches: \t{0:N0}", ballMatchTwo);
        Console.WriteLine("3  Matches: \t{0:N0}", ballMatchThree);
        Console.WriteLine("4  Matches: \t{0:N0}", ballMatchFour);
        Console.WriteLine("5  Matches: \t{0:N0}", ballMatchFive);
        Console.WriteLine("5B Matches: \t{0:N0}", ballMatchFiveBonus);
        Console.WriteLine("6  Matches: \t{0:N0}\n", ballMatchSix);
        Console.WriteLine("Spent: \t\t{0:C}", spent);
        Console.WriteLine("Paid: \t\t{0:C}", payout);
        Console.WriteLine("Profit: \t{0:C}", payout - spent);

        Log.Information($"Application Time: {Timer.ElapsedMilliseconds / 1000f}s");
        Timer.Stop();
    }

    private static void CustomTicket()
    {
        Console.Clear();
        while (true)
        {
            var cTicket = "Enter 6 (n<60>0) numbers separated by a space".PromptString();
            var cTicketList = new List<int>();
            foreach (var i in cTicket.Split(" "))
            {
                var isNumeric = int.TryParse(i, out var n);
                if (!isNumeric) continue;
                if (n is < 60 and > 0)
                {
                    cTicketList.Add(n);
                }
            }

            if (cTicketList.Count == 6)
            {
                TicketProcessing(cTicketList);
                break;
            }

            Console.WriteLine("Value(s) entered doesn't meet requirement...");
        }
    }

    private static void TicketProcessing(List<int> ticket)
    {
        Console.Clear();
        Console.WriteLine("\nTicket Numbers: {0}", string.Join("-", ticket));
        var ticketMatchId = 0;
        var lottoList = new List<int>();
        
        Log.Information("Processing until 100,000,000; will exit sooner if found");

        Thread.Sleep(100);
        Timer.Restart();

        Parallel.For(0, 100_000_000, new ParallelOptions {MaxDegreeOfParallelism = 200}, (i, state) =>
        {
            var genLottoNum = new Generation().GenerateNumbers();
            switch (Rules.GlobalMatching(ticket, genLottoNum, 0))
            {
                case 60:
                    ticketMatchId = i;
                    lottoList = genLottoNum;
                    state.Stop();
                    break;
            }
        });

        Console.WriteLine("\nTicket Numbers: {0}", string.Join("-", ticket));
        Console.WriteLine("Lotto Numbers:  {0}", string.Join("-", lottoList));
        Console.WriteLine("Jackpot ID: \t{0:N0}", ticketMatchId);
        Console.WriteLine("The Jackpot ID is the amount of games it took to win with your numbers...\n");

        Log.Information($"Application Time: {Timer.ElapsedMilliseconds / 1000f}s");
        Timer.Stop();
    }
}