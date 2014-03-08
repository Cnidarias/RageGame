using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace RageGame
{
    class spamButtonDeath
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        int zPressed, shiftPressed, keysPressed;
        KeyboardState oldKeys;

        public bool spamUpdate(KeyboardState keys)
        {
            if (!stopWatch.IsRunning)
                stopWatch.Start();

            if (stopWatch.ElapsedMilliseconds > 2000)
            {
                stopWatch.Restart();
                zPressed = 0;
                shiftPressed = 0;
                keysPressed = 0;
            }
            if (keys.IsKeyDown(Keys.Z) && oldKeys.IsKeyUp(Keys.Z))
                zPressed += 1;
            if (keys.IsKeyDown(Keys.LeftShift) && oldKeys.IsKeyUp(Keys.LeftShift))
                shiftPressed += 1;           

            Keys[] k = keys.GetPressedKeys();
            int pressed = k.Length;

            if (k.Contains(Keys.LeftShift))
                pressed--;
            if (k.Contains(Keys.Right))
                pressed--;
            if (k.Contains(Keys.Left))
                pressed--;
            if (k.Contains(Keys.Z))
                pressed--;
            if (k.Contains(Keys.R))
                pressed--;

            keysPressed += pressed;

            oldKeys = keys;

            if (zPressed > 9 || shiftPressed > 5 || keysPressed > 3)
            {
                stopWatch.Reset();
                zPressed = 0; shiftPressed = 0; keysPressed = 0;
                return true;                            
            }

            return false;
        }
    }
}
