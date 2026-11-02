using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    public class ExitCommand() : Command(CommandType.Exit)
    {
        public override void Execute()
        {
            GameEnvironment.Exit();
        }
    }
}
