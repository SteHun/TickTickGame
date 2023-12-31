﻿using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> LevelMenuState
/// </summary>
class LevelMenuState : GameState
{
    Button backButton, customLevelButton;

    // An array of extra references to the level buttons. 
    // This makes it easier to check if a level button has been pressed.
    LevelButton[] levelButtons;

    public LevelMenuState()
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
        
        // add a custom level button
        customLevelButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, "CUSTOM LEVELS", "Fonts/MainFont");
        customLevelButton.LocalPosition = new Vector2(740, 690);
        gameObjects.AddChild(customLevelButton);
        customLevelButton.Reset();

        // Add a level button for each level.
        levelButtons = new LevelButton[ExtendedGameWithLevels.NumberOfLevels];

        Vector2 gridOffset = new Vector2(395, 175);
        const int buttonsPerRow = 4;
        const int spaceBetweenColumns = 20;
        const int spaceBetweenRows = 20;

        for (int i = 0; i < ExtendedGameWithLevels.NumberOfLevels; i++)
        {
            // create the button
            LevelButton levelButton = new LevelButton(i + 1, ExtendedGameWithLevels.GetLevelStatus(i + 1));

            // give it the correct position
            int row = i / buttonsPerRow;
            int column = i % buttonsPerRow;

            levelButton.LocalPosition = gridOffset + new Vector2(
                column * (levelButton.Width + spaceBetweenColumns),
                row * (levelButton.Height + spaceBetweenRows)
            );

            // add the button as a child object
            gameObjects.AddChild(levelButton);
            // also store it in the array of level buttons
            levelButtons[i] = levelButton;
        }
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);

        // if the back button is pressed, go back to the title screen
        if (backButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
        
        if(customLevelButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_CustomLevelSelect);

        // if a (non-locked) level button has been pressed, go to that level
        foreach (LevelButton button in levelButtons)
        {
            if (button.Pressed && button.Status != LevelStatus.Locked)
            {
                // go to the playing state
                TickTick.previousStatePlaying = ExtendedGameWithLevels.StateName_LevelSelect;
                ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Playing);

                // load the correct level
                ExtendedGameWithLevels.GetPlayingState().LoadLevel(button.LevelIndex);

                return;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        foreach (LevelButton button in levelButtons)
        {
            if (button.Status != ExtendedGameWithLevels.GetLevelStatus(button.LevelIndex))
                button.Status = ExtendedGameWithLevels.GetLevelStatus(button.LevelIndex);
        }
    }
}
