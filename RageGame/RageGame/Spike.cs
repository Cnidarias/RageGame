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
    struct spikeStruct
    {
        public int ID;
        public Vector2 Position;
        public int Speed;
        public float Direction;
        public Rectangle rectangle;
        public bool moving;
    }

    class Spike
    {
        List<spikeStruct> _deathObjects = new List<spikeStruct>();
        spikeStruct[] spikeArray;
        string _path;
        StreamReader reader;      
        Texture2D spikeTexture, characterTexture;
        Color [] spikeColorField, characterColorField;
        Rectangle characterRectaangle;
        ContentManager _content;
        SpriteBatch _spriteBatch;

        public Spike(String level, ContentManager Content, SpriteBatch spriteBatch)
        {
            _path = level;
            _content = Content;
            _spriteBatch = spriteBatch;
            loadSpikes();
        }

        private void loadSpikes()
        {
            characterTexture = _content.Load<Texture2D>("Graphics\\Character\\Character");
            characterColorField = new Color[characterTexture.Width * characterTexture.Height];
            characterTexture.GetData(characterColorField);

            spikeTexture = _content.Load<Texture2D>("Graphics\\DeathObjects\\Spike");
            spikeColorField = new Color[spikeTexture.Width * spikeTexture.Height];
            spikeTexture.GetData(spikeColorField);


            reader = new StreamReader("Content\\Maps\\Death\\deathlvl" + _path + ".txt");
            while (!reader.EndOfStream)
            {
                spikeStruct s = new spikeStruct();
                s.ID = Convert.ToInt32(reader.ReadLine());
                s.Position.X = (float)Convert.ToDouble(reader.ReadLine());
                s.Position.Y = (float)Convert.ToDouble(reader.ReadLine());
                s.Speed = Convert.ToInt32(reader.ReadLine());
                s.Direction = (float)Convert.ToDouble(reader.ReadLine());
                s.moving = Convert.ToBoolean(reader.ReadLine());

                _deathObjects.Add(s);
            }
            reader.Close();
            spikeArray = _deathObjects.ToArray();
            
        }

        public bool SpikeUpdate(Vector2 playerPosition)
        {
            characterRectaangle = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, characterTexture.Width, characterTexture.Height);
            for (int i = 0; i < spikeArray.Length; i++)
            {
                if (spikeArray[i].moving == true)
                {
                    spikeArray[i].Position.Y -= (float)(spikeArray[i].Speed * Math.Sin(MathHelper.ToRadians(spikeArray[i].Direction)));
                    spikeArray[i].Position.X += (float)(spikeArray[i].Speed * Math.Cos(MathHelper.ToRadians(spikeArray[i].Direction)));
                }
                spikeArray[i].rectangle = new Rectangle((int)spikeArray[i].Position.X, (int)spikeArray[i].Position.Y, spikeTexture.Width, spikeTexture.Height);
                if (IntersectPixels(characterRectaangle, characterColorField, spikeArray[i].rectangle, spikeColorField))
                {
                    return false;
                }
            }
            return true;
        }

        public void SpikeDraw()
        {
            for(int i = 0; i < spikeArray.Length; i++)
            {
                _spriteBatch.Draw(spikeTexture, spikeArray[i].rectangle, Color.White);
            }
           
        }

        public void triggerSpike(int[] IDnumber)
        {
            if (IDnumber != null)
            {
                for (int i = 0; i < IDnumber.Length; i++)
                {
                    spikeArray[IDnumber[i]].moving = true;
                }
            }
        }

        private bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                   Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
