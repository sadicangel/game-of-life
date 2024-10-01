// See https://aka.ms/new-console-template for more information
using System.Text;
using Gol;
using Spectre.Console;

Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

using var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    cancellation.Cancel();
    e.Cancel = true;
};

AnsiConsole.MarkupLine("[green]Conway's Game of Life[/]");

var file = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Select a [i]seed[/]")
        .PageSize(10)
        .MoreChoicesText("\"[grey](Move up and down to reveal more seeds)[/]")
        .AddChoices(Directory.EnumerateFiles("assets/seeds"))
        .UseConverter(Path.GetFileNameWithoutExtension));
var seed = File.ReadAllText(file);
AnsiConsole.WriteLine(file);
AnsiConsole.WriteLine(seed);
AnsiConsole.WriteLine();

var speed = AnsiConsole.Prompt(
    new TextPrompt<int>("[green]Game speed (milliseconds)[/]")
        .DefaultValue(200)
        .Validate(i => i > 100));
AnsiConsole.WriteLine();

AnsiConsole.WriteLine("Press any key to start..");
AnsiConsole.Console.Input.ReadKey(intercept: true);
AnsiConsole.Clear();

var world = new World(seed);
var layout = new Layout("Root");
try
{
    // Game loop.
    while (!cancellation.IsCancellationRequested)
    {
        AnsiConsole.Cursor.SetPosition(0, 0);
        AnsiConsole.Write(layout.Update(world.ToGrid()));
        world.Evolve();
        await Task.Delay(speed, cancellation.Token);
    }
}
catch (TaskCanceledException)
{
    // Cancelled.
}
