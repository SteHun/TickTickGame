using System;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Represents a rocket enemy that flies horizontally through the screen.<br/>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> AnimatedGameObject -> Rocket
/// </summary>
class Rocket : AnimatedGameObject
{
    Level level;
    Vector2 startPosition;
    const float speed = 500;
    private bool isActive;

    public Rocket(Level level, Vector2 startPosition, bool facingLeft) 
        : base(TickTick.Depth_LevelObjects)
    {
        this.level = level;

        LoadAnimation("Sprites/LevelObjects/Rocket/spr_rocket@3", "rocket", true, 0.1f);
        PlayAnimation("rocket");
        SetOriginToCenter();

        sprite.Mirror = facingLeft;
        if (sprite.Mirror)
        {
            velocity.X = -speed;
            this.startPosition = startPosition + new Vector2(2*speed, 0);
        }
        else
        {
            velocity.X = speed;
            this.startPosition = startPosition - new Vector2(2 * speed, 0);
        }
        Reset();
    }

    public override void Reset()
    {
        // go back to the starting position
        LocalPosition = startPosition;
        isActive = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // if the rocket has left the screen, reset it
        if (sprite.Mirror && BoundingBox.Right < level.BoundingBox.Left)
            Reset();
        else if (!sprite.Mirror && BoundingBox.Left > level.BoundingBox.Right)
            Reset();
        
        //No collision if rocket is not active (still moves to call reset at right time)
        if (!isActive)
            return;

        // if the rocket touches the player, the player dies
        if (level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
        {
            //Check if player jumped on rocket
            if (level.Player.GlobalPosition.Y + 15 < GlobalPosition.Y)
            {
                isActive = false;
                
                //If player pressed jump then jump normally
                if (level.Player.timeSinceLastAirborneJumpPress < level.Player.jumpBufferTime)
                {
                    level.Player.Jump(); 
                }
                else //Little bounce if jump is not pressed
                {
                    level.Player.Jump(600f);
                }
            }
            else //If hit somewhere else the player dies
            {
                level.Player.Die();
            }
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        if (isActive)
        {
            base.Draw(gameTime, spriteBatch, opacity);
        }
    }
}
