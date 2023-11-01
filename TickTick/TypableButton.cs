
using System.Linq;
using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> Button -> TypableButton
/// </summary>
public class TypableButton : Button
{
    private readonly Keys[] allowedKeys = new Keys[] {Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, 
        Keys.P, Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.K, Keys.L, Keys.Z, Keys.X, Keys.C, 
        Keys.V, Keys.B, Keys.N, Keys.M
    };

    public bool isTypable { get; private set; }
    public TypableButton(string assetName, float depth, string text, string fontAssetName)
        : base(assetName, depth, text, fontAssetName)
    {
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float opacity = 1)
    {
        // This is kind of jank but whatever
        if (isTypable)
        {
            text += '_';
            base.Draw(gameTime, spriteBatch, opacity);
            text = text[..^1];
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
                if (text.Length > 0)
                    text = text[..^1]; // delete last character
                continue;
            }

            if (key == Keys.Enter)
            {
                isTypable = false;
                continue;
            }

            if (allowedKeys.Contains(key))
            {
                text += key.ToString();
            }
        }
    }
}