using Betty_SlotMachine.Commands;

namespace Betty_SlotMachine.Validation
{
    /// <summary>
    /// Checks amounts of any withdraw whether player has enough funds to withdraw
    /// </summary>
    public class WithdrawValidator : IValidator
    {
        public bool Validate()
        {
            if (CommandContext.RecentCommand is not WithdrawCommand command)
            {
                Console.WriteLine("Invalid command.");
                return false;
            }

            if (command.WithdrawAmount > Player.Balance)
            {
                Console.WriteLine($"Insufficient funds. Please deposit. Current balance is {Player.Balance:F2}$");
                return false;
            }

            return true;
        }
    }
}
