using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

public class TextBox : UISpriteGameObject
{
    public string text = "";
    private SpriteFont font;

    private Texture2D beginTextBoxTexture;
    private Texture2D middleTextBoxTexture;
    private Texture2D endTextBoxTexture;
    
    public TextBox(string assetName, float depth, string text, string fontAssetName) : base(assetName, depth)
    {
        this.text = text;
        beginTextBoxTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_textbox_begin");
        middleTextBoxTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_textbox_middle");
        endTextBoxTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_textbox_end");
        font = ExtendedGame.AssetManager.LoadFont(fontAssetName);
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        if (!Visible)
            return;
        
        int stringWidth = (int)font.MeasureString(text).X;
            
        //Draw all 3 parts of button
        spriteBatch.Draw(beginTextBoxTexture, new Rectangle(GlobalPosition.ToPoint(), beginTextBoxTexture.Bounds.Size), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
        spriteBatch.Draw(middleTextBoxTexture, new Rectangle(GlobalPosition.ToPoint() + new Point(beginTextBoxTexture.Width, 0), new Point(stringWidth + 8, middleTextBoxTexture.Height)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
        spriteBatch.Draw(endTextBoxTexture, new Rectangle(GlobalPosition.ToPoint() + new Point(beginTextBoxTexture.Width + stringWidth + 8, 0), endTextBoxTexture.Bounds.Size), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            
        //Draw string
        spriteBatch.DrawString(font, text, localPosition + new Vector2(beginTextBoxTexture.Width + 4, 10), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }
}