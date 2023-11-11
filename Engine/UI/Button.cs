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
            
            TextWidth = (int)font.MeasureString(text).X;
            HitBox = new Rectangle(GlobalPosition.ToPoint(),
                new Point(beginButtonTexture.Width + endButtonTexture.Width + TextWidth + 8,
                    beginButtonTexture.Height));
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (!Visible)
                return;
            
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

        public void SetText(string text)
        {
            this.text = text;

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
            if (fixedWidth == 0)
            {
                HitBox = new Rectangle(GlobalPosition.ToPoint(),
                    new Point(beginButtonTexture.Width + endButtonTexture.Width + stringWidth + 8,
                        beginButtonTexture.Height));
            }
            else
            {
                HitBox = new Rectangle(GlobalPosition.ToPoint(),
                    new Point(beginButtonTexture.Width + endButtonTexture.Width + fixedWidth + 8,
                        beginButtonTexture.Height));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
        {
            if (!Visible)
                return;
            
            if (text == "")
            {
                base.Draw(gameTime, spriteBatch);
                return;
            }

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