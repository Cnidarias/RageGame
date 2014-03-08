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
using System.IO;

namespace RageGame
{
    class LoadDrawLevel
    {
        ContentManager _Content;
        SpriteBatch _spriteBatch;
        string _levelName = "Content\\Maps\\lvl";
        public string right, top, left, bottom;
        Texture2D[] _tileTexture;
        int _tileHeight, _tileWidth, _xTile, _yTile, _numberOfTiles;
        public int tileHeight, tileWidth, xTile, yTile;
        int[,] _tileNumber;
        public int[,] tileNumber;
        StreamReader _sr;

        public LoadDrawLevel(String levelName, ContentManager Content, SpriteBatch spriteBatch)
        {
            _levelName += levelName + ".txt";
            _Content = Content;
            _spriteBatch = spriteBatch;
            readFile();
        }
        public void changeTiles(Vector2 [] idsToBeChanged)
        {
            for (int i = 0; i < idsToBeChanged.Length; i++)
            {
                tileNumber[(int)idsToBeChanged[i].X, (int)idsToBeChanged[i].Y] = 9;
            }
        }

        public void ReloadLevel(String levelName)
        {
            _levelName = "Content\\Maps\\lvl" + levelName + ".txt";
            readFile();
        }

        private void readFile()
        {
            _sr = new StreamReader(_levelName);

            _numberOfTiles = Convert.ToInt32(_sr.ReadLine());
            _xTile = Convert.ToInt32(_sr.ReadLine());
            _yTile = Convert.ToInt32(_sr.ReadLine());
            _tileWidth = Convert.ToInt32(_sr.ReadLine());
            _tileHeight = Convert.ToInt32(_sr.ReadLine());

            _tileNumber = new int[_xTile, _yTile];
            _tileTexture = new Texture2D[_numberOfTiles];

            _sr.ReadLine();

            right = _sr.ReadLine();
            top = _sr.ReadLine();
            left = _sr.ReadLine();
            bottom = _sr.ReadLine();

            _sr.ReadLine();

            for (int i = 0; i < _xTile; i++)
            {
                for (int j = 0; j < _yTile; j++)
                {
                    _tileNumber[i, j] = Convert.ToInt32(_sr.ReadLine());
                }
            }
            for (int i = 0; i < _numberOfTiles; i++)
            {
                _tileTexture[i] = _Content.Load<Texture2D>("Graphics\\Walls\\" + i.ToString());
            }

            xTile = _xTile;
            yTile = _yTile;
            tileHeight = _tileHeight;
            tileWidth = _tileWidth;
            tileNumber = _tileNumber;
        }

        public void drawTiles()
        {
            for (int i = 0; i < _xTile; i++)
            {
                for (int j = 0; j < _yTile; j++)
                {
                    _spriteBatch.Draw(_tileTexture[_tileNumber[i, j]], new Rectangle(i * _tileWidth, j * _tileHeight, _tileWidth, _tileHeight), Color.White);
                }
            }
        }


    }
}
