using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace GameStates;

public class LevelEditorState : GameState
{
    private Button saveButton;
    private Button quitButton;

    public LevelEditorState()
    {
        // add a "save" button
        saveButton = new Button("Sprites/UI/spr_button_editor", 1);// temp
        saveButton.LocalPosition = new Vector2(20, 20);
        gameObjects.AddChild(saveButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(1290, 20);
        gameObjects.AddChild(quitButton);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        
    }
}