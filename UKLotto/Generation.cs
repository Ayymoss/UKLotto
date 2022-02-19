using Serilog;

namespace UKLotto;

internal class Generation
{
    internal List<int> GenerateNumbers()
    {
        while (true)
        {
            var rand = new Random();
            var lottoList = new List<int>();
            var i = 0;
            while (i < 6)
            {
                lottoList.Add(rand.Next(1, 60));
                i++;
            }

            if (lottoList.Count == lottoList.Distinct().Count()) return lottoList;
            Log.Debug("Generation.GenerateLotto Duplicate");
        }
    }

    internal static int GenerateBonus()
    {
        var rand = new Random();
        var lottoBonus = rand.Next(1, 60);
        return lottoBonus;
    }
}