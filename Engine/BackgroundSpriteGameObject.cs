using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;

public class BackgroundSpriteGameObject : SpriteGameObject
{
    private  readonly float parallaxStrength;
    
    public BackgroundSpriteGameObject(string spriteName, float depth, float parallaxStrength = 5f) : base(spriteName, depth)
    {
        this.parallaxStrength = parallaxStrength;
    }
    
    /// <summary>
    /// Draws this SpriteGameObject on the screen, using its global position and origin. 
    /// Note that the object will only get drawn if it's actually marked as visible.
    /// Is affected by camera.
    /// </summary>
    /// <param name="gameTime">An object containing information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">A sprite batch object used for drawing sprites.</param>
    /// <param name="opacity">The opacity of a sprite.</param>
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        if (!Visible)
            return;

        // draw the sprite at its *global* position in the game world
        // affected by parallax
        if (sprite != null)
            sprite.Draw(spriteBatch,  GlobalPosition - Camera.position/parallaxStrength, Origin, opacity);
    }
}