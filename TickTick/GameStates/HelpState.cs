using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

/// <summary>
/// IGameLoopObject -> GameState -> HelpState
/// </summary>
class HelpState : GameState
{
    Button backButton;

    public HelpState()
    {
        Camera.position = Vector2.Zero;
        
        // add a background
        gameObjects.AddChild(new SpriteGameObject("Sprites/Backgrounds/spr_help", 1));

        // add a back button
        backButton = new Button("Sprites/UI/spr_button_back", 1);
        backButton.LocalPosition = new Vector2(1180, 750);
        gameObjects.AddChild(backButton);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);
        if (backButton.Pressed)
            ExtendedGame.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
    }
}
