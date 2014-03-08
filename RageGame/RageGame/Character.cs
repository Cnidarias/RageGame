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
    class Character
    {
        public struct fallingBlocks
        {
            public int timeOneCanStayInIt;
            public Vector2[] ids;
            public BoundingBox bBox;
        }

        enum State
        {
            walking,
            jumping
        }

        State currentState = State.walking;

        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;
        const int MOV_SPEED = 120;
        readonly Vector2 gravity = new Vector2(0, 0.8f);

        public Texture2D _characterTexture;
        public Vector2 _Position, oldPosition, theDirection = Vector2.Zero, theSpeed = Vector2.Zero, velocity;
        BoundingBox _characterBB_TOP, _characterBB_LEFT, _characterBB_RIGHT, _characterBB_BOTTOM, generalBB;
        List<BoundingBox> groundBB = new List<BoundingBox>();
        bool canMoveRight, canMoveLeft;
        SpriteBatch _spriteBatch;
        SpriteFont font;
        public int lastDirection;
        int jumpCounter = 0;
        int[,] _Tiles;
        int _tileWidth, _tileHeight, numberOfXTiles, numberOfYTiles;
        fallingBlocks[] fallingBlocksArray;
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
       
        GameTime oldGT = new GameTime();

        /*
         * Color[] personTextureData;
         * 
         * personTextureData =
                new Color[personTexture.Width * personTexture.Height];
            personTexture.GetData(personTextureData);
         * 
         * Rectangle personRectangle =
                new Rectangle((int)personPosition.X, (int)personPosition.Y,
                personTexture.Width, personTexture.Height);
         * 
         * needs the same for the second object being tested if pixel-perfect is required - means that non color needs to be in alpha (or 255)
         * 
         */

        KeyboardState _keys, _oldKeys;

        public Character(ContentManager Content, SpriteBatch spriteBatch, Vector2 Position)
        {
            _characterTexture = Content.Load<Texture2D>("Graphics\\Character\\Character");
            font = Content.Load<SpriteFont>("Fonts\\font");
            _spriteBatch = spriteBatch;
            _Position = Position;
        }

        public void loadGroundBB(int xTiles, int yTiles, int tileWidth, int tileHeight, int[,] tiles, string level)
        {
            _Tiles = tiles;
            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            numberOfXTiles = xTiles;
            numberOfYTiles = yTiles;

            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    if (tiles[i, j] != 9)
                    {
                        BoundingBox b = new BoundingBox(new Vector3(i * tileWidth, j * tileHeight, 0),
                            new Vector3((i + 1) * tileWidth, (j + 1) * tileHeight, 0));

                        groundBB.Add(b);
                    }

                }
            }
            loadFallingTriggers(level);
        }

        public void loadFallingTriggers(string level)
        {
            StreamReader sr = new StreamReader("Content\\Maps\\Death\\FallingBlocksLvl" + level + ".txt");

            fallingBlocksArray = new fallingBlocks[Convert.ToInt32(sr.ReadLine())];
            sr.ReadLine();
            for (int i = 0; i < fallingBlocksArray.Length; i++)
            {
                int xPos = Convert.ToInt32(sr.ReadLine());
                int yPos = Convert.ToInt32(sr.ReadLine());
                fallingBlocksArray[i].bBox = new BoundingBox(new Vector3(xPos * _tileWidth, yPos * _tileHeight, 0), new Vector3((xPos + 1) * _tileWidth, (yPos + 1) * _tileHeight, 0));
                fallingBlocksArray[i].timeOneCanStayInIt = Convert.ToInt32(sr.ReadLine());
                fallingBlocksArray[i].ids = new Vector2[Convert.ToInt32(sr.ReadLine())];
                for (int j = 0; j < fallingBlocksArray[i].ids.Length; j++)
                {
                    fallingBlocksArray[i].ids[j].X = Convert.ToInt32(sr.ReadLine());
                    fallingBlocksArray[i].ids[j].Y = Convert.ToInt32(sr.ReadLine());
                }
            }
            sr.Close();
        }

        public Vector2[] processFallingTrigers()
        {
            for (int i = 0; i < fallingBlocksArray.Length; i++)
            {
                if (fallingBlocksArray[i].bBox.Intersects(generalBB))
                    fallingBlocksArray[i].timeOneCanStayInIt--;

                if (fallingBlocksArray[i].timeOneCanStayInIt == 0)
                {
                    int indexOfFallTrigger = i;

                    if (indexOfFallTrigger > -1)
                    {
                        for (int j = 0; j < fallingBlocksArray[indexOfFallTrigger].ids.Length; j++)
                        {
                            Vector3 vec3forcheck = new Vector3(fallingBlocksArray[indexOfFallTrigger].ids[j].X * _tileWidth, fallingBlocksArray[indexOfFallTrigger].ids[j].Y * _tileHeight, 0);
                            for (int k = 0; k < groundBB.Count; k++)
                            {
                                if (vec3forcheck == groundBB.ElementAt<BoundingBox>(k).Min)
                                    groundBB.RemoveAt(k);
                            }
                        }
                    }
                    return fallingBlocksArray[indexOfFallTrigger].ids;
                }
            }
            Vector2[] nullie = new Vector2[1];
            return nullie;
        }

        public void Update(GameTime gameTime)
        {
            _keys = Keyboard.GetState();
            canMoveLeft = true;
            canMoveRight = true;           

            #region Create Bounding Boxes at Character Position
            _characterBB_BOTTOM = new BoundingBox(new Vector3(_Position.X+10, _Position.Y + _characterTexture.Height-5, 0),
                new Vector3(_Position.X + _characterTexture.Width-10, _Position.Y + _characterTexture.Height + 5, 0));

            _characterBB_TOP = new BoundingBox(new Vector3(_Position.X+10, _Position.Y-5, 0),
                new Vector3(_Position.X + _characterTexture.Width-10, _Position.Y + 5, 0));

            _characterBB_LEFT = new BoundingBox(new Vector3(_Position.X, _Position.Y + 10, 0),
                new Vector3(_Position.X + 5, _Position.Y + _characterTexture.Height - 10, 0));

            _characterBB_RIGHT = new BoundingBox(new Vector3(_Position.X + _characterTexture.Width - 5, _Position.Y + 10, 0),
                new Vector3(_Position.X + _characterTexture.Width, _Position.Y + _characterTexture.Height - 10, 0));

            generalBB = new BoundingBox(new Vector3(_Position.X, _Position.Y, 0), new Vector3(_Position.X + _characterTexture.Width, _Position.Y + _characterTexture.Height, 0));
            #endregion

            updateMovement(_keys);

            velocity += gravity;
            if (isThereIntersectionWithGround(_characterBB_BOTTOM))
            {
                jumpCounter = 0;
                velocity = Vector2.Zero;
            }
            else {if (jumpCounter == 0)jumpCounter = 1;}

            if (_keys.IsKeyDown(Keys.LeftShift) && _oldKeys.IsKeyUp(Keys.LeftShift) && jumpCounter < 2)
            {
                jump();
            }
            if (isThereContainmentWithGroundNONPARTY(_characterBB_TOP))
            {
                _Position.Y += 1;
                velocity.Y = 0.0f;
            } 
            _Position += velocity;


            _Position += theDirection * theSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            

            _oldKeys = _keys;
            oldGT = gameTime;
            
        }

        private bool isThereIntersectionWithGround(BoundingBox personBB)
        {
            foreach (BoundingBox b in groundBB)
            {
                if (b.Intersects(personBB))
                {
                    _Position.Y = b.Min.Y-2*_tileHeight;
                    return true;
                }
            }
            return false;
        }


        private bool isThereContainmentWithGroundNONPARTY(BoundingBox personBB)
        {
            foreach (BoundingBox b in groundBB)
            {
                if (b.Contains(personBB) == ContainmentType.Intersects)
                    return true;
            }
            return false;
        }

        public void drawCharacter()
        {
            _spriteBatch.Draw(_characterTexture, new Rectangle((int)_Position.X, (int)_Position.Y, _characterTexture.Width, _characterTexture.Height), Color.White);
            
            //_spriteBatch.DrawString(font, fallingBlocksArray[0].timeOneCanStayInIt.ToString(), new Vector2(100, 50), Color.Red);
            //_spriteBatch.DrawString(font, groundBB.Count.ToString(), new Vector2(100, 70), Color.Red);
        }

        #region IntersectPixel - we dont care
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
        #endregion

        public bool UpdateDeathMovement()
        {
            if (!stopWatch.IsRunning)
                stopWatch.Start();

            if (oldPosition!=_Position)
                stopWatch.Restart();
            oldPosition = _Position;

            if (stopWatch.ElapsedMilliseconds > 2000)
                return true;

            return false;
        }

        private void jump()
        {
            jumpCounter++;
            velocity.Y = -10;
        }

        private void updateMovement(KeyboardState cKeys)
        {
            if (currentState == State.walking)
            {
                theSpeed = Vector2.Zero;
                theDirection = Vector2.Zero;
                canMoveLeft = true;
                canMoveRight = true;

                if (cKeys.IsKeyDown(Keys.Right))
                {
                    foreach (BoundingBox b in groundBB)
                    {
                        if (b.Intersects(_characterBB_RIGHT))
                            canMoveRight = false;
                    }
                    if (canMoveRight)
                    {
                        theSpeed.X = MOV_SPEED;
                        theDirection.X = MOVE_RIGHT;
                        lastDirection = MOVE_RIGHT;
                    }
                }
                else if (cKeys.IsKeyDown(Keys.Left))
                {
                    foreach (BoundingBox b in groundBB)
                    {
                        if (b.Intersects(_characterBB_LEFT))
                            canMoveLeft = false;
                    }
                    if (canMoveLeft)
                    {
                        theSpeed.X = MOV_SPEED;
                        theDirection.X = MOVE_LEFT;
                        lastDirection = MOVE_LEFT;
                    }
                }
            }
        }

        //private float groundIsAtHeight()
        //{
        //    int xTile =(int) Math.Floor(_Position.X / _tileWidth);
        //    int yTile = (int)Math.Floor(_Position.Y / _tileHeight);
        //    if (xTile >= 0 && yTile >= 0)
        //    {
        //        for (int i = yTile; i < numberOfYTiles; i++)
        //        {
        //            if (_Tiles[xTile, i] != 9)
        //            {
        //                return i * _tileHeight - (_tileHeight * 2);
        //            }
        //        }
        //    }
        //    return 500000f;
        //}

    }
}