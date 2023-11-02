using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// IGameLoopObject -> GameState -> PauseState
/// </summary>
class PauseState : GameState
{
    Button resumeButton, quitButton;

    public PauseState()
    {
        Camera.position = Vector2.Zero;
        
        // add a background
        gameObjects.AddChild(new UISpriteGameObject("Sprites/Backgrounds/spr_pause", TickTick.Depth_Background));

        // add text box for pause
        // UISpriteGameObject pauseTextBox = new UISpriteGameObject("Sprites/UI/spr_frame_text", TickTick.Depth_UIBackground);
        // pauseTextBox.LocalPosition = new Vector2(720, 200);
        // gameObjects.AddChild(pauseTextBox);
        
        // add pause text
        // TextGameObject hintText = new TextGameObject("Fonts/HintFont", TickTick.Depth_UIForeground, Color.Black);
        // hintText.Text = "Game is paused";
        // hintText.LocalPosition = new Vector2(720, 200) + new Vector2(25, 20);
        // gameObjects.AddChild(hintText);
        
        // add a "resume" button
        resumeButton = new Button("Sprites/UI/spr_button_quit", TickTick.Depth_UIForeground, "resume", "Fonts/MainFont");
        resumeButton.LocalPosition = new Vector2((1440 - (float)resumeButton.TextWidth) / 2, 400);
        resumeButton.Reset();
        gameObjects.AddChild(resumeButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", TickTick.Depth_UIForeground, "quit", "Fonts/MainFont");
        quitButton.LocalPosition = new Vector2((1440 - (float)quitButton.TextWidth) / 2, 600); 
        quitButton.Reset();
        gameObjects.AddChild(quitButton);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);

        if (resumeButton.Pressed)
        {
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Playing);
        }
        else if (quitButton.Pressed)
        {
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_LevelSelect);
        }
    }
}