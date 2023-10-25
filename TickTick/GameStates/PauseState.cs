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
    private TextBox pauseTextBox;

    public PauseState(SpriteFont spriteFont)
    {
        // add a background
        gameObjects.AddChild(new SpriteGameObject("Sprites/Backgrounds/spr_help", 0));
        
        // add pause text
        pauseTextBox = new TextBox("Sprites/UI/spr_frame_text", 0.9f, spriteFont, Color.Black, "Game is paused");
        pauseTextBox.LocalPosition = new Vector2(720, 200);
        gameObjects.AddChild(pauseTextBox);
        
        // add a "resume" button
        resumeButton = new Button("Sprites/UI/spr_button_quit", 1);
        resumeButton.LocalPosition = new Vector2(720, 400);
        gameObjects.AddChild(resumeButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(720, 600); 
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