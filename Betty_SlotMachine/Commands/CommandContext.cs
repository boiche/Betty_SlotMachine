using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    /// <summary>
    /// Reads commands from the player and runs them against the <see cref="GameEnvironment"/>
    /// </summary>
    /// <param name="environment">Environment the commands are executed on</param>
    /// <param name="input">Where commands are read from</param>
    /// <param name="output">Where parsing errors are written</param>
    public class CommandContext(GameEnvironment environment, TextReader input, TextWriter output)
    {
        /// <summary>
        /// Contains the last typed command
        /// </summary>
        public Command? RecentCommand { get; private set; }

        /// <summary>
        /// Prompts user to input new command
        /// </summary>
        public void ReadCommand()
        {
            string line = input.ReadLine() ?? string.Empty;

            if (!CommandParser.TryParse(line, out var command, out string? errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    output.WriteLine(errorMessage);
            }

            RecentCommand = command;
        }

        /// <summary>
        /// Executes recent <see cref="Command"/>
        /// </summary>
        public void ExecuteCommand()
        {
            RecentCommand?.Execute(environment);
        }
    }
}
