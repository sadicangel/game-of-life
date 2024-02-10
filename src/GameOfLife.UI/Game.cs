using Spectre.Console;

namespace Gol;
public sealed class Game : IDisposable
{
    const char DEAD = '.';
    const char ALIVE = '*';

    private readonly int _size;
    private readonly nint _game;

    public Game(int size)
    {
        _size = size;
        _game = Library.CreateGame(_size);
    }

    public Game(string seed)
    {
        var lines = seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _size = lines.Length;
        _game = Library.CreateGame(_size);
        Seed(lines);
    }

    public void Seed(string seed) => Seed(seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

    public void Seed(string[] lines)
    {
        if (lines.Length != lines[0].Length)
            throw new NotSupportedException("Non square grids");

        var data = new byte[_size * _size];
        for (int y = 0; y < lines.Length; ++y)
        {
            for (int x = 0; x < lines[y].Length; ++x)
            {
                var value = lines[y][x];
                if (value is not '*' and not '.')
                    throw new ArgumentException("Invalid seed");

                data[y * _size + x] = (byte)lines[y][x];
            }
        }

        unsafe
        {
            fixed (byte* ptr = data)
            {
                if (Library.Seed(_game, ptr, _size * _size) is var result && result != 0)
                {
                    Span<byte> error = new byte[result];
                    fixed (byte* ptr2 = error)
                    {
                        Library.GetError(_game, ptr2, result);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        if (_game != 0)
        {
            unsafe
            {
                Library.DeleteGame(_game);
                fixed (nint* ptr = &_game)
                    *ptr = 0;
            }
        }
    }

    public void Tick()
    {
        unsafe
        {
            Library.Tick(_game);
        }
    }

    internal Grid GetGrid()
    {
        var grid = new Grid()
            .AddColumns(_size);
        unsafe
        {
            var state = Library.GetState(_game);
            for (int y = 0; y < _size; ++y)
            {
                var row = new string[_size];
                for (int x = 0; x < _size; ++x)
                {
                    row[x] = state[y * _size + x] == ALIVE ? $"[darkorange3]{ALIVE}[/]" : $"[silver]{DEAD}[/]";
                }
                grid.AddRow(row);
            }
        }
        return grid;
    }
}
