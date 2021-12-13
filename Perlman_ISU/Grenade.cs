//Author Name: Elad Perlman
//File Name: Grenade
//Project Name: Perlman_ISU
//Date Created: Dec 30th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of a grenade such as moving it, as well as drawing it 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Grenade
    {
        //Stores grenade's image and destination rectangle
        public Texture2D grdImg;
        public Rectangle grdRec;

        //Stores angle grenade was shot at
        private float angleShot;

        //Stores grenades speed
        private Vector2 grdSpeed;

        //Stores grenades speed factor
        private const int SPEED_MULT = 20;

        //Stores grendes friction values
        private Vector2 friction;

        //Stores grenade's explosion timer
        public float grdTimer = 0f;

        //Stores the amount the timers will icnrease by each update
        private const float TIME_INC = 1f / Game1.FPS;

        //Stores the radius which the grenade will deal damage
        private const int RAD_RANGE = 400;

        //Stores grenades min speed
        private const float MIN_SPEED = 0.4f;

        //Stores time grenade will explode at
        public int maxTime = 1;

        //Pre: grdImg, and grdRec are not null, angleShot is a valid float
        //Post: N/A
        //Description: Sets global variables equal to their local ones, and sets up grenades speed and friction 
        public Grenade(Texture2D grdImg, Rectangle grdRec, float angleShot)
        {
            this.grdRec = grdRec;
            this.grdImg = grdImg;
            
            //Adjusts the angle to the right direction
            this.angleShot = -1 * angleShot + 180;
            
            //Caculates grenades speed and frcition values
            grdSpeed.X = (float)Math.Sin(MathHelper.ToRadians(this.angleShot)) * SPEED_MULT;
            grdSpeed.Y = (float)Math.Cos(MathHelper.ToRadians(this.angleShot)) * SPEED_MULT;
            friction.X = -Math.Abs(grdSpeed.X / 20);
            friction.Y = Math.Abs(grdSpeed.Y / 20);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Calls MoveGrenade and returns if grenade has exploded yet
        public bool Update()
        {
            MoveGrenade();
            
            return GrenadeExplode();
        }
        
        //Pre: N/A 
        //Post: N/A
        //Description: Handles the physics of moving the grenade
        public void MoveGrenade()
        {
            //Checks if grenades's x speed is above the minimum value
            if (Math.Abs(grdSpeed.X) > MIN_SPEED)
            {
                //Checks if x speed is in the left direction, if so add friction in the oppisite diretion 
                if (grdSpeed.X < 0)
                {
                    grdSpeed.X += -friction.X;
                }

                //Executes if above if statement failed, if so add friction in the oppisite diretion the player is travelling in 
                else
                {
                    grdSpeed.X += friction.X;
                }
            }

            //Execute if above if statement failed, if so set x speed to zero
            else
            {
                grdSpeed.X = 0;
            }

            //Checks if grenade's y speed is above the minimum value
            if (Math.Abs(grdSpeed.Y) > MIN_SPEED)
            {
                //Checks if y speed is in the left direction, if so add friction in the oppisite diretion
                if (grdSpeed.Y < 0)
                {
                    grdSpeed.Y += friction.Y;
                }

                //Executes if above if statement failed, if so add friction in the oppisite diretion the player is travelling in
                else
                {
                    grdSpeed.Y += -friction.Y;
                }
            }

            //Execute if above if statement failed, if so set y speed to zero
            else
            {
                grdSpeed.Y = 0;
            }

            //Moves the grenade based on its speed
            grdRec.X += (int)grdSpeed.X;
            grdRec.Y += (int)grdSpeed.Y;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Increases grdTimer and returns if grenade has exploded yet or not
        public bool GrenadeExplode()
        {
            grdTimer += TIME_INC;

            if (grdTimer >= maxTime)
            {
                return true;
            }

            return false;
        }

        //Pre: center1 and center2 are both not null
        //Post: Returns the distance between both Vector2's
        //Description: Calculates and returns the distance between both Vector2's
        public double Distance(Vector2 center1, Vector2 center2)
        {
            return Math.Sqrt(Math.Pow(center1.X - center2.X, 2) + Math.Pow(center1.Y - center2.Y, 2));
        }

        //Pre: center is not null
        //Post: returns the damage an object will take based on its distance away from the grenade
        public float ObjectInRange(Vector2 center)
        {
            //Checks if distance is below the max amount, if so return the damage the object will take
            if (Distance(center, grdRec.Center.ToVector2()) <= RAD_RANGE)
            {
                return (float)(((RAD_RANGE - Distance(center, grdRec.Center.ToVector2())) / RAD_RANGE));
            }

            //Returns zero damage
            return 0f;
        }
        
        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws the grenade
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(grdImg, grdRec, Color.White);
        }
    }
}
