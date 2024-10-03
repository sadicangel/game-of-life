using GameOfLife.Scenes;

namespace GameOfLife.Services;
internal sealed class SceneManager
{
    private readonly Stack<IScene> _scenes = [];

    public IScene Scene { get => _scenes.Peek(); }

    public int Count => _scenes.Count;

    public void PushScene(IScene scene) => _scenes.Push(scene);
    public IScene PopScene() => _scenes.Pop();
}
