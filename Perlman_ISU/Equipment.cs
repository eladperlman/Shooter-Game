//Author Name: Elad Perlman
//File Name: Equipment
//Project Name: Perlman_ISU
//Date Created: Jan 5th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of any type of equipment
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Equipment
    {
        //Stores random variable
        private Random rnd = new Random();
        
        //Stores equipment's image and destination rectangle
        private Texture2D eqImg;
        private Rectangle eqRec;

        //Stores equipment's trancperancy value
        public float transVal = 0f;

        //Stores the amount the equipment's trancperancy will increase by
        private const float TRANS_INC = 0.06f;

        //Stores the max trancperancy value
        private const int TRANS_MAX = 1;

        //Stores the equipments 4 rotated rectangle lines
        private Line[] recLines = new Line[4];

        //Stores collision variable
        private PerPixelCol col;

        //Pre: eqImg is not null, Sets up the equipments rectangle and all its lines, as well as its collision class
        public Equipment(Texture2D eqImg)
        {
            this.eqImg = eqImg;

            eqRec = new Rectangle(rnd.Next(0, Game1.windowWidth - eqImg.Width / 4), rnd.Next(0, Game1.windowHeight - eqImg.Height / 4), eqImg.Width / 4, eqImg.Height / 4);

            col = new PerPixelCol();
                   
            //Sets up all the rectangle's lines based on their four vertacies
            recLines[0] = new Line(eqRec.Location.ToVector2(), new Vector2(eqRec.Right, eqRec.Top));
            recLines[1] = new Line(new Vector2(eqRec.Right, eqRec.Top), new Vector2(eqRec.Right, eqRec.Bottom));
            recLines[2] = new Line(new Vector2(eqRec.Right, eqRec.Bottom), new Vector2(eqRec.Left, eqRec.Bottom));
            recLines[3] = new Line(new Vector2(eqRec.Left, eqRec.Bottom), eqRec.Location.ToVector2());
        }

        //Pre: N/A
        //Post: N/A
        //Description: increases the transperancy value and sets it to 1 when it goes over its max value
        public void TransSpawn()
        {
            transVal += TRANS_INC;
                        
            if (transVal >= TRANS_MAX)
            {
                transVal = 1;
            }
        }

        //Pre: playerLines are not null
        //Post: returns if equipment collided with player
        //Description: Checks if equipment collided with player and returns the result
        public bool ColCheck(Line[] playerLines)
        {   
            if (col.AngledBoxCol(recLines, playerLines))
            {
                return true;
            }
                        
            return false;
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws the equipment
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(eqImg, eqRec, Color.Multiply(Color.White, transVal));
        }
    }
}
