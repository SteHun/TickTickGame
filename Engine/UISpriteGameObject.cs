using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;

public class UISpriteGameObject : SpriteGameObject
{
    public UISpriteGameObject(string spriteName, float depth, int sheetIndex = 0) : base(spriteName,depth,sheetIndex)
    {
    }
    
    /// <summary>
    /// Draws this SpriteGameObject on the screen, using its global position and origin. 
    /// Note that the object will only get drawn if it's actually marked as visible.
    /// Is NOT affected by camera.
    /// </summary>
    /// <param name="gameTime">An object containing information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">A sprite batch object used for drawing sprites.</param>
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (!Visible)
            return;

        // draw the sprite at its *global* position in the game world
        if (sprite != null)
            sprite.Draw(spriteBatch, GlobalPosition, Origin);
    }
}