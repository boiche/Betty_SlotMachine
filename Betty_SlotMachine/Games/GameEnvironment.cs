using Betty_SlotMachine.Validation;

namespace Betty_SlotMachine.Games
{
    /// <summary>
    /// Represents current game environment. Provides entry point to each action allowed
    /// </summary>
    public static class GameEnvironment
    {
        private const string DEPOSIT_MESSAGE = "{0:F2}$ deposited successfully. Your balance is {1:F2}$";
        private const string WITHDRAW_MESSAGE = "{0:F2}$ withdrawn successfully. Your balance is {1:F2}$";

        private static SlotGame? _loadedSlotGame;
        /// <summary>
        /// Is game terminated
        /// </summary>
        public static bool Terminated { get; private set; }

        /// <summary>
        /// Loads desired <see cref="SlotGame"/> for further actions over it
        /// </summary>
        /// <param name="game"></param>
        public static void LoadGame(SlotGame game)
        {
            _loadedSlotGame = game;
            game.Configure();
            game.Boot();
        }

        /// <summary>
        /// Starts the <see cref="SlotGame"/>
        /// </summary>
        /// <param name="bet">Amount to bet</param>
        public static void Bet(decimal bet)
        {
            if (!new BettingValidator().Validate())
                return;

            _loadedSlotGame?.Bet(bet);
        }

        /// <summary>
        /// Terminates the <see cref="SlotGame"/>
        /// </summary>
        public static void Exit()
        {
            _loadedSlotGame?.Exit();
            Terminated = true;
        }

        /// <summary>
        /// Deposits funds to the <see cref="Player"/>'s wallet
        /// </summary>
        /// <param name="amount">Amount to deposit</param>
        public static void Deposit(decimal amount)
        {
            Player.Balance += amount;
            Console.WriteLine(string.Format(DEPOSIT_MESSAGE, amount, Player.Balance));
        }

        /// <summary>
        /// Withdraws funds from the <see cref="Player"/>'s wallet
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        public static void Withdraw(decimal amount)
        {
            if (!new WithdrawValidator().Validate())
                return;

            Player.Balance -= amount;
            Console.WriteLine(string.Format(WITHDRAW_MESSAGE, amount, Player.Balance));
        }
    }
}
