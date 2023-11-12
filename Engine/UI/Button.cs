using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI
{
    /// <summary>
    /// A class that can represent a UI button in the game.<br/>
    /// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> Button
    /// There are 2 types of buttons, one where you pass a sprite and when clicked on an event triggers,
    /// the other requires passing a text and font and a custom sized button will be made.
    /// </summary>
    public class Button : UISpriteGameObject
    {
        /// <summary>
        /// Whether this button has been pressed (clicked) in the current frame.
        /// </summary>
        public bool Pressed { get; protected set; }
        public bool Hovered { get; protected set; }

        private string text = "";
        public string Text => text;
        private SpriteFont font;
        public Color Color { get; set; }
        
        public int TextWidth { get; private set; }

        private Texture2D beginButtonTexture;
        private Texture2D middleButtonTexture;
        private Texture2D endButtonTexture;

        public int fixedWidth = 0; // 0 means auto. only works with custom strings

        /// <summary>
        /// Creates a new <see cref="Button"/> with the given sprite name and depth.
        /// </summary>
        /// <param name="assetName">The name of the sprite to use.</param>
        /// <param name="depth">The depth at which the button should be drawn.</param>
        public Button(string assetName, float depth) : base(assetName, depth)
        {
            Pressed = false;
        }
        
        /// <summary>
        /// Creates a button with custom size (based on text size in selected font)
        /// string assetName is not used, only passed to base because it is required
        /// Can select font and text can be changed later
        /// Also allows for changing color
        /// </summary>
        /// <param name="assetName"/>Not used, only passed to satisfy base
        /// <param name="depth"></param>
        /// <param name="text"></param>
        /// <param name="fontAssetName"></param>
        /// <param name="color">Background color of button</param>
        public Button(string assetName, float depth, string text, string fontAssetName, Color? color = null) : base(assetName, depth)
        {
            Pressed = false;
            this.text = text;
            beginButtonTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_button_begin");
            middleButtonTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_button_middle");
            endButtonTexture = ExtendedGame.AssetManager.LoadSprite("Sprites/UI/spr_button_end");
            font = ExtendedGame.AssetManager.LoadFont(fontAssetName);
            if (color != null)
            {
                this.Color = color.Value;
            }
            else
            {
                this.Color = Color.White;
            }
            
            //Hitbox is dynamically generated (doesn't use standard BoundingBox, because that is based on a Texture2D from assetName)
            TextWidth = (int)font.MeasureString(text).X;
            HitBox = new Rectangle(GlobalPosition.ToPoint(),
                new Point(beginButtonTexture.Width + endButtonTexture.Width + TextWidth + 8,
                    beginButtonTexture.Height));
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            //Can't be clicked if invisible
            if (!Visible)
            {
                Pressed = false;
                Hovered = false;
                return;
            }
            
            //If text is not set (meaning it's a sprite based button, use the BoundingBox for detecting being pressed)
            if (text == "")
            {
                Pressed = Visible && inputHelper.MouseLeftButtonPressed() && BoundingBox.Contains(inputHelper.MousePositionWorld);
                Hovered = Visible && BoundingBox.Contains(inputHelper.MousePositionWorld);
                return;
            }

            //Detection for dynamic buttons
            Pressed = Visible && inputHelper.MouseLeftButtonPressed() && HitBox.Contains(inputHelper.MousePositionWorld);
            Hovered = Visible && HitBox.Contains(inputHelper.MousePositionWorld);
        }

        //Edit text with this function
        public void SetText(string text)
        {
            this.text = text;

            //Fixed with is used when the size of the button is not based on the text it contains (for input fields)
            if (fixedWidth != 0)
            {
                if (font.MeasureString(text).X > fixedWidth)
                    this.text = this.text[..^1];
                return;
            }
            
            //Update hitbox
            HitBox = new Rectangle(GlobalPosition.ToPoint(),
                new Point(beginButtonTexture.Width + endButtonTexture.Width + TextWidth + 8,
                    beginButtonTexture.Height));
        }

        public override void Reset()
        {
            base.Reset();
            Pressed = false;
            int stringWidth = (int)font.MeasureString(text).X;
            
            //If fixedWidth is used then use that system
            if (fixedWidth != 0)
            {
                stringWidth = fixedWidth;
            }
            
            HitBox = new Rectangle(GlobalPosition.ToPoint(),
                new Point(beginButtonTexture.Width + endButtonTexture.Width + stringWidth + 8,
                    beginButtonTexture.Height));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
        {
            if (!Visible)
                return;
            
            //Draw normally if a sprite based button
            if (text == "")
            {
                base.Draw(gameTime, spriteBatch, opacity);
                return;
            }

            //Get string width
            int stringWidth;
            if (fixedWidth == 0)
                stringWidth = (int)font.MeasureString(text).X;
            else
                stringWidth = fixedWidth;
            
            //Draw all 3 parts of button
            spriteBatch.Draw(beginButtonTexture, new Rectangle(GlobalPosition.ToPoint(), beginButtonTexture.Bounds.Size), null, Color, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(middleButtonTexture, new Rectangle(GlobalPosition.ToPoint() + new Point(beginButtonTexture.Width, 0), new Point(stringWidth + 8, middleButtonTexture.Height)), null, Color, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.Draw(endButtonTexture, new Rectangle(GlobalPosition.ToPoint() + new Point(beginButtonTexture.Width + stringWidth + 8, 0), endButtonTexture.Bounds.Size), null, Color, 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            
            //Draw string
            spriteBatch.DrawString(font, text, localPosition + new Vector2(beginButtonTexture.Width + 4, 10), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }
}