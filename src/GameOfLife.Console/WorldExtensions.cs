using Spectre.Console;

namespace Gol;
internal static class WorldExtensions
{
    public static Markup ToMarkup(this State state) =>
        new(state is State.Alive ? "[olive]▣[/]" : "[grey]□[/]");

    public static TResult[] Map<TSource, TResult>(this ReadOnlySpan<TSource> span, Func<TSource, TResult> map)
    {
        var result = new TResult[span.Length];
        for (var i = 0; i < span.Length; ++i)
            result[i] = map(span[i]);
        return result;
    }

    public static Grid ToGrid(this World world)
    {
        var grid = new Grid().AddColumns(world.Cols);

        for (var i = 0; i < world.Rows; ++i)
            grid.AddRow(world[i].Map(ToMarkup));

        return grid;
    }
}
