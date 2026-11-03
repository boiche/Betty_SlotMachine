using Betty_SlotMachine.Commands;
using Betty_SlotMachine.Games;
using Betty_SlotMachine.Models;

var environment = new GameEnvironment(new BettyGame(Console.Out), new Player(), Console.Out);
var commands = new CommandContext(environment, Console.In, Console.Out);

environment.Start();

while (!environment.Terminated)
{
    commands.ReadCommand();
    commands.ExecuteCommand();
}
