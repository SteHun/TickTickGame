using System;
using System.Diagnostics;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameStates;

public class LevelEditorState : GameState
{
    private Button saveButton;
    private Button quitButton;
    private Vector2 offset = Vector2.Zero;
    private string levelDescription;
    private char[,] level;
    private Point hoveredTile;
    private Vector2 HoveredTilePixelPosition;
    private readonly Point defaultLevelSize = new Point(20, 15);
    private bool drawingBlocks;
    private bool hoveringAnyButton;
    private Vector2 toMove;
    private const float scrollSpeed = 500;

    private Texture2D wallTexture;

    public LevelEditorState()
    {
        // fill empty level
        levelDescription = "click to change";
        level = new char[defaultLevelSize.X, defaultLevelSize.Y];
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                level[x, y] = '.';
            }
        }
        
        // add a "save" button
        saveButton = new Button("Sprites/UI/spr_button_editor", 1);// temp
        saveButton.LocalPosition = new Vector2(20, 20);
        gameObjects.AddChild(saveButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(1290, 20);
        gameObjects.AddChild(quitButton);

        wallTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/Tiles/spr_wall");
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        offset += toMove * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Debug.WriteLine($"{offset.X}, {offset.Y}");
        
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);
        Vector2 mousePos = inputHelper.MousePositionWorld;
        
        Vector2 absoluteMousePos = mousePos - offset;
        

        hoveringAnyButton = false;
        if (quitButton.Hovered || saveButton.Hovered)
            hoveringAnyButton = true;
        
        if (inputHelper.MouseLeftButtonPressed() && !hoveringAnyButton)
            drawingBlocks = true;
        else if (!inputHelper.MouseLeftButtonDown())
            drawingBlocks = false;

        hoveredTile = new Point((int)MathF.Floor(absoluteMousePos.X / Level.TileWidth),
            (int)MathF.Floor(absoluteMousePos.Y / Level.TileHeight));
        HoveredTilePixelPosition = new Vector2(hoveredTile.X * Level.TileWidth + offset.X,
            hoveredTile.Y * Level.TileHeight + offset.Y);

        if (drawingBlocks && !hoveringAnyButton)
            PlaceTile(hoveredTile, '#');
        
        if (quitButton.Pressed)
            TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);

        toMove = Vector2.Zero;
        if (inputHelper.KeyDown(Keys.Left))
            toMove.X += scrollSpeed;
        if (inputHelper.KeyDown(Keys.Right))
            toMove.X += -scrollSpeed;
        if (inputHelper.KeyDown(Keys.Up))
            toMove.Y += scrollSpeed;
        if (inputHelper.KeyDown(Keys.Down))
            toMove.Y += -scrollSpeed;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        base.Draw(gameTime, spriteBatch, opacity);

        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                Rectangle destRectangle = new Rectangle((int)MathF.Round(x * Level.TileWidth + offset.X),
                    (int)MathF.Round(y * Level.TileHeight + offset.Y), Level.TileWidth, Level.TileHeight);
                Texture2D textureToDraw = null;
                switch (level[x, y])
                {
                    case '#':
                        textureToDraw = wallTexture;
                        break;
                }
                if (textureToDraw != null)
                    spriteBatch.Draw(textureToDraw, destRectangle, Color.White);
            }
        }

        if (hoveringAnyButton)
            return;
        
        // draw preview of curren item at cursor
        Rectangle uiDestRectangle = new Rectangle((int)MathF.Round(HoveredTilePixelPosition.X),
            (int)MathF.Round(HoveredTilePixelPosition.Y), Level.TileWidth, Level.TileHeight);
        spriteBatch.Draw(wallTexture, uiDestRectangle, Color.White * 0.5f);
    }

    private void PlaceTile(Point point, char tile)
    {
        if (point.X < 0)
        {
            ResizeLevelTopLeft(new Point(level.GetLength(0) - point.X, level.GetLength(1)));
            point.X = 0;
        }
        if (point.Y < 0)
        {
            ResizeLevelTopLeft(new Point(level.GetLength(0), level.GetLength(1) - point.Y));
            point.Y = 0;
        }
        if (point.X >= level.GetLength(0))
        {
            ResizeLevelBottomRight(new Point(point.X + 1, level.GetLength(1)));
        }
        if (point.Y >= level.GetLength(1))
        {
            ResizeLevelBottomRight(new Point(level.GetLength(0), point.Y + 1));
        }

        level[point.X, point.Y] = tile;
    }

    private void TrimLevel()
    {
        Point newSize = new Point(level.GetLength(0), level.GetLength(1));
        for (int x = defaultLevelSize.X; x < level.GetLength(0); x++)
        {
            if (ColumnHasNoItems(x))
                newSize.X = x;
            else
                break;
        }
        
        for (int y = defaultLevelSize.Y; y < level.GetLength(1); y++)
        {
            if (RowHasNoItems(y))
                newSize.Y = y;
            else
                break;
        }
        if (newSize != new Point(level.GetLength(0), level.GetLength(1)))
            ResizeLevelBottomRight(newSize);
    }

    private bool RowHasNoItems(int row)
    {
        for (int y = 0; y < level.GetLength(1); y++)
        {
            if (level[row, y] is not ' ' or '.')
            {
                return false;
            }
        }
        return true;
    }
    
    private bool ColumnHasNoItems(int column)
    {
        for (int x = 0; x < level.GetLength(0); x++)
        {
            if (level[x, column] is not ' ' or '.')
            {
                return false;
            }
        }
        return true;
    }

    private void ResizeLevelBottomRight(Point newSize)
    {
        // copy the old level
        char[,] newLevel = new char[newSize.X, newSize.Y];
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                newLevel[x,y] = level[x, y];
            }
        }
        // fill the new level with empty space
        for (int x = level.GetLength(0); x < newSize.X; x++)
        {
            for (int y = 0; y < newSize.Y; y++)
            {
                newLevel[x,y] = '-';
            }
        }
        

        level = newLevel;
    }
    
    private void ResizeLevelTopLeft(Point newSize)
    {
        char[,] newLevel = new char[newSize.X, newSize.Y];
        Point difference = newSize - new Point(level.GetLength(0), level.GetLength(1));
        offset -= difference.ToVector2() * new Vector2(Level.TileWidth, Level.TileHeight);
        // copy the old level to the new level
        for (int x = difference.X; x < newSize.X; x++)
        {
            for (int y = difference.Y; y < newSize.Y; y++)
            {
                newLevel[x,y] = level[x - difference.X, y - difference.Y];
            }
        }
        
        for (int x = 0; x < difference.X; x++)
        {
            for (int y = 0; y < difference.Y; y++)
            {
                newLevel[x,y] = '-';
            }
        }
        

        level = newLevel;
    }
}