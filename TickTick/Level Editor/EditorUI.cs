
using Engine;
using GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Contains all UI elements
/// </summary>
public class EditorUI
{
    private EditorDock dock;
    private LevelEditorState editor;
    public GameObjectList gameObjects;

    public EditorUI(GameObjectList gameObjects, LevelEditorState editor)
    {
        this.gameObjects = gameObjects;
        this.editor = editor;
        dock = new EditorDock(this, editor);
    }

    public void HandleInput()
    {
        dock.HandleInput();
    }
    
    public void Update(GameTime gameTime)
    {
        dock.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}