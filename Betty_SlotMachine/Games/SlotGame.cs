using Betty_SlotMachine.Configuration;

namespace Betty_SlotMachine.Games
{
    /// <summary>
    /// Represents base slot game experience
    /// </summary>
    public abstract class SlotGame
    {
        protected bool _configured;
        protected SlotGameConfiguration _configuration;
        protected List<(long Upper, GameOutcome Outcome)> _outcomeBounds;

        public SlotGameConfiguration Configuration => _configuration;

        /// <summary>
        /// Executes single run of the game
        /// </summary>
        /// <param name="bet">Amount to bet</param>
        public abstract decimal Bet(decimal bet);

        /// <summary>
        /// Applies all policies required for the game
        /// </summary>
        public abstract bool Configure();

        /// <summary>
        /// Terminates the game
        /// </summary>
        public abstract void Exit();

        public abstract void Boot();
    }
}
