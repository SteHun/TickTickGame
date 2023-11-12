﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Engine;
using Engine.UI;
using GameStates;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> LevelMenuState
/// </summary>
class CustomLevelMenuState : GameState
{
    Button backButton, officialLevelButton, nextPageButton, previousPageButton;

    // An array of extra references to the level buttons. 
    // This makes it easier to check if a level button has been pressed.
    Button[] customLevelButtons, editCustomLevelButtons, deleteCustomLevelButtons;
    private bool[] markedForDeletion; //All true are deleted from list when going back to home screen
    private int page = 0;
    private const int levelsPerPage = 7;

    public CustomLevelMenuState()
    {
        Camera.position = Vector2.Zero;
        
        // add a background
        SpriteGameObject background = new SpriteGameObject("Sprites/Backgrounds/spr_levelselect", TickTick.Depth_Background);
        gameObjects.AddChild(background);

        // add a back button
        backButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "BACK", "Fonts/MainFont");
        backButton.LocalPosition = new Vector2(500, 690);
        gameObjects.AddChild(backButton);
        backButton.Reset();
        
        // add a to official/standard level button
        officialLevelButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "OFFICIAL LEVELS", "Fonts/MainFont");
        officialLevelButton.LocalPosition = new Vector2(740, 690);
        gameObjects.AddChild(officialLevelButton);
        officialLevelButton.Reset();
        
        // add a next page button
        nextPageButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "Next page", "Fonts/MainFont");
        nextPageButton.LocalPosition = new Vector2(880, 175);
        gameObjects.AddChild(nextPageButton);
        nextPageButton.Reset();
        
        // add a previous page button
        previousPageButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "Prev page", "Fonts/MainFont");
        previousPageButton.LocalPosition = new Vector2(880, 225);
        gameObjects.AddChild(previousPageButton);
        previousPageButton.Reset();

        // Get all custom level files in the folder
        List<string> directory;
        string sdira= "Content/CustomLevels/";
        directory = Directory.GetFiles(sdira, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();
        
        // check if levels are valid
        for (int i = 0; i < directory.Count; i++)
        {
            try
            {
                string levelContent = File.ReadAllText(sdira + directory[i]);
                if (!LevelEditorState.LevelIsValid(levelContent))
                {
                    directory.RemoveAt(i);
                    i--;
                }
            }
            catch (IOException _)
            {// The files exist, but they could theoretically not have read privileges
                directory.RemoveAt(i);
                i--;
            }
        }

        string[] arrays = directory.ToArray();
        
        

        //Set array size
        customLevelButtons = new Button[arrays.Length];
        editCustomLevelButtons = new Button[arrays.Length];
        deleteCustomLevelButtons = new Button[arrays.Length];
        markedForDeletion = new bool[arrays.Length];
        Vector2 gridOffset = new Vector2(395, 175);
        const int verticalSpace = 20;

        //Place all buttons and give them the correct names
        for (int i = 0; i < arrays.Length; i++)
        {
            // create the button (and give it the correct name)
            string levelName = arrays[i].Remove(arrays[i].Length - 4, 4);
            Button levelButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, levelName, "Fonts/MainFont");
            Button editButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "Edit","Fonts/MainFont");
            Button deleteButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "Delete","Fonts/MainFont");
            
            // give it the correct position
            levelButton.LocalPosition = gridOffset + new Vector2(0, (levelButton.Height + verticalSpace) * (i%levelsPerPage));
            editButton.LocalPosition = levelButton.LocalPosition + new Vector2(320, 0);
            deleteButton.LocalPosition = levelButton.LocalPosition + new Vector2(380, 0);
            
            //Reset tu update button hitbox
            levelButton.Reset();
            editButton.Reset();
            deleteButton.Reset();
            
            // add the button as a child object
            gameObjects.AddChild(levelButton);
            gameObjects.AddChild(editButton);
            gameObjects.AddChild(deleteButton);
            
            // also store it in the array of level buttons
            customLevelButtons[i] = levelButton;
            editCustomLevelButtons[i] = editButton;
            deleteCustomLevelButtons[i] = deleteButton;
        }

        //Hide all levels not on the main page
        if (customLevelButtons.Length <= 6)
            return;
        
        for (int i = 0; i < customLevelButtons.Length; i++)
        {
            if (i < 7) continue;
            customLevelButtons[i].Visible = false;
            editCustomLevelButtons[i].Visible = false;
            deleteCustomLevelButtons[i].Visible = false;
        }
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);

        // if the back button is pressed, go back to the title screen
        if (backButton.Pressed)
        {
            //Delete everything marked for deletion
            for (int i = 0; i < markedForDeletion.Length; i++)
            {
                if (markedForDeletion[i])
                {
                    File.Delete("Content/CustomLevels/" + customLevelButtons[i].Text + ".txt");
                    Console.WriteLine("Delete: " + customLevelButtons[i].Text);
                }
            }
            
            //Go back to title screen
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
        }


        if (officialLevelButton.Pressed)
        {
            //Don't delete anything if going back to normal levels
            for (int i = 0; i < markedForDeletion.Length; i++)
            {
                markedForDeletion[i] = false;
            }
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_LevelSelect);
        }


        if (nextPageButton.Pressed)
        {
            page++;
            page = MathHelper.Clamp(page, 0, customLevelButtons.Length / levelsPerPage);
            UpdatePage();
        }

        if (previousPageButton.Pressed)
        {
            page--;
            page = MathHelper.Clamp(page, 0, customLevelButtons.Length / levelsPerPage);
            UpdatePage();
        }

        // if a (non-locked) level button has been pressed, go to that level
        foreach (Button button in customLevelButtons)
        {
            if (button.Pressed)
            {
                // go to the playing state
                TickTick.previousStatePlaying = ExtendedGameWithLevels.StateName_CustomLevelSelect;
                ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Playing);

                // load the correct level
                ExtendedGameWithLevels.GetPlayingState().LoadCustomLevel(button.Text);

                return;
            }
        }

        //Edit a level
        for (int i = 0; i < editCustomLevelButtons.Length; i++)
        {
            if (editCustomLevelButtons[i].Pressed)
            {
                TickTick.previousStatePlaying = ExtendedGameWithLevels.StateName_CustomLevelSelect;
                ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Editor);
                TickTick.GetEditor().LoadLevelFromFile(customLevelButtons[i].Text + ".txt");
            }
        }
        
        //Delete a level
        for (int i = 0; i < deleteCustomLevelButtons.Length; i++)
        {
            if (deleteCustomLevelButtons[i].Pressed)
            {
                markedForDeletion[i] = !markedForDeletion[i];
                customLevelButtons[i].Color = markedForDeletion[i] ? Color.Red : Color.White;
                deleteCustomLevelButtons[i].SetText(markedForDeletion[i] ? "Keep" : "Delete");
            }
        }
    }

    private void UpdatePage()
    {
        for (int i = 0; i < customLevelButtons.Length; i++)
        {
            if (i / levelsPerPage == page)
            {
                customLevelButtons[i].Visible = true;
                editCustomLevelButtons[i].Visible = true;
                deleteCustomLevelButtons[i].Visible = true;
            }
            else
            {
                customLevelButtons[i].Visible = false;
                editCustomLevelButtons[i].Visible = false;
                deleteCustomLevelButtons[i].Visible = false;
            }
        }
    }
}
