namespace Betty_SlotMachine.Commands
{
    public static class CommandContext
    {
        private static Command? _recentCommand;

        /// <summary>
        /// Contains the last typed command
        /// </summary>
        public static Command? RecentCommand { get => _recentCommand; }

        static CommandContext()
        {
            _recentCommand = null;
        }

        /// <summary>
        /// Prompts user to input new command
        /// </summary>
        public static void ReadCommand()
        {
            string input = Console.ReadLine() ?? string.Empty;

            if (!CommandParser.TryParse(input, out _recentCommand, out string? errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    Console.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// Executes recent <see cref="Command"/>
        /// </summary>
        public static void ExecuteCommand()
        {
            if (_recentCommand == null)
                return;

            _recentCommand.Execute();
        }
    }
}
