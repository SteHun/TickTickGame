using Engine;
using Engine.UI;
using GameStates;
using Microsoft.Xna.Framework;

/// <summary>
/// The general buttons in the editor, not linked to the dock
/// </summary>
public class EditorHUD
{
    const string DefaultName = "Name";
    public const string DefaultDescription = "Description";
    private EditorUI editorUI;
    private LevelEditorState editor;

    private Button playButton;
    private Button saveButton;
    private Button quitButton;
    private Button visibilityButton;
    public TypebleButton nameButton;
    private TypebleButton levelDescriptionInputField;
    private BombTimer timer;
    private Button timerUpButton;
    private Button timerDownButton;
    
    public EditorHUD(EditorUI editorUI, LevelEditorState editor)
    {
        this.editorUI = editorUI;
        this.editor = editor;
        
        // add a "play" button
        playButton = new Button("Sprites/UI/spr_button_play", 1);
        playButton.LocalPosition = new Vector2(270, 20);
        editorUI.gameObjects.AddChild(playButton);
        
        // add a "save" button
        saveButton = new Button("Sprites/UI/spr_button_editor", 1, "Save", "Fonts/MainFont");
        saveButton.LocalPosition = new Vector2(20, 20);
        saveButton.Reset();
        editorUI.gameObjects.AddChild(saveButton);
        
        // add a "quit" button
        quitButton = new Button("Sprites/UI/spr_button_quit", 1);
        quitButton.LocalPosition = new Vector2(1290, 20);
        editorUI.gameObjects.AddChild(quitButton);
        
        // add a "visibility" toggle button
        visibilityButton = new Button("Sprites/UI/spr_button_visibility", 1);
        visibilityButton.LocalPosition = new Vector2(1185, 20);
        editorUI.gameObjects.AddChild(visibilityButton);
        
        // add a name button
        nameButton = new TypebleButton("Sprites/UI/spr_frame_text", 0.9f, DefaultName, "Fonts/MainFont");
        nameButton.LocalPosition = new Vector2(520, 20);
        nameButton.fixedWidth = 300;
        nameButton.Reset();
        editorUI.gameObjects.AddChild(nameButton);
        
        // add a "level description" typeble button (aka input field)
        levelDescriptionInputField = new TypebleButton("Sprites/UI/spr_frame_text", 0.9f, DefaultDescription, "Fonts/MainFont");
        levelDescriptionInputField.LocalPosition = new Vector2(520, 80);
        levelDescriptionInputField.fixedWidth = 500;
        levelDescriptionInputField.Reset();
        editorUI.gameObjects.AddChild(levelDescriptionInputField);
        
        // add timer settings
        timer = new BombTimer(editor.LevelTimer);
        timer.LocalPosition = new Vector2(100, 20);
        timer.Running = false;
        editorUI.gameObjects.AddChild(timer);

        timerUpButton = new Button("Sprites/UI/spr_button_timer_up", 1);
        timerUpButton.LocalPosition = new Vector2(198, 20);
        editorUI.gameObjects.AddChild(timerUpButton);
        
        timerDownButton = new Button("Sprites/UI/spr_button_timer_down", 1);
        timerDownButton.LocalPosition = new Vector2(198, 60);
        editorUI.gameObjects.AddChild(timerDownButton);
    }

    public void HandleInput()
    {
        if (quitButton.Hovered || saveButton.Hovered || visibilityButton.Hovered || nameButton.Hovered || nameButton.IsTyping || 
            playButton.Hovered || levelDescriptionInputField.Hovered || levelDescriptionInputField.IsTyping || 
            timerUpButton.Hovered || timerDownButton.Hovered)
            editor.hoveringAnyButton = true;

        if (playButton.Pressed)
        {
            editor.levelDescription = levelDescriptionInputField.TypedText;
            editor.Play();
        }
        
        if (quitButton.Pressed)
            TickTick.GameStateManager.SwitchTo(ExtendedGameWithLevels.StateName_Title);

        if (visibilityButton.Pressed)
            ToggleVisibility();

        if (saveButton.Pressed)
        {
            //Update level description
            editor.levelDescription = levelDescriptionInputField.TypedText;
            
            //Update level name
            editor.SaveLevelToFile(nameButton.Text);
        }

        if (timerUpButton.Pressed)
        {
            if (editor.LevelTimer >= 10)
            {
                editor.LevelTimer += 10;
            }
            else
            {
                editor.LevelTimer += 1;
            }
            timer.SetTime(editor.LevelTimer);
        }
        
        if (timerDownButton.Pressed)
        {
            if (editor.LevelTimer > 10)
            {
                editor.LevelTimer -= 10;
            }
            else
            {
                editor.LevelTimer = MathHelper.Max(1, editor.LevelTimer - 1);
            }
            timer.SetTime(editor.LevelTimer);
        }

        if (nameButton.Pressed)
        {
            levelDescriptionInputField.IsTyping = false;
        }

        if (levelDescriptionInputField.Pressed)
        {
            nameButton.IsTyping = false;
        }
    }

    private void ToggleVisibility()
    {
        bool oppositeState = !playButton.Visible;
        
        playButton.Visible = oppositeState;
        saveButton.Visible = oppositeState;
        quitButton.Visible = oppositeState;
        nameButton.Visible = oppositeState;
        levelDescriptionInputField.Visible = oppositeState;
        timer.Visible = oppositeState;
        timerUpButton.Visible = oppositeState;
        timerDownButton.Visible = oppositeState;
    }
}