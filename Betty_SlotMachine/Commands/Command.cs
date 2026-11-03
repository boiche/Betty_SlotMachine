using Betty_SlotMachine.Games;

namespace Betty_SlotMachine.Commands
{
    public abstract class Command(CommandType Type)
    {
        public CommandType Type { get; } = Type;

        public abstract void Execute(GameEnvironment environment);
    }
}
