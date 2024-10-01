using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Scenes;

internal class GameScene(World world, TimeSpan gameSpeed) : IScene
{
    private readonly TimeSpan _gameSpeed = gameSpeed;
    private GraphicsDevice _graphicsDevice = null!;
    private Texture2D _cell = null!;
    private Texture2D _cellBackground = null!;
    private TimeSpan _timeUntilNextEvolution = gameSpeed;

    private Vector2 _topLeftOffset = Vector2.Zero;

    public void Initialize(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void Load(ContentManager contentManager)
    {
        _cell = contentManager.Load<Texture2D>("images/cell");
        _cellBackground = contentManager.Load<Texture2D>("images/cell_bg");
    }

    public void Unload()
    {
        _cell?.Dispose();
        _cellBackground?.Dispose();
    }

    public void Update(GameTime gameTime)
    {
        _timeUntilNextEvolution -= gameTime.ElapsedGameTime;
        if (_timeUntilNextEvolution > TimeSpan.Zero)
            return;

        while (_timeUntilNextEvolution < TimeSpan.Zero)
            _timeUntilNextEvolution += _gameSpeed;

        world.Evolve();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        const int Size = 32;

        var hGridWidth = (world.Cols * Size) / 2;
        var hGridHeight = (world.Rows * Size) / 2;

        var hBoundsWidth = (_graphicsDevice.PresentationParameters.Bounds.Width) / 2;
        var hBoundsHeight = (_graphicsDevice.PresentationParameters.Bounds.Height) / 2;

        _topLeftOffset = new Vector2(hBoundsWidth - hGridWidth, hBoundsHeight - hGridHeight);

        for (var y = 0; y < world.Rows; ++y)
        {
            for (var x = 0; x < world.Cols; ++x)
            {
                var position = _topLeftOffset + new Vector2(x * Size, y * Size);
                spriteBatch.Draw(_cellBackground, position, Color.DarkGray);
                if (world[y, x] is State.Alive)
                    spriteBatch.Draw(_cell, position, new Color(82, 155, 156));
            }
        }
    }
}
