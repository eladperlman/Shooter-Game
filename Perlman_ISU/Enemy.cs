//Author Name: Elad Perlman
//File Name: Enemy
//Project Name: Perlman_ISU
//Date Created: Dec 23rd 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of an enemy such as moving it, shooting, and its collision, as well as drawing it 
using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Enemy
    {
        //Stores enemy's image and rectangle
        public Texture2D enImg;
        public Rectangle enRec;

        //Stores all enemy's bullets
        public List<Bullet> bullets = new List<Bullet>();

        //Stores enemy's speed and distance away from player
        protected Vector2 speed;
        protected Vector2 disp;

        //Stores the enemy's rotation angle
        protected float angle;

        //Stores random variable
        protected Random rnd = new Random();

        //Stores enemy's collision variable
        public PerPixelCol enCol;

        //Stores enemy's rotated rectangle
        public AngledRec angledRec;

        //Stores the 4 lines of the rotated rectangle
        public Line[] enLines = new Line[4];

        //Stores enemy's health
        public int health;

        //Stores damage an enemy takes from player's bullet
        protected const int BUL_DAMG = -10;

        //Stores enemy's speed's decreasing factor
        private const int SPEED_MULT = 40;

        //Pre: enImg and rndLoc are not null, health is a positive integer
        //Post: N/A
        //Description: Sets up enemy's health and image, as well as its destination rectangle and its location
        public Enemy(Texture2D enImg, Vector2 rndLoc, int health)
        {
            this.enImg = enImg;
            this.health = health;

            //Sets up enemy's rectangle, and its location
            enRec = new Rectangle(0, 0, enImg.Width / 4, enImg.Height / 4);
            enRec.X = (int)rndLoc.X;
            enRec.Y = (int)rndLoc.Y;

            //Sets up enemy's collision class
            enCol = new PerPixelCol();
        }
        
        //Pre: All variables are not null
        //Post: Returns if enemy collided with player
        //Description: Handles all the physics and behaviour of the enemy
        public virtual bool Update(Line[] playerLines, Rectangle playerRec, List<Bullet> playerBul, List<Enemy> enemys)
        {
            //Sets up enemys rotated rectangle and its four lines
            angledRec = new AngledRec();
            angledRec.NewRec(enRec, angle);
            enLines[0] = new Line(angledRec.topLeft, angledRec.topRight);
            enLines[1] = new Line(angledRec.topRight, angledRec.bottomRight);
            enLines[2] = new Line(angledRec.bottomRight, angledRec.bottomLeft);
            enLines[3] = new Line(angledRec.bottomLeft, angledRec.topLeft);
    
            //Calls EnemyDamage 
            EnemyDamage(playerBul);

            //Calculates the angle of the enemy if its x position is less than the player's center x position
            angle = enRec.X < playerRec.X + playerRec.Width / 2
                ? MathHelper.ToDegrees((float)Math.Atan2(playerRec.Center.Y - enRec.Center.Y, playerRec.Center.X - enRec.Center.X))
                : MathHelper.ToDegrees((float)Math.Atan2(enRec.Center.Y - playerRec.Center.Y, enRec.Center.X - playerRec.Center.X));

            //Calculates distance away from player
            disp.X = playerRec.X - enRec.X;
            disp.Y = playerRec.Y - enRec.Y;

            //Calculates the speed of the enemy
            speed = disp / SPEED_MULT;

            //Checks if enemy didnt collide with player, if so move enemy based on its speed
            if (!enCol.AngledBoxCol(playerLines, enLines))
            {
                enRec.X += (int)speed.X;
                enRec.Y += (int)speed.Y;
            }

            //Execute if enemy collided with player, and return true
            else
            {
                return true;
            }

            //Return that enemy didn't collide with player
            return false;
        }

        //Pre: playerBul is not null
        //Post: N/A
        //Description: Checks if enemy collided with any of player's bullets, if so removes health
        public void EnemyDamage(List<Bullet> playerBul)
        {
            //Incriments through player's bullets
            for (int i = 0; i < playerBul.Count; i++)
            {
                //Checks if enemy collided with player bullet
                if (enCol.AngledBoxCol(playerBul[i].bulLines, enLines))
                {
                    //Decreases enemys health, removes player's bullet and exits loop
                    health += BUL_DAMG;
                    playerBul.RemoveAt(i);
                    break;               
                }
            }
        }

        //Pre: both Vector2's are not null
        //Post: returns the distance between the two positions
        //Description: Calculates and returns the distance between the two positions
        public double Distance(Vector2 center1, Vector2 center2)
        {
            return Math.Sqrt(Math.Pow(center1.X - center2.X, 2) + Math.Pow(center1.Y - center2.Y, 2));
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Description: Draws the enemy based on its position relative to the player's position
        public virtual void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            //Checks if enemy is ahead of player, if so draw the enemy flipped horizontally 
            if (enRec.X > playerRec.X + playerRec.Width / 2)
            {
                spriteBatch.Draw(enImg, enRec, null, Color.White, MathHelper.ToRadians(angle), new Vector2(enImg.Width / 2, enImg.Height / 2), SpriteEffects.FlipHorizontally, 0);
            }
            
            //Executes if enemy is behind player, if so draws enemy
            else
            {
                spriteBatch.Draw(enImg, enRec, null, Color.White, MathHelper.ToRadians(angle), new Vector2(enImg.Width / 2, enImg.Height / 2), SpriteEffects.None, 0);
            }
        }
    }
}
