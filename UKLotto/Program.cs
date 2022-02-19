using System.Diagnostics;
using Serilog;

namespace UKLotto;

internal static class UkLotto
{
    private static readonly Stopwatch Timer = new();
    private static List<int> _lottoNum = new();
    private static List<int> _luckyDip = new();
    private static int _lottoBonus;
    private static int _payout;
    private static int _ballMatchZero;
    private static int _ballMatchTwo;
    private static int _ballMatchThree;
    private static int _ballMatchFour;
    private static int _ballMatchFive;
    private static int _ballMatchFiveBonus;
    private static int _ballMatchSix;
    private static int _rollOverCount = 1;
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
                LottoProcessingInit();
                break;
            case 2:
                _luckyDip = new Generation().GenerateNumbers();
                TicketProcessing();
                break;
            case 3:
                CustomTicket();
                break;
        }

        Logging.Disable();
    }

    private static void LottoProcessingInit()
    {
        Console.Clear();

        while (true)
        {
            Log.Information($"Processing {TicketSales:N0} Games");
            Thread.Sleep(100);

            LottoProcessing();

            if (_rollOverCount == 5)
            {
                Console.WriteLine("5th roll over!");
                Console.WriteLine("5+ Bonus Winners share £10m");
                break;
            }

            if (_ballMatchSix != 0) break;
            
            _rollOverCount += 1;
            Console.WriteLine("Current Roll Over: {0}", _rollOverCount);
            var uConfirm = "Do you wish to process Roll Over's? (y/n)".PromptString();

            if (uConfirm.ToLower() == "y")
            {
                continue;
            }

            break;
        }

        Console.WriteLine("\nLotto Numbers: \t\t{0}, Bonus Ball: {1}", string.Join("-", _lottoNum), _lottoBonus);
        Console.WriteLine("Lotto Ticket Sales: \t{0:N0}\n", TicketSales * _rollOverCount);
        Console.WriteLine("0  Matches: \t{0:N0}", _ballMatchZero);
        Console.WriteLine("2  Matches: \t{0:N0}", _ballMatchTwo);
        Console.WriteLine("3  Matches: \t{0:N0}", _ballMatchThree);
        Console.WriteLine("4  Matches: \t{0:N0}", _ballMatchFour);
        Console.WriteLine("5  Matches: \t{0:N0}", _ballMatchFive);
        Console.WriteLine("5B Matches: \t{0:N0}", _ballMatchFiveBonus);
        Console.WriteLine("6  Matches: \t{0:N0}\n", _ballMatchSix);
        Console.WriteLine("Spent: \t\t{0:C0}", TicketSales * 2 * _rollOverCount);
        Console.WriteLine("Paid: \t\t{0:C0}", _payout);
        Console.WriteLine("Profit: \t{0:C0}", _payout - TicketSales * 2 * _rollOverCount);

        Log.Information($"Application Time: {Timer.ElapsedMilliseconds / 1000f}s");
        Timer.Stop();
    }

    private static void LottoProcessing()
    {
        _lottoNum = new Generation().GenerateNumbers();
        _lottoBonus = Generation.GenerateBonus();
        Timer.Restart();
        Parallel.For(1, TicketSales, new ParallelOptions {MaxDegreeOfParallelism = 200}, i =>
        {
            _luckyDip = new Generation().GenerateNumbers();
            while (true)
            {
                switch (Rules.GlobalMatching(_luckyDip, _lottoNum, _lottoBonus))
                {
                    case 20:
                        Interlocked.Increment(ref _ballMatchTwo);
                        _luckyDip = new Generation().GenerateNumbers();
                        if (_rollOverCount == 4)
                        {
                            Interlocked.Add(ref _payout, 5);
                        }

                        continue;
                    case 30:
                        Interlocked.Increment(ref _ballMatchThree);
                        Interlocked.Add(ref _payout, 30);
                        break;
                    case 40:
                        Interlocked.Increment(ref _ballMatchFour);
                        Interlocked.Add(ref _payout, 140);
                        break;
                    case 50:
                        Interlocked.Increment(ref _ballMatchFive);
                        Interlocked.Add(ref _payout, 1_750);
                        break;
                    case 55:
                        Interlocked.Increment(ref _ballMatchFiveBonus);
                        Interlocked.Add(ref _payout, 1_000_000);
                        break;
                    case 60:
                        Interlocked.Increment(ref _ballMatchSix);
                        Interlocked.Add(ref _payout, 10_000_000);
                        //Log.Information($"Jackpot Winner: {string.Join("-", genLuckyDip)} | Ticket ID: {i:N0}");
                        break;
                    default:
                        Interlocked.Increment(ref _ballMatchZero);
                        break;
                }

                break;
            }
        });
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
                _luckyDip = cTicketList;
                TicketProcessing();
                break;
            }

            Console.WriteLine("Value(s) entered doesn't meet requirement...");
        }
    }

    private static void TicketProcessing()
    {
        Console.Clear();
        Console.WriteLine("Ticket Numbers: {0}", string.Join("-", _luckyDip));
        var ticketMatchId = 0;
        var lottoList = new List<int>();

        Log.Information("Processing until 100,000,000; will exit sooner if found");

        Thread.Sleep(100);
        Timer.Restart();

        Parallel.For(0, 100_000_000, new ParallelOptions {MaxDegreeOfParallelism = 200}, (i, state) =>
        {
            _lottoNum = new Generation().GenerateNumbers();
            switch (Rules.GlobalMatching(_luckyDip, _lottoNum, 0))
            {
                case 60:
                    ticketMatchId = i;
                    lottoList = _lottoNum;
                    state.Stop();
                    break;
            }
        });

        Console.WriteLine("\nTicket Numbers: {0}", string.Join("-", _luckyDip));
        Console.WriteLine("Lotto Numbers:  {0}", string.Join("-", lottoList));
        Console.WriteLine("Jackpot ID: \t{0:N0}", ticketMatchId);
        Console.WriteLine("The Jackpot ID is the amount of games it took to win with your numbers...\n");

        Log.Information($"Application Time: {Timer.ElapsedMilliseconds / 1000f}s");
        Timer.Stop();
    }
}