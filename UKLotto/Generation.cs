using Serilog;

namespace UKLotto;

internal class Generation
{
    internal List<int> GenerateNumbers()
    {
        while (true)
        {
            var rand = new Random();
            var numList = new List<int>();
            var i = 0;
            while (i < 6)
            {
                numList.Add(rand.Next(1, 60));
                i++;
            }

            if (numList.Count == numList.Distinct().Count()) return numList;
            Log.Debug("Generation Duplicate");
        }
    }

    internal static int GenerateBonus()
    {
        var rand = new Random();
        var lottoBonus = rand.Next(1, 60);
        return lottoBonus;
    }
}