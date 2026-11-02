using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    public class WithdrawCommand(decimal withdrawAmount) : Command(CommandType.Withdraw)
    {
        public decimal WithdrawAmount { get; } = withdrawAmount;

        public override void Execute()
        {
            GameEnvironment.Withdraw(WithdrawAmount);
        }
    }
}
