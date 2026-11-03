using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;

namespace Betty_SlotMachine.Validation
{
    /// <summary>
    /// Checks amounts of any bet whether player has sufficient funds to bet
    /// </summary>
    public class BettingValidator(SlotGame game, decimal bet, Wallet wallet, TextWriter output) : IValidator
    {
        private const string NO_FUNDS = "Insufficient funds. Please deposit. Current balance is {0:F2}$";
        private const string INVALID_BET = "Amount should be between {0:F2}$ and {1:F2}$";

        public bool Validate()
        {
            if (bet < game.Configuration.MinBet || bet > game.Configuration.MaxBet)
            {
                output.WriteLine(string.Format(INVALID_BET, game.Configuration.MinBet, game.Configuration.MaxBet));
                return false;
            }

            if (bet > wallet.Balance)
            {
                output.WriteLine(string.Format(NO_FUNDS, wallet.Balance));
                return false;
            }

            return true;
        }
    }
}
