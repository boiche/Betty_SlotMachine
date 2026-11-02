using System.Globalization;

namespace Betty_SlotMachine.Commands
{
    internal static class CommandParser
    {
        private static readonly Dictionary<string, CommandType> Names =
            Enum.GetValues<CommandType>().ToDictionary(t => t.ToString(), t => t, StringComparer.OrdinalIgnoreCase);

        internal static bool TryParse(string input, out Command? command, out string? error)
        {
            command = null;
            error = null;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            if (!Names.TryGetValue(parts[0], out var type))
            {
                error = "Invalid command";
                return false;
            }

            if (type == CommandType.Exit)
            {
                if (parts.Length != 1)
                {
                    error = $"No command '{type}' takes {parts.Length - 1} parameters";
                    return false;
                }
                command = new ExitCommand();
                return true;
            }

            if (parts.Length != 2)
            {
                error = $"'{type}' expects 1 parameter, got {parts.Length - 1}";
                return false;
            }
            if (!TryParseAmount(parts[1], out var amount))
            {
                error = "Invalid param value";
                return false;
            }

            command = type switch
            {
                CommandType.Bet => new BetCommand(amount),
                CommandType.Deposit => new DepositCommand(amount),
                _ => new WithdrawCommand(amount)
            };
            return true;
        }

        private static bool TryParseAmount(string text, out decimal amount)
        {
            return decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount)
            && amount > 0
            && amount == Math.Round(amount, 2);
        }
    }
}