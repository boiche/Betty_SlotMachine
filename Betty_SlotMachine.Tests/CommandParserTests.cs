using Betty_SlotMachine.Commands;

namespace Betty_SlotMachine.Tests;

public class CommandParserTests
{
    [Theory]
    [InlineData("bet 5", typeof(BetCommand))]
    [InlineData("BET 5.50", typeof(BetCommand))]
    [InlineData("deposit 100", typeof(DepositCommand))]
    [InlineData("  Deposit    10  ", typeof(DepositCommand))]
    [InlineData("withdraw 0.01", typeof(WithdrawCommand))]
    [InlineData("exit", typeof(ExitCommand))]
    [InlineData("EXIT", typeof(ExitCommand))]
    public void Valid_input_creates_the_matching_command(string input, Type expected)
    {
        var ok = CommandParser.TryParse(input, out var command, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.IsType(expected, command);
    }

    [Theory]
    [InlineData("bet 5", "5")]
    [InlineData("bet 5.5", "5.5")]
    [InlineData("deposit 0.01", "0.01")]
    public void Amount_is_parsed_with_invariant_culture(string input, string expected)
    {
        CommandParser.TryParse(input, out var command, out _);

        var amount = command switch
        {
            BetCommand b => b.BetAmount,
            DepositCommand d => d.DepositAmount,
            _ => throw new InvalidOperationException()
        };
        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture), amount);
    }

    [Theory]
    [InlineData("foo", "Invalid command")]
    [InlineData("1", "Invalid command")]
    [InlineData("99", "Invalid command")]
    [InlineData("none", "Invalid command")]
    [InlineData("bet", "expects 1 parameter")]
    [InlineData("bet 1 2", "expects 1 parameter")]
    [InlineData("exit 1", "takes 1 parameters")]
    [InlineData("bet abc", "Invalid param value")]
    [InlineData("bet 0", "Invalid param value")]
    [InlineData("bet -5", "Invalid param value")]
    [InlineData("bet 1.005", "Invalid param value")]
    [InlineData("withdraw 0.001", "Invalid param value")]
    [InlineData("bet 5,5", "Invalid param value")]
    [InlineData("bet 1e1", "Invalid param value")]
    public void Invalid_input_is_rejected_with_a_message(string input, string expectedMessage)
    {
        var ok = CommandParser.TryParse(input, out var command, out var error);

        Assert.False(ok);
        Assert.Null(command);
        Assert.Contains(expectedMessage, error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_input_is_rejected_silently(string input)
    {
        var ok = CommandParser.TryParse(input, out var command, out var error);

        Assert.False(ok);
        Assert.Null(command);
        Assert.Null(error);
    }
}
