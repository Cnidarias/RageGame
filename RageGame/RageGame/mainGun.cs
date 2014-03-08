using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace RageGame
{
    struct bulletStruct 
    {
        public Vector2 position;
        public int direction;
        public int speed;
        
    }
    class mainGun
    {
        ContentManager _Content;
        SpriteBatch _spirteBatch;
        Texture2D bulletTexture;
        List<bulletStruct> bulletList = new List<bulletStruct>();
        bulletStruct[] bulletArray;
        KeyboardState keys, oldKeys;

        public mainGun(ContentManager Content, SpriteBatch spriteBatch)
        {
            _Content = Content;
            _spirteBatch = spriteBatch;
            InitializeGun();
        }
        private void InitializeGun()
        {
            bulletTexture = _Content.Load<Texture2D>("Graphics\\Character\\mainGun\\mainGunTexture");
        }

        public void UpdateGun(Vector2 characterPosition, int directionCharacterIsFacing)
        {
            keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Z) && oldKeys.IsKeyUp(Keys.Z))
            {
                bulletStruct b = new bulletStruct();
                b.position = characterPosition;
                b.direction = directionCharacterIsFacing;
                b.speed = 8;
                bulletList.Add(b);
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                float x = bulletList.ElementAt<bulletStruct>(i).position.X;
                float y = bulletList.ElementAt<bulletStruct>(i).position.Y;
                int dir = bulletList.ElementAt<bulletStruct>(i).direction;

                if (x > 800 || x < 0 || y > 600 || y < 0 || dir == 0)
                {
                    bulletList.RemoveAt(i);
                }
            }
                bulletArray = bulletList.ToArray();

            for (int i = 0; i < bulletArray.Length; i++)
            {
                bulletArray[i].position.X += bulletArray[i].direction * bulletArray[i].speed;
            }
            bulletList = bulletArray.ToList<bulletStruct>();    
            oldKeys = keys;
        }
        public void drawGun()
        {
            if (bulletArray != null)
            {
                for (int i = 0; i < bulletArray.Length; i++)
                {
                    _spirteBatch.Draw(bulletTexture, new Rectangle((int)bulletArray[i].position.X, (int)bulletArray[i].position.Y, bulletTexture.Width, bulletTexture.Height), Color.White);
                }
            }
        }

    }
}
