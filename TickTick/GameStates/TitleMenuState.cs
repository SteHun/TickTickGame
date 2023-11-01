using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> TitleMenuState
/// </summary>
class TitleMenuState : GameState
{
    Button playButton, editorButton, helpButton;

    public TitleMenuState()
    {
        Camera.position = Vector2.Zero;

        // load the title screen
        SpriteGameObject titleScreen = new SpriteGameObject("Sprites/Backgrounds/spr_title", TickTick.Depth_Background);
        gameObjects.AddChild(titleScreen);

        // add a play button
        playButton = new Button("Sprites/UI/spr_button_play", TickTick.Depth_UIForeground);
        playButton.LocalPosition = new Vector2(600, 540);
        gameObjects.AddChild(playButton);
        
        // add a editor button
        editorButton = new Button("Sprites/UI/spr_button_editor", TickTick.Depth_UIForeground);
        editorButton.LocalPosition = new Vector2(600, 600);
        gameObjects.AddChild(editorButton);

        // add a help button
        helpButton = new Button("Sprites/UI/spr_button_help", TickTick.Depth_UIForeground);
        helpButton.LocalPosition = new Vector2(600, 660);
        gameObjects.AddChild(helpButton);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);
        if (playButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_LevelSelect);
        else if (editorButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Editor);
        else if (helpButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Help);
    }
}