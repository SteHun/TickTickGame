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
    private TypebleButton nameButton;
    public string NameButtonTypedText => nameButton.TypedText;

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
        
        // add a name button
        nameButton = new TypebleButton("Sprites/UI/spr_frame_text", 0.9f, "test", "Fonts/MainFont");
        nameButton.LocalPosition = new Vector2(300, 50);
        nameButton.fixedWidth = 400;
        nameButton.Reset();
        editorUI.gameObjects.AddChild(nameButton);
    }

    public void HandleInput()
    {
        if (quitButton.Hovered || saveButton.Hovered || nameButton.Hovered || nameButton.IsTyping)
            editor.hoveringAnyButton = true;

        if (quitButton.Pressed)
            TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);
        
        if (saveButton.Pressed)
            editor.SaveLevelToFile("test");
        
        //nameButton.HandleInput(inputHelper);
    }
}