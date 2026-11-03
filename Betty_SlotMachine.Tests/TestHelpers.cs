using Betty_SlotMachine.Commands;
using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;

namespace Betty_SlotMachine.Tests;

/// <summary>
/// One isolated game session: its own player, game, environment and in-memory console.
/// Lines are typed through a single <see cref="CommandContext"/>, just like the real game loop.
/// </summary>
internal sealed class TestSession
{
    private readonly Queue<string> _lines = new();
    private readonly CommandContext _commands;

    public StringWriter Output { get; } = new();
    public Player Player { get; } = new();
    public GameEnvironment Environment { get; }

    public TestSession(Random? random = null)
    {
        var game = new BettyGame(Output, random ?? new Random());
        Environment = new GameEnvironment(game, Player, Output);
        _commands = new CommandContext(Environment, new LineQueueReader(_lines), Output);

        Environment.Start();
        Output.GetStringBuilder().Clear();
    }

    public decimal Balance
    {
        get => Player.Wallet.Balance;
    }

    public void Fund(decimal amount) => Player.Wallet.Add(amount);

    /// <summary>Types one line, executes it and returns everything the game printed for it.</summary>
    public string Run(string input)
    {
        Output.GetStringBuilder().Clear();
        _lines.Enqueue(input);
        _commands.ReadCommand();
        _commands.ExecuteCommand();
        return Output.ToString();
    }

    private sealed class LineQueueReader(Queue<string> lines) : TextReader
    {
        public override string? ReadLine() => lines.TryDequeue(out var line) ? line : null;
    }
}

/// <summary><see cref="Random"/> that returns the given values in order, so a round's outcome is known in advance.</summary>
internal sealed class FixedRandom(params long[] values) : Random
{
    private int _next;

    public override long NextInt64(long minValue, long maxValue) => values[_next++];
}
