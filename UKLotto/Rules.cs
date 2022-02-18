namespace UKLotto;

internal static class Rules
{
    internal static int GlobalMatching(List<int> luckyDipNum, List<int> lottoNum, int lottoNumBonus)
    {
        if (BallMatchSix(luckyDipNum, lottoNum)) return 60;
        if (BallMatchFiveBonus(luckyDipNum, lottoNum, lottoNumBonus)) return 55;
        if (BallMatchFive(luckyDipNum, lottoNum)) return 50;
        if (BallMatchFour(luckyDipNum, lottoNum)) return 40;
        if (BallMatchThree(luckyDipNum, lottoNum)) return 30;
        if (BallMatchTwo(luckyDipNum, lottoNum)) return 20;

        return -1;
    }

    private static bool BallMatchTwo(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 2;
    }

    private static bool BallMatchThree(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 3;
    }

    private static bool BallMatchFour(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 4;
    }

    private static bool BallMatchFive(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 5;
    }

    private static bool BallMatchFiveBonus(List<int> luckyDipNum, List<int> lottoNum, int lottoNumBonus)
    {
        lottoNum.Add(lottoNumBonus);
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 6;
    }

    private static bool BallMatchSix(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        return matchCount == 6;
    }
}