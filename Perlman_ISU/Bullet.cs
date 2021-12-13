//Author Name: Elad Perlman
//File Name: Button
//Project Name: Perlman_ISU
//Date Created: Jan 17th 2020
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of a bullet, as well as drawing it 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Bullet
    {
        //Stores the bullets speeds in both x and y directions 
        private Vector2 bulletSpeed;

        //Stores the bullet's image and destination rectangle
        private Rectangle bulletRec;
        private Texture2D bulletImg;

        //Stores the bullets rotated rectangle's lines
        public Line[] bulLines = new Line[4];

        //Stores the angle the bullet is shot at
        private float angle;

        //Stores bullets life span timer
        private float timer = 0f;

        //Stores bullet's life span
        private const int MAX_TIME = 5;

        //Stores the amount of time will increase each update
        private const float TIME_INC = 0.05f;

        //Stores the value which the speed will increase by
        private const int SPEED_MULT = 20;


        //Pre: bulletRec and bulletImg are both not null
        //Post: N/A
        //Description: Sets up the bullet image and rectangle 
        public Bullet(Rectangle bulletRec, Texture2D bulletImg, float angle)
        {
            //Sets global bullet image and rectangle equal to the local ones
            this.bulletRec = bulletRec;
            this.bulletImg = bulletImg;
            this.angle = angle;

            //Sets up the rectangle's lines
            bulLines[0] = new Line(bulletRec.Location.ToVector2(), new Vector2(bulletRec.Right, bulletRec.Top));
            bulLines[1] = new Line(new Vector2(bulletRec.Right, bulletRec.Top), new Vector2(bulletRec.Right, bulletRec.Bottom));
            bulLines[2] = new Line(new Vector2(bulletRec.Right, bulletRec.Bottom), new Vector2(bulletRec.Left, bulletRec.Bottom));
            bulLines[3] = new Line(new Vector2(bulletRec.Left, bulletRec.Bottom), bulletRec.Location.ToVector2());
        }

        //Pre: N/A
        //Post: N/A
        //Description: Moves the bullets based on their speeds 
        public bool MoveBullet()
        {
            //Updates the rectangle's lines
            bulLines[0] = new Line(bulletRec.Location.ToVector2(), new Vector2(bulletRec.Right, bulletRec.Top));
            bulLines[1] = new Line(new Vector2(bulletRec.Right, bulletRec.Top), new Vector2(bulletRec.Right, bulletRec.Bottom));
            bulLines[2] = new Line(new Vector2(bulletRec.Right, bulletRec.Bottom), new Vector2(bulletRec.Left, bulletRec.Bottom));
            bulLines[3] = new Line(new Vector2(bulletRec.Left, bulletRec.Bottom), bulletRec.Location.ToVector2());

            //Sets up the bullet's speeds
            bulletSpeed.X = (float)Math.Sin(MathHelper.ToRadians(angle)) * SPEED_MULT;
            bulletSpeed.Y = (float)Math.Cos(MathHelper.ToRadians(angle)) * SPEED_MULT;

            //Shifts the bullet based on its x and y speeds
            bulletRec.X += (int)bulletSpeed.X;
            bulletRec.Y -= (int)bulletSpeed.Y;

            //Increases bullet's timer
            timer += TIME_INC;

            //Checks if bullet's life span is over, if so return true
            if (timer >= MAX_TIME)
            {
                return true;
            }

            //Returns false
            return false;
        }       

        //Pre: rec is not null 
        //Post: Returns if the bullet collided with the given rectangle
        //Description: Returns if the bullet collided with the given rectangle
        public bool CheckCollision(Rectangle rec)
        {
            return !((rec.X + rec.Width) < bulletRec.X || rec.X > (bulletRec.X + bulletRec.Width) || rec.Y > (bulletRec.Y + bulletRec.Height) || (rec.Y + rec.Height) < bulletRec.Y);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws the bullet
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletImg, bulletRec, Color.White);
        }

        //Pre: N/A
        //Post: Returns the bullet's rectangle
        //Description: Returns the bullet's rectangle
        public Rectangle GetRec()
        {
            return bulletRec;
        }
    }
}

