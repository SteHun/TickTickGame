using System;
using System.Collections.Generic;
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
    private List<Button> hideableButtons = new();

    //Not an actual textbox, used as a makeshift background for group tabs
    private TextBox tabBackground, groupBackground; //groupBackground is a temp sprite for clarity
    
    //The group buttons
    private Button wallGroupButton;
    private Button platformGroupButton;
    
    //Wall group
    private Button selectWallButton, selectIceWallButton, selectHotWallButton, selectSpeedWallButton;
    private const string wallFileName = "Sprites/Tiles/spr_wall";
    private const string wallIceFileName = "Sprites/Tiles/spr_wall_ice";
    private const string wallHotFileName = "Sprites/Tiles/spr_wall_hot";
    private const string wallSpeedFileName = "Sprites/Tiles/spr_wall_speed";
    
    //Platform group
    private Button selectPlatformButton, selectIcePlatformButton, selectHotPlatformButton, selectSpeedPlatformButton;
    private const string platformFileName = "Sprites/Tiles/spr_platform";
    private const string platformIceFileName = "Sprites/Tiles/spr_platform_ice";
    private const string platformHotFileName = "Sprites/Tiles/spr_platform_hot";
    private const string platformSpeedFileName = "Sprites/Tiles/spr_platform_speed";

    public EditorDock(EditorUI editorUI, LevelEditorState editor)
    {
        this.editorUI = editorUI;
        this.editor = editor;
        
        //Group buttons
        wallGroupButton = CreateButton(wallFileName, new Vector2(50, 600), true);
        platformGroupButton = CreateButton(platformFileName, new Vector2(120, 600), true);

        //Group tab background (doesn't hold text)
        tabBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        tabBackground.LocalPosition = new Vector2(30, 495);
        this.editorUI.gameObjects.AddChild(tabBackground);
        tabBackground.Visible = false;
        
        groupBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        groupBackground.LocalPosition = new Vector2(30, 595);
        groupBackground.text = "           " + "           ";
        this.editorUI.gameObjects.AddChild(groupBackground);

        int horizontalTabSpacing = 75;
        
        //Wall group
        selectWallButton = CreateButton(wallFileName, new Vector2(50, 500));
        selectIceWallButton = CreateButton(wallIceFileName, new Vector2(50+horizontalTabSpacing, 500));
        selectHotWallButton = CreateButton(wallHotFileName, new Vector2(50+horizontalTabSpacing*2, 500));
        selectSpeedWallButton = CreateButton(wallSpeedFileName, new Vector2(50+horizontalTabSpacing*3, 500));
        
        //Platform group
        selectPlatformButton = CreateButton(platformFileName, new Vector2(120, 500));
        selectIcePlatformButton = CreateButton(platformIceFileName, new Vector2(120+horizontalTabSpacing, 500));
        selectHotPlatformButton = CreateButton(platformHotFileName, new Vector2(120+horizontalTabSpacing*2, 500));
        selectSpeedPlatformButton = CreateButton(platformSpeedFileName, new Vector2(120+horizontalTabSpacing*3, 500));
    }

    private Button CreateButton(string fileName, Vector2 position, bool visible = false)
    {
        Button button = new Button(fileName, 1);
        button.LocalPosition = position;
        editorUI.gameObjects.AddChild(button);
        if (!visible)
        {
            hideableButtons.Add(button);
        }
        button.Visible = visible;

        return button;
    }
    
    public void HandleInput()
    {
        #region Group buttons check
        if (wallGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Walls ? SelectedTab.Walls : SelectedTab.None;
        if(platformGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Platforms ? SelectedTab.Platforms : SelectedTab.None;
        #endregion

        #region Groups

        #region Wall group
        if (selectWallButton.Pressed)
            editor.selectedTile = '#';
        else if (selectIceWallButton.Pressed)
            editor.selectedTile = 'I';
        else if (selectHotWallButton.Pressed)
            editor.selectedTile = 'H';
        else if (selectSpeedWallButton.Pressed)
            editor.selectedTile = 'D';
        #endregion

        #region Platform group
        if (selectPlatformButton.Pressed)
            editor.selectedTile = '-';
        else if (selectIcePlatformButton.Pressed)
            editor.selectedTile = 'i';
        else if (selectHotPlatformButton.Pressed)
            editor.selectedTile = 'h';
        else if (selectSpeedPlatformButton.Pressed)
            editor.selectedTile = 'd';
        #endregion

        #endregion
        
        
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
                tabBackground.LocalPosition = new Vector2(30, 495);
                tabBackground.Visible = true;
                selectWallButton.Visible = true;
                selectIceWallButton.Visible = true;
                selectHotWallButton.Visible = true;
                selectSpeedWallButton.Visible = true;
                break;
            case SelectedTab.Platforms:
                //Draw all available platforms
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace + defaultSpace;
                tabBackground.LocalPosition = new Vector2(100, 495);
                tabBackground.Visible = true;
                selectPlatformButton.Visible = true;
                selectIcePlatformButton.Visible = true;
                selectHotPlatformButton.Visible = true;
                selectSpeedPlatformButton.Visible = true;
                break;
        }
        
        previousSelectedTab = selectedTab;
    }

    private void MakeAllTabsInvisible()
    {
        tabBackground.Visible = false;
        foreach (var button in hideableButtons)
        {
            button.Visible = false;
        }
    }

    private bool CheckAnyButtonHovered()
    {
        //Check group buttons
        if (wallGroupButton.Hovered || platformGroupButton.Hovered)
            return true;

        //Check wall group tab
        if (selectWallButton.Hovered || selectIceWallButton.Hovered || selectHotWallButton.Hovered ||
            selectSpeedWallButton.Hovered)
            return true;

        //Check platform group tab
        if (selectPlatformButton.Hovered || selectIcePlatformButton.Hovered || selectHotPlatformButton.Hovered ||
            selectSpeedPlatformButton.Hovered)
            return true;

        return false;
    }
    
    private enum SelectedTab
    {
        None,
        Walls,
        Platforms,
    }
}