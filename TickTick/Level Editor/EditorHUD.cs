using Engine;
using Engine.UI;
using GameStates;
using Microsoft.Xna.Framework;

public class EditorHUD
{
    /// <summary>
    /// The general buttons in the editor, not linked to the dock
    /// </summary>
    
    private EditorUI editorUI;
    private LevelEditorState editor;

    private Button saveButton;
    private Button quitButton;
    public EditorHUD(EditorUI editorUI, LevelEditorState editor)
    {
        this.editorUI = editorUI;
        this.editor = editor;
        
        // add a "save" button
        saveButton = new Button("Sprites/UI/spr_button_editor", 1);// temp
        saveButton.LocalPosition = new Vector2(20, 20);
        editorUI.gameObjects.AddChild(saveButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(1290, 20);
        editorUI.gameObjects.AddChild(quitButton);
    }

    public void HandleInput()
    {
        if (quitButton.Hovered || saveButton.Hovered)
            editor.hoveringAnyButton = true;

        if (quitButton.Pressed)
            TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
    }
}