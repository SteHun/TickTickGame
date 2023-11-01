using System;
using System.Diagnostics;
using Engine;
using Engine.UI;
using GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// EditorUI -> EditorDock
/// The UI that holds all the GameObjects that can be placed
/// </summary>

public class EditorDock
{
    private EditorUI editorUI;
    private LevelEditorState editor;
    private SelectedTab selectedTab = SelectedTab.None, previousSelectedTab;

    //Not an actual textbox, used as a makeshift background for group tabs
    private TextBox tabBackground, groupBackground; //groupBackground is a temp sprite for clarity
    
    //The group buttons
    private Button wallGroupButton;
    
    //Wall group buttons
    private Button selectWallButton, selectIceWallButton, selectHotWallButton, selectSpeedWallButton;

    private const string wallFileName = "Sprites/Tiles/spr_wall";
    private const string wallIceFileName = "Sprites/Tiles/spr_wall_ice";
    private const string wallHotFileName = "Sprites/Tiles/spr_wall_hot";
    private const string wallSpeedFileName = "Sprites/Tiles/spr_wall_speed";

    public EditorDock(EditorUI editorUI, LevelEditorState editor)
    {
        this.editorUI = editorUI;
        this.editor = editor;
        
        //Group buttons
        wallGroupButton = new Button(wallFileName, 1);
        wallGroupButton.LocalPosition = new Vector2(50, 600);
        this.editorUI.gameObjects.AddChild(wallGroupButton);

        //Group tab background (doesn't hold text)
        tabBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        tabBackground.LocalPosition = new Vector2(30, 495);
        this.editorUI.gameObjects.AddChild(tabBackground);
        tabBackground.Visible = false;
        
        groupBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        groupBackground.LocalPosition = new Vector2(30, 595);
        groupBackground.text = "           ";
        this.editorUI.gameObjects.AddChild(groupBackground);

        int horizontalTabSpacing = 75;
        
        //Wall group
        selectWallButton = CreateButton(wallFileName, new Vector2(50, 500));
        selectIceWallButton = CreateButton(wallIceFileName, new Vector2(50+horizontalTabSpacing, 500));
        selectHotWallButton = CreateButton(wallHotFileName, new Vector2(50+horizontalTabSpacing*2, 500));
        selectSpeedWallButton = CreateButton(wallSpeedFileName, new Vector2(50+horizontalTabSpacing*3, 500));
    }

    private Button CreateButton(string fileName, Vector2 position)
    {
        Button button = new Button(fileName, 1);
        button.LocalPosition = position;
        editorUI.gameObjects.AddChild(button);
        button.Visible = false;

        return button;
    }
    
    public void HandleInput()
    {
        if (wallGroupButton.Pressed)
        {
            selectedTab = selectedTab != SelectedTab.Walls ? SelectedTab.Walls : SelectedTab.None;
        }

        if (selectWallButton.Pressed)
        {
            editor.selectedTile = '#';
        }
        else if (selectIceWallButton.Pressed)
        {
            editor.selectedTile = 'I';
        }
        else if (selectHotWallButton.Pressed)
        {
            editor.selectedTile = 'H';
        }
        if (selectSpeedWallButton.Pressed)
        {
            editor.selectedTile = 'D';
        }
        
        if(CheckAnyButtonHovered())
        {
            editor.hoveringAnyButton = true;
        }
    }

    public void Update(GameTime gameTime)
    {
        //Check if a new tab is entered.
        if (previousSelectedTab == selectedTab)
        {
            return;
        }
        
        MakeAllTabsInvisible();
        
        string defaultSpace = "           "; //Used for making an empty box
        switch (selectedTab)
        {
            case SelectedTab.Walls:
                //Draw all available walls
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace + defaultSpace;
                tabBackground.Visible = true;
                selectWallButton.Visible = true;
                selectIceWallButton.Visible = true;
                selectHotWallButton.Visible = true;
                selectSpeedWallButton.Visible = true;
                break;
        }
        
        previousSelectedTab = selectedTab;
    }

    private void MakeAllTabsInvisible()
    {
        tabBackground.Visible = false;
        selectWallButton.Visible = false;
        selectIceWallButton.Visible = false;
        selectHotWallButton.Visible = false;
        selectSpeedWallButton.Visible = false;
    }

    private bool CheckAnyButtonHovered()
    {
        //Check group buttons
        if (wallGroupButton.Hovered)
            return true;

        //Check wall group tab
        if (selectWallButton.Hovered || selectIceWallButton.Hovered || selectHotWallButton.Hovered ||
            selectSpeedWallButton.Hovered)
            return true;

        return false;
    }
    
    private enum SelectedTab
    {
        None,
        Walls,
    }
}