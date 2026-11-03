using System.Globalization;
using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;
using Betty_SlotMachine.Validation;

namespace Betty_SlotMachine.Tests;

/// <summary>
/// The deposit limit from slotGameSettings.json: a single deposit may be at most MaxDeposit.
/// The limit is read from the loaded config, so these tests don't depend on its exact value.
/// </summary>
public class DepositValidatorTests
{
    private readonly BettyGame _game;
    private readonly decimal _maxDeposit;

    public DepositValidatorTests()
    {
        _game = new BettyGame(TextWriter.Null);
        _game.Configure();
        _maxDeposit = _game.Configuration.MaxDeposit;
    }

    private static string Amount(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    // ---- The validator on its own ----

    [Fact]
    public void Deposit_equal_to_the_limit_is_valid()
    {
        var output = new StringWriter();

        var valid = new DepositValidator(_game, _maxDeposit, new Wallet(), output).Validate();

        Assert.True(valid);
        Assert.Empty(output.ToString());
    }

    [Fact]
    public void Deposit_below_the_limit_is_valid()
    {
        var valid = new DepositValidator(_game, 0.01m, new Wallet(), TextWriter.Null).Validate();

        Assert.True(valid);
    }

    [Fact]
    public void Deposit_one_cent_over_the_limit_is_rejected_with_the_limit_and_balance()
    {
        var output = new StringWriter();
        var wallet = new Wallet();
        wallet.Add(12.34m);

        var valid = new DepositValidator(_game, _maxDeposit + 0.01m, wallet, output).Validate();

        Assert.False(valid);
        Assert.Contains($"Amount should be at most {_maxDeposit:F2}$", output.ToString());
        Assert.Contains($"Current balance is {12.34m:F2}$", output.ToString());
    }

    // ---- Through the game, as the player types it ----

    [Fact]
    public void Depositing_exactly_the_limit_succeeds()
    {
        var session = new TestSession();

        var output = session.Run($"deposit {Amount(_maxDeposit)}");

        Assert.Equal(_maxDeposit, session.Balance);
        Assert.Contains("deposited successfully", output);
    }

    [Fact]
    public void Depositing_over_the_limit_leaves_the_balance_untouched()
    {
        var session = new TestSession();
        session.Fund(50);

        var output = session.Run($"deposit {Amount(_maxDeposit + 0.01m)}");

        Assert.Equal(50m, session.Balance);
        Assert.Contains("Invalid deposit amount", output);
        Assert.DoesNotContain("deposited successfully", output);
    }

    [Fact]
    public void The_limit_applies_to_each_deposit_not_to_the_balance()
    {
        var session = new TestSession();

        session.Run($"deposit {Amount(_maxDeposit)}");
        var output = session.Run($"deposit {Amount(_maxDeposit)}");

        Assert.Equal(2 * _maxDeposit, session.Balance);
        Assert.Contains("deposited successfully", output);
    }

    [Fact]
    public void The_largest_possible_amount_is_rejected_instead_of_crashing()
    {
        var session = new TestSession();
        session.Run($"deposit {Amount(_maxDeposit)}");

        var output = session.Run($"deposit {Amount(decimal.MaxValue)}");

        Assert.Equal(_maxDeposit, session.Balance);
        Assert.Contains("Invalid deposit amount", output);
    }
}
