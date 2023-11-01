using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// EditorUI -> EditorDock
/// The UI that holds all the GameObjects that can be placed
/// </summary>

public class EditorDock
{
    private EditorUI editorUI;
    private SelectedTab selectedTab = SelectedTab.None;
    
    //The group buttons
    private Button wallGroupButton;
    
    //Wall group buttons
    private Button selectWallButton;
    private Button selectIceWallButton;
    private Button selectHotWallButton;
    private Button selectSpeedWallButton;

    private const string wallFileName = "Sprites/Tiles/spr_wall";
    private const string wallIceFileName = "Sprites/Tiles/spr_wall_ice";
    private const string wallHotFileName = "Sprites/Tiles/spr_wall_hot";
    private const string wallSpeedFileName = "Sprites/Tiles/spr_wall_speed";

    public EditorDock(EditorUI editorUI)
    {
        this.editorUI = editorUI;
        
        wallGroupButton = new Button(wallFileName, 1);
        wallGroupButton.LocalPosition = new Vector2(50, 500);
        this.editorUI.gameObjects.AddChild(wallGroupButton);
        wallGroupButton.Visible = false; //Temp
        
        selectWallButton = new Button(wallFileName, 1);
        selectWallButton.LocalPosition = new Vector2(50, 500);
        this.editorUI.gameObjects.AddChild(selectWallButton);
        selectWallButton.Visible = false;
    }
    
    public void HandleInput()
    {
        if (wallGroupButton.Pressed)
        {
            selectedTab = SelectedTab.Walls;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        switch (selectedTab)
        {
            case SelectedTab.Walls:
                //Draw all available walls
                break;
        }
    }
    
    private enum SelectedTab
    {
        None,
        Walls,
    }
}