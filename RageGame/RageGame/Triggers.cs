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
    public struct triggerStruct
    {
        public BoundingBox BB;
        public int[] IDsToBeActivated;
    }
    class Triggers
    {
        StreamReader reader;
        List<triggerStruct> trigList = new List<triggerStruct>();
        triggerStruct[] triggerArray;

        public Triggers(string path)
        {
            reader = new StreamReader("Content\\Maps\\Death\\triggerslvl" + path + ".txt");
            loadTriggers();
        }

        private void loadTriggers()
        {
            while (!reader.EndOfStream)
            {
                triggerStruct s = new triggerStruct();
                s.BB = new BoundingBox(new Vector3((float)Convert.ToDouble(reader.ReadLine()), (float)Convert.ToDouble(reader.ReadLine()), 0), 
                    (new Vector3((float)Convert.ToDouble(reader.ReadLine()), (float)Convert.ToDouble(reader.ReadLine()), 0)));
                int x = Convert.ToInt32(reader.ReadLine());
                s.IDsToBeActivated = new int[x];
                for (int i = 0; i < s.IDsToBeActivated.Length; i++)
                {
                    s.IDsToBeActivated[i] = Convert.ToInt32(reader.ReadLine());
                }

                trigList.Add(s);
            }
            reader.Close();
            triggerArray = trigList.ToArray();
        }

        public int[] Update(Vector2 characterPosition, Texture2D characterTexture)
        {
            int counter = 0;
            List<int> l = new List<int>();
            int[] IDsToBeReturned;

            BoundingBox characterBox = new BoundingBox(new Vector3(characterPosition.X, characterPosition.Y, 0),
                new Vector3(characterPosition.X + characterTexture.Width, characterPosition.Y + characterTexture.Height, 0));
            for (int i = 0; i < triggerArray.Length; i++)
            {
                if (characterBox.Intersects(triggerArray[i].BB))
                {
                    counter++;
                    for (int j = 0; j < triggerArray[i].IDsToBeActivated.Length; j++)
                    {
                        int x = triggerArray[i].IDsToBeActivated[j];
                        l.Add(x);
                    }
                }
            }
            IDsToBeReturned = l.ToArray();
            return IDsToBeReturned;
        }
    }
}
