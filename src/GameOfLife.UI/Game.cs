using Spectre.Console;

namespace Gol;
public sealed class Game
{
    private const byte DEAD = 0;
    private const byte ALIVE = 1;

    private readonly byte[] _grid;
    private readonly byte[] _next;
    private readonly int _size;

    public Game(string seed)
    {
        var lines = seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length != lines[0].Length)
            throw new InvalidOperationException("Only square grids supported for now");

        _size = lines.Length;
        _grid = new byte[_size * _size];
        _next = new byte[_grid.Length];

        for (int y = 0; y < lines.Length; ++y)
        {
            for (int x = 0; x < lines[y].Length; ++x)
            {
                _grid[y * _size + x] = lines[y][x] is '*' ? ALIVE : DEAD;
            }
        }
    }

    public byte this[int x, int y] { get { unsafe { return _grid[y * _size + x]; } } }


    public void Tick()
    {
        unsafe
        {
            Span<byte> prevSpan = _grid;
            Span<byte> nextSpan = _next;
            nextSpan.Clear();

            fixed (byte* prev = prevSpan, next = nextSpan)
            {
                Library.Tick(prev, next, (nuint)_size);
            }

            nextSpan.CopyTo(prevSpan);
        }
    }

    internal Grid GetGrid()
    {
        var grid = new Grid()
            .AddColumns(_size);
        unsafe
        {
            for (int y = 0; y < _size; ++y)
            {
                var row = new string[_size];
                for (int x = 0; x < _size; ++x)
                {
                    row[x] = _grid[y * _size + x] == ALIVE ? "[darkorange3]*[/]" : "[silver].[/]";
                }
                grid.AddRow(row);
            }
        }
        return grid;
    }
}
