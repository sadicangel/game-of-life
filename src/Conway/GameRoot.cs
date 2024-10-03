using GameOfLife.Scenes;
using GameOfLife.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife;
public class GameRoot : Game
{
    private SpriteBatch _spriteBatch = null!;
    private readonly SceneManager _sceneManager = new();
    private readonly KeyboardManager _keyboardManager = new();

    public GameRoot()
    {
        _ = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.Title = "Conway's Game of Life";
        Window.AllowUserResizing = true;

        // Load services
        Services.AddService(Content);
        Services.AddService(_sceneManager);
        Services.AddService(_keyboardManager);

        _sceneManager.PushScene(new MainScene(Services));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_keyboardManager.IsKeyPressed(Keys.Escape) && _sceneManager.Count is 0)
            Exit();

        _sceneManager.Scene.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(54, 69, 79));

        _spriteBatch.Begin();
        _sceneManager.Scene.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
