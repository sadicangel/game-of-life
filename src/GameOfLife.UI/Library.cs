using System.Runtime.InteropServices;

namespace Gol;

internal static partial class Library
{
    const string DllName = "GameOfLife.dll";

    [LibraryImport(DllName)]
    internal static unsafe partial void Greet(byte* name);

    [LibraryImport(DllName)]
    internal static unsafe partial void Tick(byte* prev, byte* next, nuint size);
}
