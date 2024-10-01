using GameOfLife.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife;
public class GameRoot : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private readonly GameScene _scene;

    public GameRoot()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // TODO: Create a main menu scene, that allows loading different seeds.
        // TODO: Add a scene to allow creating seeds.

        _scene = new GameScene(
            new World("""
                .*..............
                ..*.............
                ***.............
                ................
                ................
                ................
                ................
                ................
                """),
            TimeSpan.FromMilliseconds(200));
    }

    protected override void Initialize()
    {
        Window.Title = "Conway's Game of Life";
        Window.AllowUserResizing = true;

        _scene.Initialize(GraphicsDevice);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scene.Load(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _scene.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(54, 69, 79));

        _spriteBatch.Begin();
        _scene.Draw(gameTime, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
