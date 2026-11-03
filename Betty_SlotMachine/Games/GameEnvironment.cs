using Betty_SlotMachine.Models;
using Betty_SlotMachine.Validation;

namespace Betty_SlotMachine.Games
{
    /// <summary>
    /// Represents current game environment. Provides entry point to each action allowed
    /// </summary>
    /// <param name="game">The <see cref="SlotGame"/> being played</param>
    /// <param name="player">The <see cref="Models.Player"/> whose wallet is used</param>
    /// <param name="output">Where wallet messages are written</param>
    public class GameEnvironment(SlotGame game, Player player, TextWriter output)
    {
        private const string DEPOSIT_MESSAGE = "{0:F2}$ deposited successfully. Your balance is {1:F2}$";
        private const string WITHDRAW_MESSAGE = "{0:F2}$ withdrawn successfully. Your balance is {1:F2}$";
        private const string LOSE = "No luck this time. Your balance is {0:F2}$";
        private const string WIN = "You won {0:F2}$. Your balance is {1:F2}$";
        private const string GAME_ERROR = "Game ERROR. Please contact your vendor";

        /// <summary>
        /// Is game terminated
        /// </summary>
        public bool Terminated { get; private set; }

        /// <summary>
        /// Configures and boots the <see cref="SlotGame"/>
        /// </summary>
        public void Start()
        {
            if (!game.Configure())
            {
                output.WriteLine(GAME_ERROR);
                Terminated = true;
                return;
            }
            game.Boot();
        }

        /// <summary>
        /// Plays one round of the <see cref="SlotGame"/>
        /// </summary>
        /// <param name="bet">Amount to bet</param>
        public void Bet(decimal bet)
        {
            if (!new BettingValidator(game, bet, player.Wallet, output).Validate())
                return;

            decimal winnings;

            try
            {
                winnings = game.Bet(bet);
            }
            catch (Exception)
            {
                output.WriteLine(GAME_ERROR);
                return;
            }

            player.Wallet.Remove(bet);

            if (winnings > 0)
            {
                player.Wallet.Add(winnings);
                output.WriteLine(string.Format(WIN, winnings, player.Wallet.Balance));
            }
            else if (winnings == 0)
            {
                output.WriteLine(string.Format(LOSE, player.Wallet.Balance));
            }
        }

        /// <summary>
        /// Terminates the <see cref="SlotGame"/>
        /// </summary>
        public void Exit()
        {
            game.Exit();
            Terminated = true;
        }

        /// <summary>
        /// Deposits funds to the <see cref="Models.Player"/>'s wallet
        /// </summary>
        /// <param name="amount">Amount to deposit</param>
        public void Deposit(decimal amount)
        {
            if (!new DepositValidator(game, amount, player.Wallet, output).Validate())
                return;

            player.Wallet.Add(amount);
            output.WriteLine(string.Format(DEPOSIT_MESSAGE, amount, player.Wallet.Balance));
        }

        /// <summary>
        /// Withdraws funds from the <see cref="Models.Player"/>'s wallet
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        public void Withdraw(decimal amount)
        {
            if (!new WithdrawValidator(amount, player.Wallet, output).Validate())
                return;

            player.Wallet.Remove(amount);
            output.WriteLine(string.Format(WITHDRAW_MESSAGE, amount, player.Wallet.Balance));
        }
    }
}
