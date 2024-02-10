using System.Runtime.InteropServices;

namespace Gol;

internal static partial class Library
{
    const string DllName = "GameOfLife.dll";

    [LibraryImport(DllName)]
    internal static unsafe partial nint CreateGame(nint rows, nint cols);

    [LibraryImport(DllName)]
    internal static unsafe partial void DeleteGame(nint game);

    [LibraryImport(DllName)]
    internal static unsafe partial void GetError(nint game, byte* error, nint size);

    [LibraryImport(DllName)]
    internal static unsafe partial nint Seed(nint game, byte* seed, nint size);

    [LibraryImport(DllName)]
    internal static unsafe partial byte* GetState(nint game);

    [LibraryImport(DllName)]
    internal static unsafe partial void Tick(nint game);
}
