using Betty_SlotMachine.Commands;
using Betty_SlotMachine.Games;

GameEnvironment.LoadGame(new BettyGame());

while (true)
{
    CommandContext.ReadCommand();

    CommandContext.ExecuteCommand();
    if (GameEnvironment.Terminated)
        break;
}