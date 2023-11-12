using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine;

public static class Camera
{
    public static Vector2 position = Vector2.Zero;
    public static Point cameraSize; //Used for bounding box
    public static Point levelSize;
    
    public static void Update(Vector2 playerPosition)
    {
        //Offset by half of cameraSize to get player in the middle (and not top left)
        position = playerPosition - cameraSize.ToVector2()/2;

        //Bounding box (camera can't show outside of level)
        position.X = MathHelper.Clamp(position.X, 0, levelSize.X - cameraSize.X);
        if (levelSize.Y < cameraSize.Y)
        {
            position.Y = levelSize.Y - cameraSize.Y;
        }
        else
        {
            position.Y = MathHelper.Clamp(position.Y, 0, levelSize.Y - cameraSize.Y);            
        }
    }
}