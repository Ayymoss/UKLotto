namespace UKLotto;

internal static class Rules
{
    internal static int GlobalMatching(List<int> luckyDipNum, List<int> lottoNum, int lottoNumBonus)
    {
        if (BallMatchSix(luckyDipNum, lottoNum)) return 60;
        return BallMatchFiveBonus(luckyDipNum, lottoNum, lottoNumBonus)
            ? 55
            : BallMatchTwoToFive(luckyDipNum, lottoNum);
    }

    private static int BallMatchTwoToFive(List<int> luckyDipNum, List<int> lottoNum)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Count(j => i == j));
        if (matchCount <= 1) return -1;
        return matchCount * 10;
    }

    private static bool BallMatchFiveBonus(List<int> luckyDipNum, List<int> lottoNum, int lottoNumBonus)
    {
        var matchCount = luckyDipNum.Sum(i => lottoNum.Append(lottoNumBonus).Count(j => i == j));
        return matchCount == 6;
    }

    private static bool BallMatchSix(List<int> luckyDipNum, List<int> lottoNum)
    {
        return luckyDipNum.OrderBy(e => e).SequenceEqual(lottoNum.OrderBy(e => e));
    }
}