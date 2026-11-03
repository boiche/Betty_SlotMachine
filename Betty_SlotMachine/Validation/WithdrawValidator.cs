using Betty_SlotMachine.Models;

namespace Betty_SlotMachine.Validation
{
    /// <summary>
    /// Checks amounts of any withdraw whether player has enough funds to withdraw
    /// </summary>
    public class WithdrawValidator(decimal amount, Wallet wallet, TextWriter output) : IValidator
    {
        private const string INSUFFICIENT_FUNDS = "Insufficient funds. Please deposit. Current balance is {0:F2}$";

        public bool Validate()
        {
            if (amount > wallet.Balance)
            {
                output.WriteLine(string.Format(INSUFFICIENT_FUNDS, wallet.Balance));
                return false;
            }

            return true;
        }
    }
}
