using System.Diagnostics;
using Serilog;

namespace UKLotto;

public class LottoProgram
{
    private readonly Stopwatch _timer = new();
    private List<int> _lottoNum = new();
    private int _lottoBonus;
    private int _payout;
    private int _ballMatchZero;
    private int _ballMatchTwo;
    private int _ballMatchThree;
    private int _ballMatchFour;
    private  int _ballMatchFive;
    private int _ballMatchFiveBonus;
    private int _ballMatchSix;
    private int _rollOverCount = 1;
    public const int TicketSales = 1_000_000;
    
    public void LottoProcessingInit()
    {
        Console.Clear();

        while (true)
        {
            Log.Information("Processing {Sales} Games", TicketSales);
            //Thread.Sleep(100);

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

        Log.Information($"Application Time: {_timer.ElapsedMilliseconds / 1000f}s");
        _timer.Stop();
    }

    private void LottoProcessing()
    {
        _lottoNum = new Generation().GenerateNumbers();
        _lottoBonus = Generation.GenerateBonus();
        _timer.Restart();
        Parallel.For(1, TicketSales, new ParallelOptions {MaxDegreeOfParallelism = 200}, i =>
        {
            var luckyDip = new Generation().GenerateNumbers();
            while (true)
            {
                switch (Rules.GlobalMatching(luckyDip, _lottoNum, _lottoBonus))
                {
                    case 20:
                        Interlocked.Increment(ref _ballMatchTwo);
                        luckyDip = new Generation().GenerateNumbers();
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

    public void CustomTicket()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("\nEnter 6 numbers separated by space. (n<60>0 & No Duplicates)");
            var cTicket = "Your Custom Ticket".PromptString();
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

            if (cTicketList.Count == cTicketList.Distinct().Count() && cTicketList.Count == 6)
            {
                TicketProcessing(cTicketList);
                break;
            }

            Console.WriteLine("Value(s) entered doesn't meet requirement...");
        }
    }

    public void TicketProcessing(List<int>? luckyDip = null)
    {
        luckyDip ??= new Generation().GenerateNumbers();
        Console.Clear();
        Console.WriteLine("Ticket Numbers: {0}", string.Join("-", luckyDip));
        var ticketMatchId = 0;
        var lottoList = new List<int>();

        Log.Information("Processing until {Count}; will exit sooner if found", TicketSales);

        //Thread.Sleep(100);
        _timer.Restart();

        Parallel.For(0, 100_000_000, new ParallelOptions {MaxDegreeOfParallelism = 100}, (i, state) =>
        {
            _lottoNum = new Generation().GenerateNumbers();
            switch (Rules.GlobalMatching(luckyDip, _lottoNum, 0))
            {
                case 60:
                    ticketMatchId = i;
                    lottoList = _lottoNum;
                    state.Stop();
                    break;
            }
        });

        Console.WriteLine("\nTicket Numbers: {0}", string.Join("-", luckyDip));
        Console.WriteLine("Lotto Numbers:  {0}", string.Join("-", lottoList));
        Console.WriteLine("Jackpot ID: \t{0:N0}", ticketMatchId);
        Console.WriteLine("The Jackpot ID is the amount of games it took to win with your numbers...\n");

        Log.Information($"Application Time: {_timer.ElapsedMilliseconds / 1000f}s");
        
        _timer.Stop();
    }
}
