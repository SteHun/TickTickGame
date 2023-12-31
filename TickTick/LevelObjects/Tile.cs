﻿using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// IGameLoopObject -> GameObject -> Title
/// </summary>
class Tile : GameObject
{
    public enum Type { Empty, Wall, Platform };
    public enum SurfaceType { Normal, Hot, Ice, Speed };

    Type type;
    SurfaceType surface;

    SpriteGameObject image;

    public Tile(Type type, SurfaceType surface)
    {
        this.type = type;
        this.surface = surface;

        // add an image depending on the type
        string surfaceExtension = "";
        switch (surface)
        {
            case SurfaceType.Hot:
                surfaceExtension = "_hot";
                break;
            case SurfaceType.Ice:
                surfaceExtension = "_ice";
                break;
            case SurfaceType.Speed:
                surfaceExtension = "_speed";
                break;
        }

        if (type == Type.Wall)
            image = new SpriteGameObject("Sprites/Tiles/spr_wall" + surfaceExtension, TickTick.Depth_LevelTiles);
        else if (type == Type.Platform)
            image = new SpriteGameObject("Sprites/Tiles/spr_platform" + surfaceExtension, TickTick.Depth_LevelTiles);

        // if there is an image, make it a child of this object
        if (image != null)
            image.Parent = this;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        // draw the image if it exists
        if (image != null)
            image.Draw(gameTime, spriteBatch, opacity);
    }

    public Type TileType
    {
        get { return type; }
    }

    public SurfaceType Surface
    {
        get { return surface; }
    }
}