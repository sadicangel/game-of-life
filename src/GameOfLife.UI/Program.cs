// See https://aka.ms/new-console-template for more information
using Gol;
using Spectre.Console;


using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    cancellation.Cancel();
    e.Cancel = true;
};

using var game = new Game(10);
var layout = new Layout("Root");
try
{
    while (!cancellation.IsCancellationRequested)
    {
        AnsiConsole.Clear();
        game.Tick();
        AnsiConsole.Write(layout.Update(Align.Center(game.GetGrid(), VerticalAlignment.Middle)));
        await Task.Delay(1000, cancellation.Token);
    }
}
catch (TaskCanceledException)
{
    // Cancelled by user.
}