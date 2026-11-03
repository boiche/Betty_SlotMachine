using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;

namespace Betty_SlotMachine.Validation
{
    public class DepositValidator(SlotGame game, decimal amount, Wallet wallet, TextWriter output) : IValidator
    {
        private const string INVALID_AMOUNT = "Invalid deposit amount. Amount should be at most {0:F2}$. Current balance is {1:F2}$";
        public bool Validate()
        {
            if (amount > game.Configuration.MaxDeposit)
            {
                output.WriteLine(string.Format(INVALID_AMOUNT, game.Configuration.MaxDeposit, wallet.Balance));
                return false;
            }

            return true;
        }
    }
}
