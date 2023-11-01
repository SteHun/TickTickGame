
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Contains all UI elements
/// </summary>
public class EditorUI
{
    private EditorDock dock;
    public GameObjectList gameObjects;

    public EditorUI(GameObjectList gameObjects)
    {
        this.gameObjects = gameObjects;
        dock = new EditorDock(this);
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