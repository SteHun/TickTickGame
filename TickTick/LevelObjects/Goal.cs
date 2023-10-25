using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Goal : SpriteGameObject
{
    public bool active { get; set; }
    public Goal(string spriteName, float depth, int sheetIndex = 0) : base(spriteName, depth, sheetIndex ){}
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (!Visible)
            return;

        // draw the sprite at its *global* position in the game world
        float opacity = 0.5f;
        if (active)
            opacity = 1;
        if (sprite != null)
            sprite.Draw(spriteBatch, GlobalPosition - Camera.position, Origin, opacity);
    }
}