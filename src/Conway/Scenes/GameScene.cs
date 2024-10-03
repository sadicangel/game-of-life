using GameOfLife.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.Scenes;

internal sealed class GameScene : IScene
{
    private readonly TimeSpan _gameSpeed = TimeSpan.FromMilliseconds(200);
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _contentManager;
    private readonly SceneManager _sceneManager;
    private readonly KeyboardManager _keyboardManager;
    private readonly SpriteFont _font;
    private readonly Texture2D _cell;
    private readonly Texture2D _cellBackground;
    private readonly World _world;

    private bool _isRunning = true;
    private TimeSpan _timeUntilNextEvolution = TimeSpan.FromMilliseconds(200);
    private Vector2 _topLeftOffset = Vector2.Zero;

    public GameScene(GameServiceContainer services, string seed)
    {
        _graphicsDevice = services.GetRequiredService<IGraphicsDeviceService>().GraphicsDevice;
        _contentManager = services.GetRequiredService<ContentManager>();
        _sceneManager = services.GetRequiredService<SceneManager>();
        _keyboardManager = services.GetRequiredService<KeyboardManager>();
        _font = _contentManager.Load<SpriteFont>("fonts/Arcade");
        _cell = _contentManager.Load<Texture2D>("images/cell");
        _cellBackground = _contentManager.Load<Texture2D>("images/cell_bg");
        _world = new World(seed);
    }

    public void Update(GameTime gameTime)
    {
        _keyboardManager.Update();
        if (_keyboardManager.IsKeyPressed(Keys.Space))
        {
            _isRunning = !_isRunning;
        }

        if (_keyboardManager.IsKeyPressed(Keys.Escape))
        {
            _sceneManager.PopScene();
        }

        _timeUntilNextEvolution -= gameTime.ElapsedGameTime;
        if (_timeUntilNextEvolution > TimeSpan.Zero)
            return;

        while (_timeUntilNextEvolution < TimeSpan.Zero)
            _timeUntilNextEvolution += _gameSpeed;

        if (_isRunning)
        {
            _world.Evolve();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        const int Size = 32;

        var hGridWidth = _world.Cols * Size / 2;
        var hGridHeight = _world.Rows * Size / 2;

        var hBoundsWidth = _graphicsDevice.PresentationParameters.Bounds.Width / 2;
        var hBoundsHeight = _graphicsDevice.PresentationParameters.Bounds.Height / 2;

        _topLeftOffset = new Vector2(hBoundsWidth - hGridWidth, hBoundsHeight - hGridHeight);

        for (var y = 0; y < _world.Rows; ++y)
        {
            for (var x = 0; x < _world.Cols; ++x)
            {
                var position = _topLeftOffset + new Vector2(x * Size, y * Size);
                spriteBatch.Draw(_cellBackground, position, Color.DarkGray);
                if (_world[y, x] is State.Alive)
                    spriteBatch.Draw(_cell, position, new Color(82, 155, 156));
            }
        }

        var generation = $"Generation {_world.Generation}";
        var generationMeasure = _font.MeasureString(generation) / 2;
        var generationPosition = new Vector2(_topLeftOffset.X, _world.Rows * Size + _topLeftOffset.Y + 10);
        spriteBatch.DrawString(_font, generation, generationPosition, Color.DarkOrange);

        if (!_isRunning)
        {
            const string Pause = "Pause";
            var measure = _font.MeasureString(Pause) / 2;
            var pausePosition = new Vector2(hBoundsWidth - measure.X, hBoundsHeight - measure.Y);
            spriteBatch.DrawString(_font, Pause, pausePosition, Color.DarkOrange);
        }
    }
}
