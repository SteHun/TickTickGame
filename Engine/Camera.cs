using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine;

public static class Camera
{
    public static Vector2 position = Vector2.Zero;
    private static Point cameraSize = new (1440, 825); //Used for bounding box
    public static Point worldSize;
    
    public static void Update(Vector2 playerPosition)
    {
        //Clamp position here
        position = playerPosition - worldSize.ToVector2()/2;

        //Bounding box (camera can't show outside of level)
        position.X = MathHelper.Clamp(position.X, 0, worldSize.X - cameraSize.X);
        position.Y = MathHelper.Clamp(position.Y, 0, worldSize.Y - cameraSize.Y);
    }
}