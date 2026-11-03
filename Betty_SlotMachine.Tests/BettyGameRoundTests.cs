namespace Betty_SlotMachine.Tests;

/// <summary>
/// Exact single-round results with a fixed random. With the default config the outcome roll (0..99) maps to
/// loss 0-49, small win 50-89 (x0.1..x2) and big win 90-99 (x2..x10); the second value (0..1_000_000) picks the multiplier.
/// </summary>
public class BettyGameRoundTests
{
    [Theory]
    [InlineData(0, 0, 5, "No luck this time. Your balance is 95.00$")]                 // loss: stake is lost
    [InlineData(49, 999_999, 5, "No luck this time. Your balance is 95.00$")]          // last loss slot
    [InlineData(50, 0, 10, "You won 1.00$. Your balance is 91.00$")]                    // small win at x0.1
    [InlineData(60, 500_000, 10, "You won 10.50$. Your balance is 100.50$")]         // small win at x1.05
    [InlineData(90, 0, 10, "You won 20.00$. Your balance is 110.00$")]                 // big win at x2
    [InlineData(99, 999_999, 10, "You won 99.99$. Your balance is 189.99$")]        // big win just under x10, rounded down
    [InlineData(99, 1_000_000, 10, "You won 100.00$. Your balance is 190.00$")]        // big win at x10
    public void Round_pays_exactly_what_the_rolls_say(long outcomeRoll, long multiplierRoll, int bet, string expectedMessage)
    {
        var session = new TestSession(new FixedRandom(outcomeRoll, multiplierRoll));
        session.Fund(100);

        var output = session.Run($"bet {bet}");

        Assert.Contains(expectedMessage, output);
    }
}
