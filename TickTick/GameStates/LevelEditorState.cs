using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameStates;

public class LevelEditorState : GameState
{
    private const string customLevelPath = "Content/CustomLevels";
    
    private EditorUI editorUI;
    
    private Vector2 offset = Vector2.Zero;
    public string levelDescription;
    public int LevelTimer = 30;
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
    
    private Dictionary<char, Texture2D> textures = new Dictionary<char, Texture2D>();
    
    public LevelEditorState()
    {
        Camera.position = Vector2.Zero;
        editorUI = new EditorUI(gameObjects, this);
        
        // fill empty level
        levelDescription = EditorHUD.DefaultDescription;
        level = new char[defaultLevelSize.X, defaultLevelSize.Y];
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                if (y == level.GetLength(1) - 1)
                {
                    level[x, y] = '#';
                }
                else
                {
                    level[x, y] = '.';
                }
            }
        }

        level[0, defaultLevelSize.Y - 2] = '1';
        level[defaultLevelSize.X - 1, defaultLevelSize.Y - 2] = 'X';
        
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
        textures.Add('T', GetSprite("Sprites/LevelObjects/Turtle/spr_editor"));
        textures.Add('S', GetSprite("Sprites/LevelObjects/Sparky/spr_editor"));
        textures.Add('A', GetSprite("Sprites/LevelObjects/Flame/spr_flame_editor"));
        textures.Add('B', GetSprite("Sprites/LevelObjects/Flame/spr_flame_blue_editor"));
        textures.Add('C', GetSprite("Sprites/LevelObjects/Flame/spr_flame_green_editor"));

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        editorUI.Update(gameTime);
        
        offset += toMove * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);

        Vector2 mousePos = inputHelper.MousePositionWorld;
        
        Vector2 absoluteMousePos = mousePos - offset;
        

        hoveringAnyButton = false;
        editorUI.HandleInput();
        
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
            if(hoveredTile.X >= 0 && hoveredTile.X < level.GetLength(0) && hoveredTile.Y >= 0 
               && hoveredTile.Y < level.GetLength(1) 
               && level[hoveredTile.X, hoveredTile.Y] is not ('1' or 'X'))
                PlaceTile(hoveredTile, '.');

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

        if (tile is '1' or 'X') //Player
        {
            RemoveDuplicates(tile);
        }

        if (level[point.X, point.Y] is '1' or 'X')
        {
            PlaceTile(new Point(point.X, point.Y-1), level[point.X, point.Y]);
        }
        level[point.X, point.Y] = tile;
    }

    private void RemoveDuplicates(char tile)
    {
        for (int i = 0; i < level.GetLength(0); i++)
        {
            for (int j = 0; j < level.GetLength(1); j++)
            {
                if (level[i, j] == tile)
                {
                    level[i, j] = '.';
                }
            }
        }
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
        char[,] newLevel = new char[newSize.X, newSize.Y];
        for (int x = 0; x < newSize.X; x++)
        {
            for (int y = 0; y < newSize.Y; y++)
            {
                newLevel[x,y] = '.';
            }
        }
        
        // copy the old level
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                newLevel[x,y] = level[x, y];
            }
        }
        

        level = newLevel;
    }
    
    private void ResizeLevelTopLeft(Point newSize)
    {
        char[,] newLevel = new char[newSize.X, newSize.Y];
        
        for (int x = 0; x < newSize.X; x++)
        {
            for (int y = 0; y < newSize.Y; y++)
            {
                newLevel[x,y] = '.';
            }
        }
        
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
        

        level = newLevel;
    }

    public void Play()
    {
        if (!LevelIsValid(level))
            return;
        TrimLevel();
        
        string levelString = GetLevelAsString();
        TickTick.previousStatePlaying = ExtendedGameWithLevels.StateName_Editor;
        TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Playing);
        ExtendedGameWithLevels.GetPlayingState().LoadLevelFromString(levelString);
    }
    
    public static bool LevelIsValid(string level)
    {
        // check if the level has at least three newlines
        string[] lines = level.Split('\n');
        

        if (lines.Length < 3)
            return false;
        
        // check if time is parsable
        if (!int.TryParse(lines[1], out _)) // discard the result, we only need to know is it is parsable
        {
            return false;
        }
        
        
        // check if there is exactly one player and one goal
        bool onePlayerFound = false;
        bool oneGoalFound = false;
        for (int y = 2; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '1')
                {
                    if (onePlayerFound)
                        return false;
                    onePlayerFound = true;
                    continue;
                }

                if (lines[y][x] == 'X')
                {
                    if (oneGoalFound)
                        return false;
                    oneGoalFound = true;
                }
            }
        }

        return oneGoalFound && onePlayerFound;
    }
    
    private static bool LevelIsValid(char[,] level)
    {
        bool onePlayerFound = false;
        bool oneGoalFound = false;
        foreach (char item in level)
        {
            if (item == '1')
            {
                if (onePlayerFound)
                    return false;
                onePlayerFound = true;
                continue;
            }

            if (item == 'X')
            {
                if (oneGoalFound)
                    return false;
                oneGoalFound = true;
            }
        }

        return oneGoalFound && onePlayerFound;
    }


    public void LoadLevelFromFile(string name)
    {
        string[] levelAsText = File.ReadAllLines($"{customLevelPath}/{name}");
        levelDescription = levelAsText[0];
        editorUI.hud.nameButton.SetText(name.Remove(name.Length-4,4));
        if (!int.TryParse(levelAsText[1], out LevelTimer))
        {
            Debug.WriteLine("Error: invalid level file");
            return;
        }
        
        // find the longest row
        int longestRow = 0;
        foreach (string row in levelAsText)
        {
            longestRow = MathHelper.Max(longestRow, row.Length);
        }

        char[,] newLevel = new char[longestRow, levelAsText.Length];
        for (int y = 2; y < levelAsText.Length; y++)
        {
            for (int x = 0; x < longestRow; x++)
            {
                if (x >= levelAsText[y].Length)
                    newLevel[x, y] = '.';
                else
                    newLevel[x, y] = levelAsText[y][x];
            }
        }

        level = newLevel;
    }
    
    public void SaveLevelToFile(string name)
    {
        TrimLevel();
        if (!LevelIsValid(level))
            return;
        offset = Vector2.Zero;
        if (!Directory.Exists(customLevelPath))
            Directory.CreateDirectory(customLevelPath);
        File.WriteAllText($"{customLevelPath}/{name}.txt", GetLevelAsString());
    }

    public string GetLevelAsString()
    {
        string outString = $"{levelDescription}\n{LevelTimer}\n";
        for (int y = 0; y < level.GetLength(1); y++)
        {
            for (int x = 0; x < level.GetLength(0); x++)
            {
                outString += level[x, y];
            }

            outString += '\n';
        }

        outString = outString[..^1]; // removes the last \n characer
        Debug.WriteLine(outString);
        return outString;
    }
    


}