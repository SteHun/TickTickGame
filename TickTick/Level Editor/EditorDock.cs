using System;
using System.Collections.Generic;
using System.Transactions;
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
    private Button wallGroupButton, platformGroupButton, playerGroupButton, enemyGroupButton;
    
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
    
    //Player group
    private Button selectPlayerButton, selectWaterButton, selectGoalButton;
    private const string playerFileName = "Sprites/LevelObjects/Player/spr_idle";
    private const string waterFileName = "Sprites/LevelObjects/spr_water";
    private const string goalFileName = "Sprites/LevelObjects/spr_goal";
    
    //Enemy group
    private Button selectFlameButton,
        selectBlueFlameButton,
        selectGreenFlameButton,
        selectRocketButton,
        selectSparkyButton,
        selectTurtleButton;
    private const string flameFileName = "Sprites/LevelObjects/Flame/spr_flame_editor";
    private const string blueFlameFileName = "Sprites/LevelObjects/Flame/spr_flame_blue_editor";
    private const string greenFlameFileName = "Sprites/LevelObjects/Flame/spr_flame_green_editor";
    private const string rocketFileName = "Sprites/LevelObjects/Rocket/spr_rocket_editor";
    private const string sparkyFileName = "Sprites/LevelObjects/Sparky/spr_editor";
    private const string turtleFileName = "Sprites/LevelObjects/Turtle/spr_editor";

    //Setup everything
    public EditorDock(EditorUI editorUI, LevelEditorState editor)
    {
        this.editorUI = editorUI;
        this.editor = editor;
        
        //Group buttons
        wallGroupButton = CreateButton(wallFileName, new Vector2(50, 600), true);
        platformGroupButton = CreateButton(platformFileName, new Vector2(120, 600), true);
        playerGroupButton = CreateButton(playerFileName, new Vector2(210, 610), true);
        enemyGroupButton = CreateButton(flameFileName, new Vector2(290, 610), true);

        //Group tab background (doesn't hold text)
        tabBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        tabBackground.LocalPosition = new Vector2(30, 495);
        this.editorUI.gameObjects.AddChild(tabBackground);
        tabBackground.Visible = false;
        
        groupBackground = new TextBox("Sprites/UI/spr_frame_text", 0.9f, "", "Fonts/HintFont");
        groupBackground.LocalPosition = new Vector2(30, 595);
        string spacing = "           ";
        groupBackground.text =  spacing + spacing + spacing + spacing;
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

        //Player group
        selectPlayerButton = CreateButton(playerFileName, new Vector2(190, 500));
        selectWaterButton = CreateButton(waterFileName, new Vector2(190+horizontalTabSpacing, 500));
        selectGoalButton = CreateButton(goalFileName, new Vector2(190+horizontalTabSpacing*2, 500));
        
        //Enemy group
        selectFlameButton = CreateButton(flameFileName, new Vector2(200, 500));
        selectBlueFlameButton = CreateButton(blueFlameFileName, new Vector2(200+horizontalTabSpacing, 500));
        selectGreenFlameButton = CreateButton(greenFlameFileName, new Vector2(200+horizontalTabSpacing*2, 500));
        selectRocketButton = CreateButton(rocketFileName, new Vector2(200+horizontalTabSpacing*3, 500));
        selectSparkyButton = CreateButton(sparkyFileName, new Vector2(200+horizontalTabSpacing*4, 500));
        selectTurtleButton = CreateButton(turtleFileName, new Vector2(200+horizontalTabSpacing*5, 520));
    }

    //All buttons have same values, so this makes it easier
    private Button CreateButton(string fileName, Vector2 position, bool visible = false)
    {
        Button button = new Button(fileName, 1);
        button.LocalPosition = position;
        float largestSize = MathF.Max(button.sprite.Width, button.sprite.Height);
        button.sprite.Scale = Level.TileWidth/largestSize;
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
        if(CheckAnyButtonHovered())
        {
            editor.hoveringAnyButton = true;
        }
        
        //Change selected tab
        #region Group buttons check
        if (wallGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Walls ? SelectedTab.Walls : SelectedTab.None;
        if(platformGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Platforms ? SelectedTab.Platforms : SelectedTab.None;
        if(playerGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Player ? SelectedTab.Player : SelectedTab.None;
        if(enemyGroupButton.Pressed)
            selectedTab = selectedTab != SelectedTab.Enemies ? SelectedTab.Enemies : SelectedTab.None;
        #endregion

        //Change selected block
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
        
        #region Player group
        if (selectPlayerButton.Pressed)
            editor.selectedTile = '1';
        else if (selectWaterButton.Pressed)
            editor.selectedTile = 'W';
        else if (selectGoalButton.Pressed)
            editor.selectedTile = 'X';
        #endregion
        
        #region Enemy group
        if (selectFlameButton.Pressed)
            editor.selectedTile = 'A';
        else if (selectBlueFlameButton.Pressed)
            editor.selectedTile = 'B';
        else if (selectGreenFlameButton.Pressed)
            editor.selectedTile = 'C';
        else if (selectRocketButton.Pressed)
            editor.selectedTile = 'R';
        else if (selectSparkyButton.Pressed)
            editor.selectedTile = 'S';
        else if (selectTurtleButton.Pressed)
            editor.selectedTile = 'T';
        #endregion

        foreach (var button in hideableButtons)
        {
            if (button.Pressed)
            {
                CloseTabAfterSelecting();
            }
        }
        #endregion
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
                //Set all available walls to visible
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace + defaultSpace;
                tabBackground.LocalPosition = new Vector2(30, 495);
                tabBackground.Visible = true;
                selectWallButton.Visible = true;
                selectIceWallButton.Visible = true;
                selectHotWallButton.Visible = true;
                selectSpeedWallButton.Visible = true;
                break;
            case SelectedTab.Platforms:
                //Set all available platforms to visible
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace + defaultSpace;
                tabBackground.LocalPosition = new Vector2(100, 495);
                tabBackground.Visible = true;
                selectPlatformButton.Visible = true;
                selectIcePlatformButton.Visible = true;
                selectHotPlatformButton.Visible = true;
                selectSpeedPlatformButton.Visible = true;
                break;
            case SelectedTab.Player:
                //Set all available player related buttons to visible
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace;
                tabBackground.LocalPosition = new Vector2(170, 495);
                tabBackground.Visible = true;
                selectPlayerButton.Visible = true;
                selectWaterButton.Visible = true;
                selectGoalButton.Visible = true;
                break;
            case SelectedTab.Enemies:
                //Set all available enemy related buttons to visible
                tabBackground.text = defaultSpace + defaultSpace + defaultSpace + defaultSpace + defaultSpace + defaultSpace;
                tabBackground.LocalPosition = new Vector2(170, 495);
                tabBackground.Visible = true;
                selectFlameButton.Visible = true;
                selectBlueFlameButton.Visible = true;
                selectGreenFlameButton.Visible = true;
                selectRocketButton.Visible = true;
                selectSparkyButton.Visible = true;
                selectTurtleButton.Visible = true;
                break;
        }
        
        previousSelectedTab = selectedTab;
    }

    //Close all tabs
    private void CloseTabAfterSelecting()
    {
        selectedTab = SelectedTab.None;
        MakeAllTabsInvisible();
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
        if (wallGroupButton.Hovered || platformGroupButton.Hovered || playerGroupButton.Hovered || enemyGroupButton.Hovered)
            return true;

        //Check wall group tab
        if (selectWallButton.Hovered || selectIceWallButton.Hovered || selectHotWallButton.Hovered ||
            selectSpeedWallButton.Hovered)
            return true;

        //Check platform group tab
        if (selectPlatformButton.Hovered || selectIcePlatformButton.Hovered || selectHotPlatformButton.Hovered ||
            selectSpeedPlatformButton.Hovered)
            return true;
        
        //Check player group tab
        if (selectPlayerButton.Hovered || selectWaterButton.Hovered || selectGoalButton.Hovered)
            return true;
        
        //Check enemy group tab
        if (selectFlameButton.Hovered || selectBlueFlameButton.Hovered || selectGreenFlameButton.Hovered ||
            selectRocketButton.Hovered || selectSparkyButton.Hovered || selectTurtleButton.Hovered)
            return true;

        return false;
    }
    
    private enum SelectedTab
    {
        None,
        Walls,
        Platforms,
        Player,
        Enemies,
    }
}