// See https://aka.ms/new-console-template for more information
using System.Text;
using GameOfLife;
using Spectre.Console;

Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

using var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    cancellation.Cancel();
    e.Cancel = true;
};

AnsiConsole.Console.Cursor.Show();
AnsiConsole.MarkupLine("[green]Conway's Game of Life[/]");

var seed = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Select a [i]seed[/]")
        .PageSize(10)
        .MoreChoicesText("\"[grey](Move up and down to reveal more seeds)[/]")
        .AddChoices(Directory.EnumerateFiles("assets/seeds"))
        .UseConverter(Path.GetFileNameWithoutExtension));

var world = new World(File.ReadAllText(seed));

AnsiConsole.MarkupLine($"[i]{Path.GetFileNameWithoutExtension(seed)}[/]");
AnsiConsole.Write(world.ToGrid());
AnsiConsole.WriteLine();

var speed = AnsiConsole.Prompt(
    new TextPrompt<int>("[green]Game speed (milliseconds)[/]")
        .DefaultValue(200)
        .Validate(i => i > 100));
AnsiConsole.WriteLine();

AnsiConsole.WriteLine("Press any key to start..");
AnsiConsole.Console.Input.ReadKey(intercept: true);
AnsiConsole.Console.Cursor.Hide();
AnsiConsole.Clear();

try
{
    // Game loop.
    while (!cancellation.IsCancellationRequested)
    {
        AnsiConsole.Cursor.SetPosition(0, 0);
        AnsiConsole.Write(world.ToGrid());
        world.Evolve();
        await Task.Delay(speed, cancellation.Token);
    }
}
catch (TaskCanceledException)
{
    // Cancelled.
}
