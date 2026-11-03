using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;

namespace Betty_SlotMachine.Tests;

/// <summary>
/// Statistical tests of the real <see cref="BettyGame"/> with real randomness. They run many rounds
/// and use tolerances far wider than the sampling error (5+ standard deviations) to stay non-flaky.
/// Exact per-round behaviour is covered with a fixed random in <see cref="BettyGameRoundTests"/>.
/// </summary>
public class BettyGameProbabilityTests
{
    private const int Rounds = 300_000;
    private const decimal Bet = 10m;

    private record Result(int Loss, int Small, int Big, decimal MinSmall, decimal MaxSmall,
                          decimal MinBig, decimal MaxBig, decimal Paid);

    private static readonly Lazy<Result> Simulation = new(Simulate);

    private static Result Simulate()
    {
        var game = new BettyGame(TextWriter.Null);
        game.Configure();
        var wallet = new Wallet();
        wallet.Add(1_000_000_000m);

        int loss = 0, small = 0, big = 0;
        decimal minSmall = decimal.MaxValue, maxSmall = 0, minBig = decimal.MaxValue, maxBig = 0, paid = 0;

        for (int i = 0; i < Rounds; i++)
        {
            var win = game.Bet(Bet);
            var multiplier = win / Bet;
            paid += win;

            if (win == 0) loss++;
            else if (multiplier < 2) { small++; minSmall = Math.Min(minSmall, multiplier); maxSmall = Math.Max(maxSmall, multiplier); }
            else { big++; minBig = Math.Min(minBig, multiplier); maxBig = Math.Max(maxBig, multiplier); }
        }

        return new Result(loss, small, big, minSmall, maxSmall, minBig, maxBig, paid);
    }

    private static double Percent(int count) => count * 100.0 / Rounds;

    [Fact]
    public void About_50_percent_of_bets_lose()
        => Assert.InRange(Percent(Simulation.Value.Loss), 49.0, 51.0);

    [Fact]
    public void About_40_percent_of_bets_win_up_to_x2()
        => Assert.InRange(Percent(Simulation.Value.Small), 39.0, 41.0);

    [Fact]
    public void About_10_percent_of_bets_win_between_x2_and_x10()
        => Assert.InRange(Percent(Simulation.Value.Big), 9.5, 10.5);

    [Fact]
    public void Every_round_is_exactly_one_outcome()
    {
        var r = Simulation.Value;

        Assert.Equal(Rounds, r.Loss + r.Small + r.Big);
    }

    [Fact]
    public void Small_wins_pay_between_x0_1_and_x2()
    {
        var r = Simulation.Value;

        Assert.InRange(r.MinSmall, 0.1m, 2m);
        Assert.InRange(r.MaxSmall, 0.1m, 2m);
        Assert.True(r.MaxSmall <= 2m);
    }

    [Fact]
    public void Big_wins_pay_between_x2_and_x10()
    {
        var r = Simulation.Value;

        Assert.InRange(r.MinBig, 2m, 10m);
        Assert.InRange(r.MaxBig, 2m, 10m);
        Assert.True(r.MaxBig > 9m);       // the top of the range is actually reached
    }

    [Fact]
    public void Return_to_player_matches_the_configured_payouts()
    {
        // 0.4 * avg(0.1..2) + 0.1 * avg(2..10) = 0.42 + 0.6 = 1.02 (std. error of the sample is about 0.35%)
        var rtp = (double)(Simulation.Value.Paid / (Rounds * Bet)) * 100;

        Assert.InRange(rtp, 100.0, 104.0);
    }
}
