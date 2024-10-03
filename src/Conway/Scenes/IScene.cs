using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Scenes;

internal interface IScene
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}
