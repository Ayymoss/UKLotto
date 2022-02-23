using Serilog;

namespace UKLotto;

public class Lotto
{
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
    private static int _rollOverCount;

    public void LottoProcessingInit()
    {
        Console.Clear();

        while (true)
        {
            Log.Information("Processing {TSales:N0} Games", UkLotto.TicketSales);
            Thread.Sleep(100);

            LottoProcessing();

            if (_rollOverCount == 5)
            {
                Console.WriteLine("5th roll over!");
                Console.WriteLine("5+ Bonus Winners share £10m");
                break;
            }

            _rollOverCount += 1;
            if (_ballMatchSix != 0) break;
            Console.WriteLine("Current Roll Over: {0}", _rollOverCount);
            var userConfirm = "Do you wish to process Roll Over's? (y/n)".PromptString();

            if (userConfirm?.ToLower() == "y")
            {
                continue;
            }

            
            break;
        }

        Console.WriteLine("\nLotto Numbers: \t\t{0}, Bonus Ball: {1}", string.Join("-", _lottoNum), _lottoBonus);
        Console.WriteLine("Lotto Ticket Sales: \t{0:N0}\n", UkLotto.TicketSales * _rollOverCount);
        Console.WriteLine("0  Matches: \t{0:N0}", _ballMatchZero);
        Console.WriteLine("2  Matches: \t{0:N0}", _ballMatchTwo);
        Console.WriteLine("3  Matches: \t{0:N0}", _ballMatchThree);
        Console.WriteLine("4  Matches: \t{0:N0}", _ballMatchFour);
        Console.WriteLine("5  Matches: \t{0:N0}", _ballMatchFive);
        Console.WriteLine("5B Matches: \t{0:N0}", _ballMatchFiveBonus);
        Console.WriteLine("6  Matches: \t{0:N0}\n", _ballMatchSix);
        Console.WriteLine("Spent: \t\t{0:C0}", UkLotto.TicketSales * 2 * _rollOverCount);
        Console.WriteLine("Paid: \t\t{0:C0}", _payout);
        Console.WriteLine("Profit: \t{0:C0}", _payout - UkLotto.TicketSales * 2 * _rollOverCount);
        Console.Beep();
    }

    private static void LottoProcessing()
    {
        _lottoNum = new Generation().GenerateNumbers();
        _lottoBonus = Generation.GenerateBonus();
        Parallel.For(1, UkLotto.TicketSales, new ParallelOptions {MaxDegreeOfParallelism = 200}, i =>
        {
            _luckyDip = new Generation().GenerateNumbers();
            while (true)
            {
                switch ((LottoValue) Rules.GlobalMatching(_luckyDip, _lottoNum, _lottoBonus))
                {
                    case LottoValue.BallTwo:
                        Interlocked.Increment(ref _ballMatchTwo);
                        _luckyDip = new Generation().GenerateNumbers();
                        if (_rollOverCount == 4)
                        {
                            Interlocked.Add(ref _payout, 5);
                        }

                        continue;
                    case LottoValue.BallThree:
                        Interlocked.Increment(ref _ballMatchThree);
                        Interlocked.Add(ref _payout, 30);
                        break;
                    case LottoValue.BallFour:
                        Interlocked.Increment(ref _ballMatchFour);
                        Interlocked.Add(ref _payout, 140);
                        break;
                    case LottoValue.BallFive:
                        Interlocked.Increment(ref _ballMatchFive);
                        Interlocked.Add(ref _payout, 1_750);
                        break;
                    case LottoValue.BallFiveBonus:
                        Interlocked.Increment(ref _ballMatchFiveBonus);
                        Interlocked.Add(ref _payout, 1_000_000);
                        break;
                    case LottoValue.BallSix:
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
            var customTicket = "Your Custom Ticket".PromptString();
            var customTicketList = new List<int>();
            foreach (var i in customTicket?.Split(" ")!)
            {
                var isNumeric = int.TryParse(i, out var n);
                if (!isNumeric) continue;
                if (n is < 60 and > 0)
                {
                    customTicketList.Add(n);
                }
            }

            if (customTicketList.Count == customTicketList.Distinct().Count() && customTicketList.Count == 6)
            {
                TicketProcessing(customTicketList);
                break;
            }

            Console.WriteLine("Value(s) entered doesn't meet requirement...");
        }
    }

    public void TicketProcessing(List<int> luckyDip)
    {
        Console.Clear();
        Console.WriteLine("Ticket Numbers: {0}", string.Join("-", luckyDip));
        var ticketMatchId = 0;
        var lottoResult = new List<int>();

        Log.Information("Processing until 100,000,000; will exit sooner if found");

        Thread.Sleep(100);

        Parallel.For(0, 100_000_000, new ParallelOptions {MaxDegreeOfParallelism = 200}, (i, state) =>
        {
            var lottoNum = new Generation().GenerateNumbers();
            switch ((LottoValue) Rules.GlobalMatching(luckyDip, lottoNum, 0))
            {
                case LottoValue.BallSix:
                    ticketMatchId = i;
                    lottoResult = lottoNum;
                    state.Stop();
                    break;
            }
        });

        Console.WriteLine("\nTicket Numbers: {0}", string.Join("-", luckyDip));
        Console.WriteLine("Lotto Numbers:  {0}", string.Join("-", lottoResult));
        Console.WriteLine("Jackpot ID: \t{0:N0}", ticketMatchId);
        Console.WriteLine("[INFO] The Jackpot ID is the amount of games it took to win with your numbers\n");
        Console.Beep();
    }

    private enum LottoValue
    {
        BallTwo = 20,
        BallThree = 30,
        BallFour = 40,
        BallFive = 50,
        BallFiveBonus = 55,
        BallSix = 60
    }
}