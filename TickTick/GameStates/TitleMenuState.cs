using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> TitleMenuState
/// </summary>
class TitleMenuState : GameState
{
    Button playButton, editorButton, helpButton, quitButton;

    public TitleMenuState()
    {
        Camera.position = Vector2.Zero;

        // load the title screen
        SpriteGameObject titleScreen = new SpriteGameObject("Sprites/Backgrounds/spr_title", TickTick.Depth_Background);
        gameObjects.AddChild(titleScreen);

        // add a play button
        playButton = new Button("Sprites/UI/spr_button_play", TickTick.Depth_UIForeground);
        playButton.LocalPosition = new Vector2(600, 480);
        gameObjects.AddChild(playButton);
        
        // add a editor button
        editorButton = new Button("Sprites/UI/spr_button_editor", TickTick.Depth_UIForeground);
        editorButton.LocalPosition = new Vector2(600, 540);
        gameObjects.AddChild(editorButton);

        // add a help button
        helpButton = new Button("Sprites/UI/spr_button_help", TickTick.Depth_UIForeground);
        helpButton.LocalPosition = new Vector2(600, 600);
        gameObjects.AddChild(helpButton);
        
        // add a quit button
        quitButton = new Button("Sprites/UI/spr_button_quit", TickTick.Depth_UIForeground);
        quitButton.LocalPosition = new Vector2(650, 660);
        gameObjects.AddChild(quitButton);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);
        if (playButton.Pressed)
        {
            //Remove and add custom levels to update custom level list
            ExtendedGame.GameStateManager.RemoveGameState(ExtendedGameWithLevels.StateName_CustomLevelSelect);
            ExtendedGame.GameStateManager.AddGameState(ExtendedGameWithLevels.StateName_CustomLevelSelect, new CustomLevelMenuState());
            
            //Actually move to menu
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_LevelSelect);
        }
            
        else if (editorButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Editor);
        else if (helpButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Help);
        else if(quitButton.Pressed)
            ExtendedGame.Instance.Exit();
    }
}