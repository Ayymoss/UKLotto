using Serilog;

namespace UKLotto;

internal class Generation
{
    internal List<int> GenerateNumbers()
    {
        while (true)
        {
            var random = new Random();
            var lottoGeneration = new List<int>();
            var i = 0;
            while (i < 6)
            {
                lottoGeneration.Add(random.Next(1, 60));
                i++;
            }

            if (lottoGeneration.Count == lottoGeneration.Distinct().Count()) return lottoGeneration;
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