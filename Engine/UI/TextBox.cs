using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

/// <summary>
/// A class that can represent a text with a box around it in game.<br/>
/// IGameLoopObject -> GameObject -> SpriteGameObject -> UISpriteGameObject -> TextBox
/// </summary>
public class TextBox : UISpriteGameObject
{
    public TextBox(string assetName, float depth) : base(assetName, depth)
    {
    }
}