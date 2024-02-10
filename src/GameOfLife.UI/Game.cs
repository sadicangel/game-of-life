using Spectre.Console;

namespace Gol;
public sealed class Game : IDisposable
{
    const char DEAD = '.';
    const char ALIVE = '*';

    private readonly int _rows;
    private readonly int _cols;
    private readonly int _size;
    private readonly nint _game;

    public Game(int rows, int cols)
    {
        _rows = rows;
        _cols = cols;
        _size = _rows * _cols;
        _game = Library.CreateGame(_rows, _cols);
    }

    public Game(string seed)
    {
        var lines = seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _rows = lines.Length;
        _cols = lines[0].Length;
        _size = _rows * _cols;
        _game = Library.CreateGame(_rows, _cols);
        Seed(lines);
    }

    public void Seed(string seed) => Seed(seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

    public void Seed(string[] lines)
    {
        var data = new byte[_size];
        for (int y = 0; y < lines.Length; ++y)
        {
            for (int x = 0; x < lines[y].Length; ++x)
            {
                var value = lines[y][x];
                if (value is not '*' and not '.')
                    throw new ArgumentException("Invalid seed");

                data[y * _cols + x] = (byte)lines[y][x];
            }
        }

        unsafe
        {
            fixed (byte* ptr = data)
            {
                if (Library.Seed(_game, ptr, _size) is var result && result != 0)
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
            .AddColumns(_cols);
        unsafe
        {
            var state = Library.GetState(_game);
            for (int y = 0; y < _rows; ++y)
            {
                var row = new string[_cols];
                for (int x = 0; x < _cols; ++x)
                {
                    row[x] = state[y * _cols + x] == ALIVE ? $"[darkorange3]{ALIVE}[/]" : $"[silver]{DEAD}[/]";
                }
                grid.AddRow(row);
            }
        }
        return grid;
    }
}
