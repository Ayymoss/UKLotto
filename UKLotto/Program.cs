using Serilog;

namespace UKLotto;


// TODO: 45m Rolls no winner trigger rollover - x4 - 5th shared with 5 Match + 1 Bonus


internal static class UkLotto
{
    private static void Main()
    {
        Logging.Enable();
        

        //var fLottoNum = $"{lottoNum[0]}-{lottoNum[1]}-{lottoNum[2]}-{lottoNum[3]}-{lottoNum[4]}-{lottoNum[5]}";
        //var fLottoNumBonus = $"{lottoNum[6]}";
        //var fLuckyDipNum = $"{luckyDipNum[0]}-{luckyDipNum[1]}-{luckyDipNum[2]}-{luckyDipNum[3]}-{luckyDipNum[4]}-{luckyDipNum[5]}";
        //Console.WriteLine($"Lottery Numbers: {fLottoNum}\nLottery Bonus Ball: {fLottoNumBonus}\n\nLucky Dip: {fLuckyDipNum}");

        //var testListOne = new List<int> {1, 2, 3, 40, 5, 6};
        //var testListTwo = new List<int> {1, 2, 3, 4, 5, 6};
        //var bonus = 7;
        //var testCase = Rules.GlobalMatching(testListOne, testListTwo, bonus);
        //Console.WriteLine(testCase);


        //var ticketPurchases = 45_000_000;
        const int ticketSales = 100;
        var spent = ticketSales * 2;
        var paid = 0;
        var profit = 0;
        
        var lottoNum = Generation.GenerateLotto();
        var lottoBonus = Generation.GenerateBonus();
        var ballMatchTwo = 0;
        var ballMatchThree = 0;
        var ballMatchFour = 0;
        var ballMatchFive = 0;
        var ballMatchFiveBonus = 0;
        var ballMatchSix = 0;
        
        Log.Debug($"Lotto Numbers: {string.Join("-", lottoNum)}");
        Log.Debug($"Lotto Bonus: {lottoBonus}");
        Log.Information($"Processing {ticketSales} Games, please wait");
        Thread.Sleep(5000);
        Parallel.For(1, ticketSales, i =>
        {
            switch (Rules.GlobalMatching(Generation.GenerateLuckyDip(), lottoNum, lottoBonus))
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
                    break;
                default:
                    Log.Error("This should never be hit");
                    break;
            }
        });

        Console.WriteLine("Lotto Numbers: {0}", string.Join("-", lottoNum));
        Console.WriteLine("Lotto Ticket Sales: {0}\n", ticketSales);
        Console.WriteLine("Two Matches: {0}", ballMatchTwo);
        Console.WriteLine("Three Matches: {0}", ballMatchThree);
        Console.WriteLine("Four Matches: {0}", ballMatchFour);
        Console.WriteLine("Five Matches: {0}", ballMatchFive);
        Console.WriteLine("Five+Bonus Matches: {0}", ballMatchFiveBonus);
        Console.WriteLine("Six Matches: {0}", ballMatchSix);
        
        Logging.Disable();
    }
}