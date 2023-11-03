using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameStates;

public class LevelEditorState : GameState
{
    private EditorUI editorUI;
    
    private Button saveButton;
    private Button quitButton;
    private Vector2 offset = Vector2.Zero;
    private string levelDescription;
    private char[,] level;
    private Point hoveredTile;
    private Vector2 HoveredTilePixelPosition;
    private readonly Point defaultLevelSize = new Point(20, 15);
    private bool drawingBlocks;
    private bool erasingBlocks;
    public bool hoveringAnyButton;
    private Vector2 toMove;
    private const float scrollSpeed = 500;
    public char selectedTile = '#';

    private TypebleButton test;
    
    // TEMP!!!
    private char[] textureKeysTemp;
    private int currenItemIndexTemp = 0;
    

    private Dictionary<char, Texture2D> textures = new Dictionary<char, Texture2D>();

    private Texture2D wallTexture;

    public LevelEditorState()
    {
        Camera.position = Vector2.Zero;
        editorUI = new EditorUI(gameObjects, this);
        
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
        
        test = new TypebleButton("Sprites/UI/spr_frame_text", 0.9f, "test", "Fonts/MainFont");
        test.LocalPosition = new Vector2(300, 50);
        test.fixedWidth = 500;
        test.Reset();
        gameObjects.AddChild(test);
        
        // add a "save" button
        saveButton = new Button("Sprites/UI/spr_button_editor", 1);// temp
        saveButton.LocalPosition = new Vector2(20, 20);
        gameObjects.AddChild(saveButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(1290, 20);
        gameObjects.AddChild(quitButton);

        Texture2D GetSprite(string path) => ExtendedGame.AssetManager.LoadSprite(path);
        
        textures.Add('-', GetSprite("Sprites/Tiles/spr_platform"));
        textures.Add('#', GetSprite("Sprites/Tiles/spr_wall"));
        textures.Add('h', GetSprite("Sprites/Tiles/spr_platform_hot"));
        textures.Add('H', GetSprite("Sprites/Tiles/spr_wall_hot"));
        textures.Add('i', GetSprite("Sprites/Tiles/spr_platform_ice"));
        textures.Add('I', GetSprite("Sprites/Tiles/spr_wall_ice"));
        textures.Add('d', GetSprite("Sprites/Tiles/spr_platform_speed"));
        textures.Add('D', GetSprite("Sprites/Tiles/spr_wall_speed"));
        textures.Add('1', GetSprite("Sprites/LevelObjects/Player/spr_idle"));
        textures.Add('X', GetSprite("Sprites/LevelObjects/spr_goal"));
        textures.Add('W', GetSprite("Sprites/LevelObjects/spr_water"));
        textures.Add('R', GetSprite("Sprites/LevelObjects/Rocket/spr_rocket_editor"));
        textures.Add('T', GetSprite("Sprites/LevelObjects/Turtle/spr_idle"));
        textures.Add('S', GetSprite("Sprites/LevelObjects/Sparky/spr_electrocute"));
        textures.Add('A', GetSprite("Sprites/LevelObjects/Flame/spr_flame_editor"));
        textures.Add('B', GetSprite("Sprites/LevelObjects/Flame/spr_flame_blue_editor"));
        textures.Add('C', GetSprite("Sprites/LevelObjects/Flame/spr_flame_green_editor"));

        textureKeysTemp = textures.Keys.ToArray();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        editorUI.Update(gameTime);
        
        offset += toMove * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        //TEMP!!!!!
        if (inputHelper.KeyPressed(Keys.W))
        {
            currenItemIndexTemp++;
            if (currenItemIndexTemp >= textureKeysTemp.Length)
                currenItemIndexTemp = 0;
            selectedTile = textureKeysTemp[currenItemIndexTemp];
        }
        
        if (inputHelper.KeyPressed(Keys.S))
        {
            currenItemIndexTemp--;
            if (currenItemIndexTemp < 0)
                currenItemIndexTemp = textureKeysTemp.Length - 1;
            selectedTile = textureKeysTemp[currenItemIndexTemp];
        }

        if (inputHelper.KeyPressed(Keys.P))
        {
            Play();
            return;
        }
        //End temp
        
        base.HandleInput(inputHelper);

        Vector2 mousePos = inputHelper.MousePositionWorld;
        
        Vector2 absoluteMousePos = mousePos - offset;
        

        hoveringAnyButton = false;
        editorUI.HandleInput();
        
        if (quitButton.Hovered || saveButton.Hovered)
            hoveringAnyButton = true;
        
        if (inputHelper.MouseRightButtonPressed() && !hoveringAnyButton)
            erasingBlocks = true;
        else if (!inputHelper.MouseRightButtonDown())
            erasingBlocks = false;

        if (inputHelper.MouseLeftButtonPressed() && !hoveringAnyButton)
        {
            drawingBlocks = true;
            erasingBlocks = false;
        }
        else if (!inputHelper.MouseLeftButtonDown())
            drawingBlocks = false;

        hoveredTile = new Point((int)MathF.Floor(absoluteMousePos.X / Level.TileWidth),
            (int)MathF.Floor(absoluteMousePos.Y / Level.TileHeight));
        HoveredTilePixelPosition = new Vector2(hoveredTile.X * Level.TileWidth + offset.X,
            hoveredTile.Y * Level.TileHeight + offset.Y);

        if (drawingBlocks && !hoveringAnyButton)
            PlaceTile(hoveredTile, selectedTile);
        else if (erasingBlocks && !hoveringAnyButton)
            PlaceTile(hoveredTile, '.');
        
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
        editorUI.Draw(spriteBatch);

        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                Rectangle destRectangle = new Rectangle((int)MathF.Round(x * Level.TileWidth + offset.X),
                    (int)MathF.Round(y * Level.TileHeight + offset.Y), Level.TileWidth, Level.TileHeight);
                Texture2D textureToDraw = null;
                if (textures.ContainsKey(level[x,y]))
                    textureToDraw = textures[level[x,y]];
                if (textureToDraw != null)
                    spriteBatch.Draw(textureToDraw, destRectangle, Color.White);
            }
        }

        if (hoveringAnyButton)
            return;
        
        // draw preview of curren item at cursor
        Rectangle uiDestRectangle = new Rectangle((int)MathF.Round(HoveredTilePixelPosition.X),
            (int)MathF.Round(HoveredTilePixelPosition.Y), Level.TileWidth, Level.TileHeight);
        spriteBatch.Draw(textures[selectedTile], uiDestRectangle, Color.White * 0.5f);
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
        TrimInXDirection();
        TrimInYDirection();

    }

    private void TrimInYDirection()
    {
        // check from the top
        while (RowHasNoItems(0))
            RemoveRow(0);
        
        // check from the bottom
        while (RowHasNoItems(level.GetLength(1) - 1))
            RemoveRow(level.GetLength(1) - 1);
    }
    
    private void TrimInXDirection()
    {
        // check from the left
        while (ColumnHasNoItems(0))
            RemoveColumn(0);
        
        // check from the right
        while (ColumnHasNoItems(level.GetLength(0) - 1))
            RemoveColumn(level.GetLength(0) - 1);
    }

    private void RemoveColumn(int columnToRemove)
    {
        char[,] newLevel = new char[level.GetLength(0) - 1, level.GetLength(1)];
        int newLevelColumn = 0;
        for (int oldLevelColumn = 0; oldLevelColumn < level.GetLength(0); oldLevelColumn++)
        {
            if (oldLevelColumn == columnToRemove)
                continue;
            for (int y = 0; y < level.GetLength(1); y++)
            {
                newLevel[newLevelColumn, y] = level[oldLevelColumn, y];
            }

            newLevelColumn++;
        }

        level = newLevel;
    }

    private void RemoveRow(int rowToRemove)
    {
        char[,] newLevel = new char[level.GetLength(0), level.GetLength(1) - 1];
        int newLevelRow = 0;
        for (int oldLevelRow = 0; oldLevelRow < level.GetLength(1); oldLevelRow++)
        {
            if (oldLevelRow == rowToRemove)
                continue;
            for (int x = 0; x < level.GetLength(0); x++)
            {
                newLevel[x, newLevelRow] = level[x, oldLevelRow];
            }

            newLevelRow++;

        }
        
        level = newLevel;
    }

    private bool ColumnHasNoItems(int column)
    {
        for (int y = 0; y < level.GetLength(1); y++)
        {
            if (!(level[column, y] is ' ' or '.'))
            {
                return false;
            }
        }
        return true;
    }
    
    private bool RowHasNoItems(int row)
    {
        for (int x = 0; x < level.GetLength(0); x++)
        {
            if (!(level[x, row] is ' ' or '.'))
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
                newLevel[x,y] = '.';
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
            for (int y = 0; y < level.GetLength(1); y++)
            {
                newLevel[x,y] = '.';
            }
        }
        
        for (int y = 0; y < difference.Y; y++)
        {
            for (int x = 0; x < level.GetLength(0); x++)
            {
                newLevel[x,y] = '.';
            }
        }
        

        level = newLevel;
    }

    private void Play()
    {
        TrimLevel();
        TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Playing);
        ExtendedGameWithLevels.GetPlayingState().LoadLevelFromString(levelAsString);
    }

    private string levelAsString
    {
        get
        {
            string outString = $"{levelDescription}\n30\n";
            for (int y = 0; y < level.GetLength(1); y++)
            {
                for (int x = 0; x < level.GetLength(0); x++)
                {
                    outString += level[x, y];
                }

                outString += '\n';
            }
            Debug.WriteLine(outString);
            return outString;
        }
    }

}