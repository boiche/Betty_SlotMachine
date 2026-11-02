using Betty_SlotMachine.Commands;

namespace Betty_SlotMachine.Validation
{
    /// <summary>
    /// Checks amounts of any bet whether player has sufficient funds to bet
    /// </summary>
    public class BettingValidator : IValidator
    {
        private const string NO_FUNDS = "Insufficient funds. Please deposit. Current balance is {0:F2}$";

        public bool Validate()
        {
            if (CommandContext.RecentCommand is not BetCommand command)
            {
                Console.WriteLine("Invalid command.");
                return false;
            }

            if (command.BetAmount < 1 || command.BetAmount > 10)
            {
                Console.WriteLine("Amount should be between 1$ and 10$");
                return false;
            }

            if (command.BetAmount > Player.Balance)
            {
                Console.WriteLine(string.Format(NO_FUNDS, Player.Balance));
                return false;
            }

            return true;
        }
    }
}
