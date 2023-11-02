
using System.Linq;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> Button -> TypableButton
/// </summary>
public class TypeableButton : Button
{
    private readonly Keys[] allowedKeys = new Keys[] {Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, 
        Keys.P, Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.K, Keys.L, Keys.Z, Keys.X, Keys.C, 
        Keys.V, Keys.B, Keys.N, Keys.M
    };

    public bool isTypable { get; private set; }
    public TypeableButton(string assetName, float depth, string text, string fontAssetName)
        : base(assetName, depth, text, fontAssetName)
    {
        SetText("ENTER TEXT");
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        // This is kind of jank but whatever
        if (isTypable)
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
            isTypable = !isTypable;
        if (!isTypable)
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
                isTypable = false;
                continue;
            }

            if (key == Keys.Space)
            {
                SetText(Text + " ");
            }

            if (allowedKeys.Contains(key))
            {
                //First letter is uppercase, rest is lowercase, also checks for CapsLock and shift
                if(Text.Length == 0 || Keyboard.GetState().CapsLock || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    SetText(Text + key.ToString().ToUpper());
                else
                    SetText(Text + key.ToString().ToLower());
            }
        }
    }
}