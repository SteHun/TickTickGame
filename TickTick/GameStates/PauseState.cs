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
            ExtendedGame.GameStateManager.SwitchTo(TickTick.previousStatePlaying);
        }
    }
}