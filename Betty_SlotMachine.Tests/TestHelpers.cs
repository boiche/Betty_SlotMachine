using Betty_SlotMachine.Commands;
using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Tests;

internal static class TestHelpers
{
    private static bool _gameLoaded;

    /// <summary>Types one line into the console, executes it and returns everything the game printed.</summary>
    public static string Run(string input)
    {
        var oldIn = Console.In;
        var oldOut = Console.Out;
        var output = new StringWriter();
        Console.SetIn(new StringReader(input));
        Console.SetOut(output);
        try
        {
            CommandContext.ReadCommand();
            CommandContext.ExecuteCommand();
        }
        finally
        {
            Console.SetIn(oldIn);
            Console.SetOut(oldOut);
        }
        return output.ToString();
    }

    /// <summary>Loads <see cref="BettyGame"/> into <see cref="GameEnvironment"/> once, silencing its welcome message.</summary>
    public static void EnsureGameLoaded()
    {
        if (_gameLoaded)
            return;

        var oldOut = Console.Out;
        Console.SetOut(TextWriter.Null);
        try { GameEnvironment.LoadGame(new BettyGame()); }
        finally { Console.SetOut(oldOut); }
        _gameLoaded = true;
    }
}
