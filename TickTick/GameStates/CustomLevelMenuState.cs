using System.IO;
using System.Linq;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> LevelMenuState
/// </summary>
class CustomLevelMenuState : GameState
{
    Button backButton, officialLevelButton;

    // An array of extra references to the level buttons. 
    // This makes it easier to check if a level button has been pressed.
    Button[] customLevelButtons;

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

        // Get all custom level files in the folder
        string [] arrays;
        string sdira= "Content/CustomLevels/";
        arrays =  Directory.GetFiles(sdira, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();

        //Set array size
        customLevelButtons = new Button[arrays.Length];
        Vector2 gridOffset = new Vector2(395, 175);
        const int verticalSpace = 20;

        //Place all buttons and give them the correct names
        for (int i = 0; i < arrays.Length; i++)
        {
            // create the button (and give it the correct name)
            Button levelButton = new Button("Sprites/UI/spr_button_back", TickTick.Depth_UIForeground, arrays[i], "Fonts/MainFont");

            // give it the correct position
            levelButton.LocalPosition = gridOffset + new Vector2(0, (levelButton.Height + verticalSpace) * i);
            
            //Reset tu update button hitbox
            levelButton.Reset();
            
            // add the button as a child object
            gameObjects.AddChild(levelButton);
            // also store it in the array of level buttons
            customLevelButtons[i] = levelButton;
        }
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);

        // if the back button is pressed, go back to the title screen
        if (backButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
        
        if (officialLevelButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_LevelSelect);

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
    }
}
