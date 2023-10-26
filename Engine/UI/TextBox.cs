using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

/// <summary>
/// A class that can represent a text with a box around it in game.<br/>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> TextBox
/// </summary>
public class TextBox : UISpriteGameObject
{
    /// <summary>
    /// The text that is displayed
    /// </summary>
    private string text;

    /// <summary>
    /// The color of the text that is displayed
    /// </summary>
    private Color color;

    private SpriteFont spriteFont;
    
    public TextBox(string assetName, float depth, SpriteFont spriteFont, Color color, string text) : base(assetName, depth)
    {
        this.text = text;
        this.spriteFont = spriteFont;
        this.color = color;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        base.Draw(gameTime, spriteBatch, opacity);
        spriteBatch.DrawString(spriteFont, text, GlobalPosition, color * opacity);
    }
}