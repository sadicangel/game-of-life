using System.Runtime.InteropServices;

namespace Gol;

internal static partial class Library
{
    const string DllName = "GameOfLife.dll";

    [LibraryImport(DllName)]
    internal static unsafe partial nint CreateGame(nint size);

    [LibraryImport(DllName)]
    internal static unsafe partial void DeleteGame(nint game);

    [LibraryImport(DllName)]
    internal static unsafe partial void GetError(nint game, byte* error, nint length);

    [LibraryImport(DllName)]
    internal static unsafe partial nint Seed(nint game, byte* seed, nint length);

    [LibraryImport(DllName)]
    internal static unsafe partial byte* GetState(nint game);

    [LibraryImport(DllName)]
    internal static unsafe partial void Tick(nint game);
}
