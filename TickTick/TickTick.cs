using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using GameStates;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Game(MonoGame) -> ExtendedGame -> ExtendedGameWithLevels -> TickTick
/// </summary>
class TickTick : ExtendedGameWithLevels
{
    public const float Depth_Background = 0; // for background images
    public const float Depth_UIBackground = 0.9f; // for UI elements with text on top of them
    public const float Depth_UIForeground = 1; // for UI elements in front
    public const float Depth_LevelTiles = 0.5f; // for tiles in the level
    public const float Depth_LevelObjects = 0.6f; // for all game objects except the player
    public const float Depth_LevelPlayer = 0.7f; // for the player

    //Used to check if the back button send the player to level select or the editor
    public static string previousStatePlaying;
    
    [STAThread]
    static void Main()
    {
        TickTick game = new TickTick();
        game.Run();
    }

    public TickTick()
    {
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        // set a custom world and window size
        worldSize = new Point(1440, 825);
        windowSize = new Point(1024, 586);
        
        //Camera is the size of what is visible in the world at one time
        Camera.cameraSize = worldSize;
        
        // to let these settings take effect, we need to set the FullScreen property again
        FullScreen = false;

        // load the player's progress from a file
        LoadProgress();

        // add the game states
        GameStateManager.AddGameState(StateName_Title, new TitleMenuState());
        GameStateManager.AddGameState(StateName_LevelSelect, new LevelMenuState());
        GameStateManager.AddGameState(StateName_CustomLevelSelect, new CustomLevelMenuState());
        GameStateManager.AddGameState(StateName_Help, new HelpState());
        GameStateManager.AddGameState(StateName_Playing, new PlayingState());
        GameStateManager.AddGameState(StateName_Pause, new PauseState());
        GameStateManager.AddGameState(StateName_Editor, new LevelEditorState());

        previousStatePlaying = StateName_Title;

        // start at the title screen
        GameStateManager.SwitchTo(StateName_Title);

        // play background music
        AssetManager.PlaySong("Sounds/snd_music", true);
    }

    //Returns the level editor state
    public static LevelEditorState GetEditor()
    {
        return (LevelEditorState)GameStateManager.gameStates[StateName_Editor];
    }
}