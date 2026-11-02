namespace Betty_SlotMachine.Commands
{
    public abstract class Command(CommandType Type)
    {
        public CommandType Type { get; } = Type;

        public abstract void Execute();
    }
}
