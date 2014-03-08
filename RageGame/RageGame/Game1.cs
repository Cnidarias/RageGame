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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        LoadDrawLevel ldl;
        Spike spike;
        Character character;
        Triggers triggers;
        mainGun _mainGun;
        spamButtonDeath sBD = new spamButtonDeath();
        KeyboardState keys, oldKeys;
        bool dead = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadFirstLevel("1");

            graphics.PreferredBackBufferHeight = ldl.tileHeight * ldl.yTile;
            graphics.PreferredBackBufferWidth = ldl.tileWidth * ldl.xTile;
            graphics.ApplyChanges();
            
        }
        public void LoadFirstLevel(string Level)
        {
            dead = false;
            ldl = new LoadDrawLevel(Level, Content, spriteBatch);
            spike = new Spike(Level, Content, spriteBatch);
            triggers = new Triggers(Level);
            character = new Character(Content, spriteBatch, new Vector2(10, 10));
            character.loadGroundBB(ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight, ldl.tileNumber, Level);
            _mainGun = new mainGun(Content, spriteBatch);
        }
        public void LoadNewLevel(string Level)
        {
            dead = false;
            ldl = new LoadDrawLevel(Level, Content, spriteBatch);
            spike = new Spike(Level, Content, spriteBatch);
            triggers = new Triggers(Level);
            character = new Character(Content, spriteBatch, character._Position);
            character.loadGroundBB(ldl.xTile, ldl.yTile, ldl.tileWidth, ldl.tileHeight, ldl.tileNumber, Level);
            _mainGun = new mainGun(Content, spriteBatch);
        }


        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            keys = Keyboard.GetState();
            
            #region IF DEBUG
#if DEBUG
            if (keys.IsKeyDown(Keys.Escape))
                this.Exit();
#endif
            #endregion
            if (!dead)
            {
                dead = sBD.spamUpdate(keys);
                dead = character.UpdateDeathMovement();
            }
            character.Update(gameTime);
            ldl.changeTiles(character.processFallingTrigers());
            spike.triggerSpike(triggers.Update(character._Position, character._characterTexture));
            if (!spike.SpikeUpdate(character._Position))
                dead = true;
            _mainGun.UpdateGun(character._Position, character.lastDirection);

            checkNewLevel();

            if (keys.IsKeyDown(Keys.R) && oldKeys.IsKeyUp(Keys.R))
                LoadNewLevel("1");

            oldKeys = keys;
            base.Update(gameTime);
        }

        private void checkNewLevel()
        {
            if (character._Position.X > graphics.GraphicsDevice.Viewport.Width)
            {
                LoadNewLevel(ldl.right);
                character._Position.X = 5;
            }
            else if (character._Position.X < 0)
            {
                LoadNewLevel(ldl.left);
                character._Position.X = graphics.GraphicsDevice.Viewport.Width - character._characterTexture.Width - 5;
            }
            else if (character._Position.Y > graphics.GraphicsDevice.Viewport.Height)
            {
                LoadNewLevel(ldl.bottom);
                character._Position.Y = 5;
            }
            else if (character._Position.Y < 0)
            {
                LoadNewLevel(ldl.top);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!dead)
                GraphicsDevice.Clear(Color.SkyBlue);
            else
                GraphicsDevice.Clear(Color.Red);
            spriteBatch.Begin();
            ldl.drawTiles();
            character.drawCharacter();
            spike.SpikeDraw();
            _mainGun.drawGun();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
