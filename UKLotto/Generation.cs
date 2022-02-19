using Serilog;

namespace UKLotto;

internal class Generation
{
    internal static List<int> GenerateLotto()
    {
        var rand = new Random();
        var lottoList = new List<int>();
        var i = 0;
        while (i < 6)
        {
            lottoList.Add(rand.Next(1, 60));
            i++;
        }

        if (lottoList.Count != lottoList.Distinct().Count())
        {
            Log.Debug("Generation.GenerateLotto Duplicate");
            GenerateLotto();
        }
        
        return lottoList;
    }

    internal List<int> GenerateLuckyDip()
    {
        while (true)
        {
            var rand = new Random();
            var luckyDip = new List<int>();
            var i = 0;
            while (i < 6)
            {
                luckyDip.Add(rand.Next(1, 60));
                i++;
            }

            if (luckyDip.Count != luckyDip.Distinct().Count())
            {
                Log.Debug("Generation.GenerateLuckyDip Duplicate");
                continue;
            }

            return luckyDip;
        }
    }

    internal static int GenerateBonus()
    {
        var rand = new Random();
        var lottoBonus = rand.Next(1, 60);
        return lottoBonus;
    }
}