using System;
using Microsoft.Xna.Framework;

/// <summary>
/// A variant of PatrollingEnemy that follows the player whenever the player moves.<br/>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> AnimatedGameObject -> PatrollingEnemy -> PlayerFollowingEnemy
/// </summary>
class PlayerFollowingEnemy : PatrollingEnemy
{
    public PlayerFollowingEnemy(Level level, Vector2 startPosition) 
        : base(level, startPosition) { }

    protected override void Load()
    {
        LoadAnimation("Sprites/LevelObjects/Flame/spr_flame_blue@9", "default", true, 0.1f);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // if the player is moving and we're not already waiting, follow the player
        if (level.Player.CanCollideWithObjects && level.Player.IsMoving && velocity.X != 0)
        {
            float dx = level.Player.GlobalPosition.X - GlobalPosition.X;
            if (Math.Sign(dx) != Math.Sign(velocity.X) && Math.Abs(dx) > 100)
                TurnAround();
        }
    }
}

