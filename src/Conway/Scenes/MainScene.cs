using GameOfLife.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.Scenes;

internal sealed class MainScene : IScene
{
    private readonly GameServiceContainer _services;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _contentManager;
    private readonly SceneManager _sceneManager;
    private readonly KeyboardManager _keyboardManager;
    private readonly SpriteFont _font;
    private readonly Dictionary<string, string> _seeds;
    private readonly Dictionary<int, string> _seedIndices;
    private readonly float _stride;
    private readonly float _spacing;

    private int _selectedIndex = 0;

    public MainScene(GameServiceContainer services)
    {
        _services = services;
        _graphicsDevice = services.GetRequiredService<IGraphicsDeviceService>().GraphicsDevice;
        _contentManager = services.GetRequiredService<ContentManager>();
        _sceneManager = services.GetRequiredService<SceneManager>();
        _keyboardManager = services.GetRequiredService<KeyboardManager>();
        _font = _contentManager.Load<SpriteFont>("fonts/Arcade");
        _seeds = Directory
            .EnumerateFiles(Path.Combine(_contentManager.RootDirectory, "seeds"), "*.txt")
            .ToDictionary(
                static path => Path.GetFileNameWithoutExtension(path),
                static path =>
                {
                    using var stream = TitleContainer.OpenStream(path);
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                });
        _seedIndices = _seeds.Keys.Select((k, i) => (k, i)).ToDictionary(p => p.i, p => p.k);
        _spacing = 5;
        _stride += _spacing + _font.MeasureString(_seeds.First().Key).Y;

        // TODO: Add option for creating seeds.
    }

    private float GetOffsetX(string text)
    {
        var hTextWidth = _font.MeasureString(text).X / 2;

        var hBoundsWidth = _graphicsDevice.PresentationParameters.Bounds.Width / 2;

        return hBoundsWidth - hTextWidth;
    }

    private float GetOffsetY(int index) => _spacing + _stride * (2 + index);

    public void Update(GameTime gameTime)
    {
        _keyboardManager.Update();
        if (_keyboardManager.IsKeyPressed(Keys.Enter))
        {
            _sceneManager.PushScene(new GameScene(_services, _seeds[_seedIndices[_selectedIndex]]));
        }
        else
        {
            if (_keyboardManager.IsKeyPressed(Keys.Down))
                ++_selectedIndex;
            if (_keyboardManager.IsKeyPressed(Keys.Up))
                --_selectedIndex;

            _selectedIndex = int.Clamp(_selectedIndex, 0, _seeds.Count - 1);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        const string Header = "select seed";

        var hTextHeight = _stride * (2 + _seeds.Count / 2);

        var hBoundsHeight = _graphicsDevice.PresentationParameters.Bounds.Height / 2;

        var y = hBoundsHeight - hTextHeight;

        spriteBatch.DrawString(_font, Header, new Vector2(GetOffsetX(Header), y), Color.Black);

        var index = 0;
        foreach (var (key, _) in _seeds)
        {
            spriteBatch.DrawString(
                _font,
                key,
                new Vector2(GetOffsetX(key), y + GetOffsetY(index)),
                index == _selectedIndex ? Color.DarkOrange : Color.Black);
            index++;
        }
    }
}
