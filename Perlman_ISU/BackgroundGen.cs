//Author Name: Elad Perlman
//File Name: BackgroundGen
//Project Name: Perlman_ISU
//Date Created: Dec 17th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic of spawning in backgrounds based on player's position 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class BackgroundGen
    {
        //Stores background's image and all of its rectangles 
        private Texture2D bgImg;
        private List<Rectangle> bgRec = new List<Rectangle>();

        //Stores background's width and height
        private int width = Game1.windowWidth;
        private int height = Game1.windowHeight;

        //Stores the rectangle of the box player is currently on
        private Vector2 curBox;

        //Pre: bgImg is not null
        //Post: N/A
        //Description: Sets bgImg equal to its local one, and adds a new background
        public BackgroundGen(Texture2D bgImg)
        {
            this.bgImg = bgImg;

            bgRec.Add(new Rectangle(0, 0, Game1.windowWidth, Game1.windowHeight));
        }

        //Pre: player is not null
        //Post: N/A
        //Description: Generates a background based on where the player is standing, and generates adjacent backgrounds
        public void Generate(Rectangle player)
        {
            //Incriments through each background
            for (int i = 0; i < bgRec.Count; i++)
            {
                //Checks if player is colliding with background
                if (CollisionDec(bgRec[i], player))
                {
                    //Sets current background's position to the player's position and deletes all backgrounds
                    curBox.X = bgRec[i].X;
                    curBox.Y = bgRec[i].Y;
                    bgRec.Clear();

                    //Loops through 3 x values
                    for (int x = -1; x < 2; x++)
                    {
                        //Loops through 3 y values
                        for (int y = -1; y < 2; y++)
                        {
                            //Generates a new Rectangle based on the x and y multipliers
                            bgRec.Add(new Rectangle(x * width + (int)curBox.X, y * height + (int)curBox.Y, width, height));
                        }
                    }
                }
            }
        }

        //Pre: rec1 and rec2 are not null
        //Post: Returns if both rectangles are colliding
        //Description: Checks if both rectangles are collding and returns the result
        public bool CollisionDec(Rectangle rec1, Rectangle rec2)
        {
            return !((rec1.X + rec1.Width) < rec2.X || rec1.X > (rec2.X + rec2.Width) || rec1.Y > (rec2.Y + rec2.Height) || (rec1.Y + rec1.Height) < rec2.Y);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the backgrounds
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Rectangle rec in bgRec)
            {
                spriteBatch.Draw(bgImg, rec, Color.White);
            }
        }
    }
}
