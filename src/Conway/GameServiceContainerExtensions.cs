using Microsoft.Xna.Framework;

namespace GameOfLife;
internal static class GameServiceContainerExtensions
{
    public static T GetRequiredService<T>(this GameServiceContainer services) where T : class =>
        services.GetService<T>() ?? throw new InvalidOperationException($"Unable to find service of type '{typeof(T)}'");
}
