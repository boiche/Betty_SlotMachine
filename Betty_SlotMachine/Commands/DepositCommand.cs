using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    public class DepositCommand(decimal depositAmount) : Command(CommandType.Deposit)
    {
        public decimal DepositAmount { get; } = depositAmount;

        public override void Execute()
        {
            GameEnvironment.Deposit(DepositAmount);
        }
    }
}
