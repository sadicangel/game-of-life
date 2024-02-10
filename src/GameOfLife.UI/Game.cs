using Spectre.Console;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text.Unicode;

namespace Gol;
public sealed class Game : IDisposable
{
    private readonly unsafe byte* _grid;
    private readonly int _size;
    private readonly nuint _length;

    public Game(int size)
    {
        _size = size;
        _length = checked((nuint)(size * size));
        unsafe
        {
            _grid = (byte*)NativeMemory.AllocZeroed(_length, sizeof(byte));
            _grid[_size + 4] = 1;
            _grid[_size * 2 + 3] = 1;
            _grid[_size * 2 + 4] = 1;
            _grid[_size * 2 + 5] = 1;
        }
    }

    public byte this[int x, int y] { get { unsafe { return _grid[y * _size + x]; } } }

    public void Greet(ReadOnlySpan<char> name)
    {
        Span<byte> bytes = stackalloc byte[name.Length + 1];
        bytes[^1] = 0;

        var status = Utf8.FromUtf16(name, bytes, out _, out _);
        if (status is not OperationStatus.Done)
            throw new InvalidOperationException($"Invalid name '{name}'");

        unsafe
        {
            fixed (byte* ptr = bytes)
            {
                Library.Greet(ptr);
            }
        }
    }

    public void Tick()
    {
        unsafe
        {
            Span<byte> prevSpan = new Span<byte>(_grid, (int)_length);
            Span<byte> nextSpan = stackalloc byte[(int)_length];
            nextSpan.Clear();

            fixed (byte* prev = prevSpan, next = nextSpan)
            {
                Library.Tick(prev, next, (nuint)_size);
            }

            nextSpan.CopyTo(prevSpan);
        }
    }

    public unsafe void Dispose()
    {
        if (_grid is not null)
        {
            NativeMemory.Free(_grid);
            fixed (byte** ptr = &_grid)
                *ptr = null;
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
                    row[x] = _grid[y * _size + x] == 1 ? "[darkorange3]*[/]" : "[silver].[/]";
                }
                grid.AddRow(row);
            }
        }
        return grid;
    }
}
