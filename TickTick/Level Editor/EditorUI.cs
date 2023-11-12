
using Engine;
using GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Contains all UI elements for the editor
/// </summary>
public class EditorUI
{
    private EditorDock dock;
    public EditorHUD hud;
    public GameObjectList gameObjects;

    public EditorUI(GameObjectList gameObjects, LevelEditorState editor)
    {
        this.gameObjects = gameObjects;
        dock = new EditorDock(this, editor);
        hud = new EditorHUD(this, editor);
    }

    public void HandleInput()
    {
        dock.HandleInput();
        hud.HandleInput();
    }
    
    public void Update(GameTime gameTime)
    {
        dock.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}