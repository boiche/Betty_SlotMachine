using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    public class BetCommand(decimal betAmount) : Command(CommandType.Bet)
    {
        public decimal BetAmount { get; } = betAmount;

        public override void Execute(GameEnvironment environment)
        {
            environment.Bet(BetAmount);
        }
    }
}
