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
    
    //The group buttons
    private Button wallGroupButton;

    private const string wallFileName = "Sprites/Tiles/spr_wall";

    public EditorDock(EditorUI editorUI)
    {
        this.editorUI = editorUI;
        
        /*
        wallGroupButton = new Button(wallFileName, 1);
        wallGroupButton.LocalPosition = new Vector2(50, 500);
        this.editorUI.gameObjects.AddChild(wallGroupButton);
        */
    }
    
    public void HandleInput()
    {
        if (wallGroupButton.Pressed)
        {
            //Show all possible walls to select from
        }
    }
}