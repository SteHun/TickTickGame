using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> AnimatedGameObject -> Player
/// </summary>
class Player : AnimatedGameObject
{
    const float walkingSpeed = 400; // Standard walking speed, in game units per second.
    const float speedBoostSpeed = 800; // Speed when having stepped on a speed boost tile.
    const float jumpSpeed = 700; // Lift-off speed when the character jumps.
    const float gravity = 2300; // Strength of the gravity force that pulls the character down.
    const float maxFallSpeed = 1200; // The maximum vertical speed at which the character can fall.
    
    const float iceFriction = 1; // Friction factor that determines how slippery the ice is; closer to 0 means more slippery.
    const float normalFriction = 20; // Friction factor that determines how slippery a normal surface is.
    const float airFriction = 10; // Friction factor that determines how much (horizontal) air resistance there is.

    private const double coyoteTime = 150; // Milliseconds of coyote time
    public readonly double jumpBufferTime = 120; // Milliseconds of jump buffer
    private const double increasedJumpTime = 200; //Milliseconds of holding down space to increase jump height
    private const double increasedJumpStartTime = 50; //Milliseconds of holding down space before jump is increased
    private const double speedBoostTime = 2000; //Milliseconds of speed boost when standing on boost pad

    bool facingLeft; // Whether or not the character is currently looking to the left.

    private float speed = walkingSpeed;
    private bool isJumping;
    bool isGrounded; // Whether or not the character is currently standing on something.
    bool standingOnIceTile, standingOnHotTile, standingOnSpeedTile; // Whether or not the character is standing on an ice tile or a hot tile.
    float desiredHorizontalSpeed; // The horizontal speed at which the character would like to move.
    private double timeSinceLastGrounded = 0;
    private double timeSinceJumpStart = 0;
    private double timeSinceSpeedBoost = speedBoostTime+1; //Doesn't activate on start
    private bool canJump => !isJumping && timeSinceLastGrounded < coyoteTime && IsAlive;
    public double timeSinceLastAirborneJumpPress = 100000; // any large enough number

    Level level;
    Vector2 startPosition;
    
    bool isCelebrating; // Whether or not the player is celebrating a level victory.
    bool isExploding;
    

    public bool IsAlive { get; private set; }

    public bool CanCollideWithObjects { get { return IsAlive && !isCelebrating; } }

    public bool IsMoving { get { return velocity != Vector2.Zero; } }

    public Player(Level level, Vector2 startPosition) : base(TickTick.Depth_LevelPlayer)
    {
        this.level = level;
        this.startPosition = startPosition;

        // load all animations
        LoadAnimation("Sprites/LevelObjects/Player/spr_idle", "idle", true, 0.1f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_run@13", "run", true, 0.04f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_jump@14", "jump", false, 0.08f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_celebrate@14", "celebrate", false, 0.05f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_die@5", "die", true, 0.1f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_explode@6x3", "explode", false, 0.04f, 3);
        
        //Really weird location, is offset from GlobalPosition (which is somewhere around bottom right for some reason)
        HitBox = new Rectangle(-38, -80, 76, 80);
        
        Reset();
    }

    public override void Reset()
    {
        // go back to the starting position
        localPosition = startPosition;
        velocity = Vector2.Zero;
        speed = walkingSpeed;
        desiredHorizontalSpeed = 0;
        timeSinceJumpStart = 999; // this is jank but it works haha
        timeSinceSpeedBoost = 999;

        // start with the idle sprite
        PlayAnimation("idle", true);
        SetOriginToBottomCenter();
        facingLeft = false;
        isGrounded = true;
        standingOnIceTile = standingOnHotTile = false;

        IsAlive = true;
        isExploding = false;
        isCelebrating = false;
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        if (!CanCollideWithObjects)
            return;

        // arrow keys: move left or right
        if (inputHelper.KeyDown(Keys.Left))
        {
            if (velocity.X > 0)
                timeSinceLastGrounded = coyoteTime; // resets the timer
            facingLeft = true;
            desiredHorizontalSpeed = -speed;
            if (isGrounded && !isJumping)
                PlayAnimation("run");
        }
        else if (inputHelper.KeyDown(Keys.Right))
        {
            if (velocity.X < 0)
                timeSinceLastGrounded = coyoteTime; // resets the timer
            facingLeft = false;
            desiredHorizontalSpeed = speed;
            if (isGrounded && !isJumping)
                PlayAnimation("run");
        }

        // no arrow keys: don't move
        else
        {
            timeSinceLastGrounded = coyoteTime;
            desiredHorizontalSpeed = 0;
            if (isGrounded && !isJumping)
                PlayAnimation("idle");
        }

        // spacebar: jump
        if (inputHelper.KeyPressed(Keys.Space))
        {
            if (canJump)
                Jump();
            else
                timeSinceLastAirborneJumpPress = 0;
        }
        else
        {
            //If not pressed on first frame
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && timeSinceJumpStart is > increasedJumpStartTime and < increasedJumpTime)
                JumpHold();
        }

        // falling?
        if (!isGrounded && !canJump && !isJumping)
            PlayAnimation("jump", true, 8);

        // set the origin to the character's feet
        SetOriginToBottomCenter();

        // make sure the sprite is facing the correct direction
        sprite.Mirror = facingLeft;
    }

    public void Jump(float speed = jumpSpeed)
    {
        isJumping = true;
        velocity.Y = -speed;
        timeSinceJumpStart = 0;
        // play the jump animation; always make sure that the animation restarts
        PlayAnimation("jump", true, 0);
        // play a sound
        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_jump");
    }

    private void JumpHold(float speed = jumpSpeed)
    {
        isJumping = true;
        
        //Jump strength is decreased by how long the jump is held, to get a more smooth jump curve
        velocity.Y = -speed + (float)timeSinceJumpStart/2;
    }

    /// <summary>
    /// Returns whether or not the Player is currently falling down.
    /// </summary>
    public bool IsFalling
    {
        get { return velocity.Y > 0 && !isGrounded; }
    }

    void SetOriginToBottomCenter()
    {
        Origin = new Vector2(sprite.Width / 2, sprite.Height);
    }

    public override void Update(GameTime gameTime)
    {
        Vector2 previousPosition = localPosition;

        //Adjust speed based on if having stood on a speed boost tile recently
        speed = timeSinceSpeedBoost < speedBoostTime ? speedBoostSpeed : walkingSpeed;

        if (CanCollideWithObjects)
            ApplyFriction(gameTime);
        else
            velocity.X = 0;

        if (!isExploding)
            ApplyGravity(gameTime);

        Camera.Update(localPosition);
        base.Update(gameTime);

        if (IsAlive)
        {
            // check for collisions with tiles
            HandleTileCollisions(previousPosition);
            // check if we've fallen down through the level
            if (BoundingBox.Center.Y > level.BoundingBox.Bottom)
                Die();

            if (standingOnHotTile)
                level.Timer.Multiplier = 2;
            else
                level.Timer.Multiplier = 1;
            
            // coyote time
            if (isGrounded)
                timeSinceLastGrounded = 0;
            else
                timeSinceLastGrounded += gameTime.ElapsedGameTime.TotalMilliseconds;

            timeSinceJumpStart += gameTime.ElapsedGameTime.TotalMilliseconds;
            timeSinceSpeedBoost += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            // jump buffer
            timeSinceLastAirborneJumpPress += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceLastAirborneJumpPress < jumpBufferTime && isGrounded)
                Jump();
        }
            
    }

    void ApplyFriction(GameTime gameTime)
    {
        // determine the friction coefficient for the character
        float friction;
        if (standingOnIceTile)
            friction = iceFriction;
        else if (isGrounded)
            friction = normalFriction;
        else
            friction = airFriction;

        // calculate how strongly the horizontal speed should move towards the desired value
        float multiplier = MathHelper.Clamp(friction * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);

        // update the horizontal speed
        velocity.X += (desiredHorizontalSpeed - velocity.X) * multiplier;
        if (Math.Abs(velocity.X) < 1)
            velocity.X = 0;
    }

    void ApplyGravity(GameTime gameTime)
    {
        if (canJump)
            return;
        velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (velocity.Y > maxFallSpeed)
            velocity.Y = maxFallSpeed;
    }

    // Checks for collisions between the character and the level's tiles, and handles these collisions when needed.
    void HandleTileCollisions(Vector2 previousPosition)
    {
        isGrounded = false;
        standingOnIceTile = false;
        standingOnHotTile = false;

        // determine the range of tiles to check
        Rectangle bbox = BoundingBoxForCollisions;
        Point topLeftTile = level.GetTileCoordinates(new Vector2(bbox.Left, bbox.Top)) - new Point(1, 1);
        Point bottomRightTile = level.GetTileCoordinates(new Vector2(bbox.Right, bbox.Bottom)) + new Point(1, 1);

        for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
        {
            for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
            {
                Tile.Type tileType = level.GetTileType(x, y);

                // ignore empty tiles
                if (tileType == Tile.Type.Empty)
                    continue;

                // ignore platform tiles if the player is standing below them
                Vector2 tilePosition = level.GetCellPosition(x, y);
                if (tileType == Tile.Type.Platform && localPosition.Y > tilePosition.Y && previousPosition.Y > tilePosition.Y)
                    continue;

                // if there's no intersection with the tile, ignore this tile
                Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                if (!tileBounds.Intersects(bbox))
                    continue;

                // calculate how large the intersection is
                Rectangle overlap = CollisionDetection.CalculateIntersection(bbox, tileBounds);

                // if the x-component is smaller, treat this as a horizontal collision
                if (overlap.Width < overlap.Height)
                {
                    if ((velocity.X >= 0 && bbox.Center.X < tileBounds.Left) || // right wall
                        (velocity.X <= 0 && bbox.Center.X > tileBounds.Right)) // left wall
                    {
                        localPosition.X = previousPosition.X;
                        velocity.X = 0;
                    }
                }

                // otherwise, treat this as a vertical collision
                else
                {
                    if (velocity.Y >= 0 && bbox.Center.Y < tileBounds.Top && overlap.Width > 6) // floor
                    {
                        isGrounded = true;
                        isJumping = false;
                        velocity.Y = 0;
                        localPosition.Y = tileBounds.Top;

                        // check the surface type: are we standing on a hot tile or an ice tile?
                        Tile.SurfaceType surface = level.GetSurfaceType(x, y);
                        switch (surface)
                        {
                            case Tile.SurfaceType.Hot:
                                standingOnHotTile = true;
                                break;
                            case Tile.SurfaceType.Ice:
                                standingOnIceTile = true;
                                break;
                            case Tile.SurfaceType.Speed:
                                timeSinceSpeedBoost = 0;
                                break;
                        }
                    }
                    else if (velocity.Y <= 0 && bbox.Center.Y > tileBounds.Bottom && overlap.Height > 2) // ceiling
                    {
                        localPosition.Y = previousPosition.Y;
                        velocity.Y = 0;
                    }
                }
            }
        }
    }
    
    // Handles physics enhancements like coyote time and jump buffering

    Rectangle BoundingBoxForCollisions
    {
        get
        {
            Rectangle bbox = BoundingBox;
            // adjust the bounding box
            bbox.X += 20;
            bbox.Width -= 40;
            bbox.Height += 1;

            return bbox;
        }
    }

    public void Die()
    {
        timeSinceLastAirborneJumpPress = jumpBufferTime;
        IsAlive = false;
        PlayAnimation("die");
        velocity = new Vector2(0, -jumpSpeed);
        level.Timer.Running = false;

        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_die");
    }

    public void Explode()
    {
        IsAlive = false;
        isExploding = true;
        PlayAnimation("explode");
        velocity = Vector2.Zero;
        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_explode");
    }

    /// <summary>
    /// Lets this Player object start celebrating due to completing a level.
    /// The Player will show an animation, and it will stop responding to keyboard input.
    /// </summary>
    public void Celebrate()
    {
        isCelebrating = true;
        PlayAnimation("celebrate");
        SetOriginToBottomCenter();

        // stop moving
        velocity = Vector2.Zero;
    }
}