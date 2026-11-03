namespace Betty_SlotMachine.Tests;

/// <summary>End-to-end tests: one typed line through parsing, validation, execution and output.</summary>
public class CommandFlowTests
{
    private readonly TestSession _session = new();

    [Fact]
    public void Player_starts_with_zero_balance() => Assert.Equal(0m, _session.Balance);

    [Fact]
    public void Deposit_increases_balance_and_shows_it()
    {
        var output = _session.Run("deposit 25.50");

        Assert.Equal(25.50m, _session.Balance);
        Assert.Contains("25.50$ deposited successfully. Your balance is 25.50$", output);
    }

    [Fact]
    public void Withdraw_decreases_balance_and_shows_it()
    {
        _session.Fund(50);

        var output = _session.Run("withdraw 20");

        Assert.Equal(30m, _session.Balance);
        Assert.Contains("20.00$ withdrawn successfully. Your balance is 30.00$", output);
    }

    [Fact]
    public void Withdrawing_the_whole_balance_is_allowed()
    {
        _session.Fund(10);

        _session.Run("withdraw 10");

        Assert.Equal(0m, _session.Balance);
    }

    [Fact]
    public void Withdraw_more_than_balance_is_rejected()
    {
        _session.Fund(10);

        var output = _session.Run("withdraw 10.01");

        Assert.Equal(10m, _session.Balance);
        Assert.Contains("Insufficient funds", output);
    }

    [Theory]
    [InlineData("bet 0.99")]
    [InlineData("bet 10.01")]
    [InlineData("bet 50")]
    public void Bet_outside_1_to_10_is_rejected_and_balance_is_untouched(string input)
    {
        _session.Fund(100);

        var output = _session.Run(input);

        Assert.Equal(100m, _session.Balance);
        Assert.Contains("Amount should be between", output);
    }

    [Theory]
    [InlineData("bet 1", 1)]
    [InlineData("bet 10", 10)]
    public void Bet_at_the_limits_is_played(string input, int bet)
    {
        _session.Fund(100);

        var output = _session.Run(input);

        Assert.DoesNotContain("Amount should be between", output);
        Assert.Matches(@"(No luck this time|You won).*Your balance is", output);
        Assert.True(_session.Balance >= 100 - bet);           // a bet can only lose the stake
    }

    [Fact]
    public void Bet_larger_than_balance_is_rejected()
    {
        _session.Fund(4);

        var output = _session.Run("bet 5");

        Assert.Equal(4m, _session.Balance);
        Assert.Contains("Insufficient funds", output);
    }

    [Fact]
    public void Invalid_input_does_not_repeat_the_previous_command()
    {
        _session.Run("deposit 5");

        _session.Run("foo");
        _session.Run("");
        _session.Run("bet");

        Assert.Equal(5m, _session.Balance);
    }

    [Fact]
    public void Printed_balance_matches_the_real_balance_after_every_bet()
    {
        _session.Fund(100_000);

        for (int i = 0; i < 500; i++)
        {
            var output = _session.Run("bet 5");

            Assert.Contains($"Your balance is {_session.Balance:F2}$", output);
        }
    }

    [Fact]
    public void Exit_says_goodbye_and_terminates_the_game()
    {
        var output = _session.Run("exit");

        Assert.Contains("Thank you for playing", output);
        Assert.True(_session.Environment.Terminated);
    }

    [Fact]
    public void Sessions_do_not_share_a_balance()
    {
        var other = new TestSession();

        _session.Run("deposit 50");

        Assert.Equal(50m, _session.Balance);
        Assert.Equal(0m, other.Balance);
    }
}
