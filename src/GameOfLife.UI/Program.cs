// See https://aka.ms/new-console-template for more information
using Gol;
using Spectre.Console;
using System.Text;

Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    cancellation.Cancel();
    e.Cancel = true;
};

AnsiConsole.MarkupLine("[green]Conway's Game of Life[/]");

var seed = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Select a [i]seed[/]")
        .PageSize(10)
        .MoreChoicesText("\"[grey](Move up and down to reveal more seeds)[/]")
        .AddChoices(Directory.EnumerateFiles("Seeds"))
        .UseConverter(Path.GetFileNameWithoutExtension));

using var game = new Game(File.ReadAllText(seed));
var layout = new Layout("Root");
try
{
    AnsiConsole.WriteLine("Press any key to start");
    AnsiConsole.Write(layout.Update(game.GetGrid()));
    AnsiConsole.Console.Input.ReadKey(intercept: true);

    // Game loop.
    while (!cancellation.IsCancellationRequested)
    {
        AnsiConsole.Clear();
        game.Tick();
        AnsiConsole.Write(layout.Update(game.GetGrid()));
        await Task.Delay(500, cancellation.Token);
    }
}
catch (TaskCanceledException)
{
    // Cancelled.
}
