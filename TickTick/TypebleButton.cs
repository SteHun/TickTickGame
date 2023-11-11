
using System.Linq;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> Button -> TypableButton
/// </summary>
public class TypebleButton : Button
{
    private readonly Keys[] allowedKeys = new Keys[] {Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, 
        Keys.P, Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.K, Keys.L, Keys.Z, Keys.X, Keys.C, 
        Keys.V, Keys.B, Keys.N, Keys.M
    };

    public bool IsTyping;
    public string TypedText => Text;

    public TypebleButton(string assetName, float depth, string text, string fontAssetName)
        : base(assetName, depth, text, fontAssetName)
    {
        SetText(text);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        if (!Visible)
            return;
        
        // This is kind of jank but whatever
        if (IsTyping)
        {
            SetText(Text + '_');
            base.Draw(gameTime, spriteBatch, opacity);
            SetText(Text[..^1]);
            return;
        }
        base.Draw(gameTime, spriteBatch, opacity);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        base.HandleInput(inputHelper);
        if (Pressed)
            IsTyping = !IsTyping;
        if (!IsTyping)
            return;

        foreach (Keys key in Keyboard.GetState().GetPressedKeys())
        {
            if (!inputHelper.KeyPressed(key))
                continue;
            if (key == Keys.Back)
            {
                if (Text.Length > 0)
                    SetText(Text[..^1]); // delete last character
                continue;
            }

            if (key == Keys.Enter && Text.Length > 0)
            {
                IsTyping = false;
                continue;
            }

            if (key == Keys.Space)
            {
                SetText(Text + " ");
            }

            if (allowedKeys.Contains(key))
            {
                //First letter is uppercase, rest is lowercase, also checks for CapsLock and shift
                if(Text.Length == 0 || Keyboard.GetState().CapsLock || Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                   || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                    SetText(Text + key.ToString().ToUpper());
                else
                    SetText(Text + key.ToString().ToLower());
            }
        }
    }
}