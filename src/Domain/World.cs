using System.Diagnostics;
using System.Text;

namespace GameOfLife;

public sealed class World
{
    private readonly State[] _main;
    private readonly State[] _temp;

    public World(int rows, int cols)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(cols, 0);

        Rows = rows;
        Cols = cols;
        _main = new State[rows * cols];
        _temp = new State[_main.Length];
    }

    public World(string seed)
    {
        var lines = seed.Split(default(char[]), options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        Rows = lines.Length;
        Cols = lines[0].Length;

        _main = new State[Rows * Cols];
        _temp = new State[_main.Length];

        for (var y = 0; y < Rows; ++y)
        {
            var line = lines[y];

            if (line.Length != Cols)
                throw new ArgumentException($"Invalid seed: line at index {y} does not have the correct length. Expected {Cols}. Actual {line.Length}");

            for (var x = 0; x < Cols; ++x)
            {
                var state = (State)line[x];
                if (!Enum.IsDefined(state))
                    throw new ArgumentException($"Invalid seed: must be in set [{string.Join(", ", Enum.GetValues<State>().Select(v => (char)v))}]");

                _main[y * Cols + x] = state;
            }
        }
    }

    public int Rows { get; }
    public int Cols { get; }
    public long Generation { get; private set; }

    public ReadOnlySpan<State> this[int row] => _main.AsSpan(row * Cols, Cols);

    public State this[int row, int col] => _main[row * Cols + col];

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (var y = 0; y < Rows; ++y)
        {
            for (var x = 0; x < Cols; ++x)
            {
                builder.Append(_main[x + y * Cols] is State.Alive
                    ? '*'
                    : '.');
                builder.Append(' ');
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }

    private int CountNeighbours(int cx, int cy)
    {
        var neighbours = 0;
        for (var dx = -1; dx <= 1; ++dx)
        {
            for (var dy = -1; dy <= 1; ++dy)
            {
                if (dx is 0 && dy is 0)
                    continue;

                var x = Mod(cx + dx, Cols);
                var y = Mod(cy + dy, Rows);
                if (this[y, x] is State.Alive)
                    ++neighbours;
            }
        }
        return neighbours;

        static int Mod(int a, int b) => (a % b + b) % b;
    }

    public void Evolve()
    {
        var i = 0;
        for (var y = 0; y < Rows; ++y)
        {
            for (var x = 0; x < Cols; ++x)
            {
                var neighbours = CountNeighbours(x, y);
                var isAlive = neighbours is 3 || neighbours is 2 && _main[i] is State.Alive;
                _temp[i] = isAlive ? State.Alive : State.Dead;

                ++i;
            }
        }
        _temp.AsSpan().CopyTo(_main);

        if (_main.All(x => x is State.Dead))
            Debugger.Break();

        ++Generation;
    }

}
