using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Tests;

/// <summary>End-to-end tests: one typed line through parsing, validation, execution and output.</summary>
public class CommandFlowTests
{
    public CommandFlowTests()
    {
        Player.Balance = 0;
        TestHelpers.EnsureGameLoaded();
    }

    [Fact]
    public void Player_starts_with_zero_balance() => Assert.Equal(0m, Player.Balance);

    [Fact]
    public void Deposit_increases_balance_and_shows_it()
    {
        var output = TestHelpers.Run("deposit 25.50");

        Assert.Equal(25.50m, Player.Balance);
        Assert.Contains("25.50$ deposited successfully. Your balance is 25.50$", output);
    }

    [Fact]
    public void Withdraw_decreases_balance_and_shows_it()
    {
        Player.Balance = 50;

        var output = TestHelpers.Run("withdraw 20");

        Assert.Equal(30m, Player.Balance);
        Assert.Contains("20.00$ withdrawn successfully. Your balance is 30.00$", output);
    }

    [Fact]
    public void Withdrawing_the_whole_balance_is_allowed()
    {
        Player.Balance = 10;

        TestHelpers.Run("withdraw 10");

        Assert.Equal(0m, Player.Balance);
    }

    [Fact]
    public void Withdraw_more_than_balance_is_rejected()
    {
        Player.Balance = 10;

        var output = TestHelpers.Run("withdraw 10.01");

        Assert.Equal(10m, Player.Balance);
        Assert.Contains("Insufficient funds", output);
    }

    [Theory]
    [InlineData("bet 0.99")]
    [InlineData("bet 10.01")]
    [InlineData("bet 50")]
    public void Bet_outside_1_to_10_is_rejected_and_balance_is_untouched(string input)
    {
        Player.Balance = 100;

        var output = TestHelpers.Run(input);

        Assert.Equal(100m, Player.Balance);
        Assert.Contains("between 1$ and 10$", output);
    }

    [Theory]
    [InlineData("bet 1", 1)]
    [InlineData("bet 10", 10)]
    public void Bet_at_the_limits_is_played(string input, int bet)
    {
        Player.Balance = 100;

        var output = TestHelpers.Run(input);

        Assert.DoesNotContain("between 1$ and 10$", output);
        Assert.Matches(@"(No luck this time|You won).*Your balance is", output);
        Assert.True(Player.Balance >= 100 - bet);           // a bet can only lose the stake
    }

    [Fact]
    public void Bet_larger_than_balance_is_rejected()
    {
        Player.Balance = 4;

        var output = TestHelpers.Run("bet 5");

        Assert.Equal(4m, Player.Balance);
        Assert.Contains("Insufficient funds", output);
    }

    [Fact]
    public void Invalid_input_does_not_repeat_the_previous_command()
    {
        TestHelpers.Run("deposit 5");

        TestHelpers.Run("foo");
        TestHelpers.Run("");
        TestHelpers.Run("bet");

        Assert.Equal(5m, Player.Balance);
    }

    [Fact]
    public void Printed_balance_matches_the_real_balance_after_every_bet()
    {
        Player.Balance = 1_000_000;

        for (int i = 0; i < 500; i++)
        {
            var output = TestHelpers.Run("bet 5");

            Assert.Contains($"Your balance is {Player.Balance:F2}$", output);
        }
    }

    [Fact]
    public void Exit_says_goodbye_and_terminates_the_game()
    {
        var output = TestHelpers.Run("exit");

        Assert.Contains("Thank you for playing", output);
        Assert.True(GameEnvironment.Terminated);
    }
}
