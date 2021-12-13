//Author Name: Elad Perlman
//File Name: ZombieEn
//Project Name: Perlman_ISU
//Date Created: Dec 24th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics and graphics of obstacle type ZombieEn, which inherates class Enemy
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class ZombieEn : Enemy
    { 
        //Stores the distance away from the player the enemy will stop at
        private double radius = 300;

        //Stores the enemy's bullet's image
        private Texture2D bulletImg;
        
        //Stores the shoot timer
        private float shootTimer = 0f;

        //Stores the time incriment for the timers every update
        private const float TIME_INC = 0.08f;

        //Stores the factor the boss's speed will be decreased by
        private const float SPEED_MULT = 50;

        //Stores gun shot sound effect
        private SoundEffect gunShotSnd;

        //Pre: enImg and bulletImg are not null, rndLoc is not nullm and health is a positive integer
        //Post: N/A
        //Description: sets bulletImg to its local one
        public ZombieEn(Texture2D enImg, Texture2D bulletImg, Vector2 rndLoc, int health, SoundEffect gunShotSnd) : base(enImg, rndLoc, health)
        {
            this.bulletImg = bulletImg;
            this.gunShotSnd = gunShotSnd;
        }

        //Pre: all variables are not null
        //Post: N/A
        //Decription: Handle's all of the zombie enemy's playable functions
        public override bool Update(Line[] playerLines, Rectangle playerRec, List<Bullet> playerBul, List<Enemy> enemys)
        {
            //Creates a new angledRec instance, and calls its NewRec function
            angledRec = new AngledRec();
            angledRec.NewRec(enRec, angle);

            //Sets each line to its corresponding location
            enLines[0] = new Line(angledRec.topLeft, angledRec.topRight);
            enLines[1] = new Line(angledRec.topRight, angledRec.bottomRight);
            enLines[2] = new Line(angledRec.bottomRight, angledRec.bottomLeft);
            enLines[3] = new Line(angledRec.bottomLeft, angledRec.topLeft);
            
            //Calls EnemyDamage
            EnemyDamage(playerBul);

            //Calculates the angle of the zombie enemy if its x position is less than the player's center x position
            angle = enRec.X < playerRec.X + playerRec.Width / 2
                ? MathHelper.ToDegrees((float)Math.Atan2(playerRec.Center.Y - enRec.Center.Y, playerRec.Center.X - enRec.Center.X))
                : MathHelper.ToDegrees((float)Math.Atan2(enRec.Center.Y - playerRec.Center.Y, enRec.Center.X - playerRec.Center.X));

            //calculates the x and y distances away from the player 
            disp.X = playerRec.X - enRec.X;
            disp.Y = playerRec.Y - enRec.Y;

            //Calculates the boss's speed
            speed = disp / SPEED_MULT;

            //Checks if zombie enemy is not within the player's radius
            if (!RadiusCal(playerRec))
            {
                //stores if enemy has collided with another enemy
                bool enemyCol = false;

                for (int i = 0; i < enemys.Count; i++)
                {
                    //Checks if enemy has collided with another enemy and is furthor from player than the other enemy it collided with
                    if (enLines != enemys[i].enLines && enemys[i].enLines[0] != null && enCol.AngledBoxCol(enemys[i].enLines, enLines) &&
                        Distance(playerRec.Center.ToVector2(), enRec.Center.ToVector2()) >
                        Distance(playerRec.Center.ToVector2(), enemys[i].enRec.Center.ToVector2()))
                    {
                        //set enemyCol to true and break out of loop
                        enemyCol = true;
                        break;
                    }
                }

                //Checks if enemy didnt collide with other enemies, if so move enemy
                if (!enemyCol)
                {
                    enRec.X += (int)speed.X;
                    enRec.Y += (int)speed.Y;
                }
            }

            //creates a new destination rectangle for bullet
            Rectangle bulletRec = new Rectangle(enRec.X, enRec.Y, bulletImg.Width, bulletImg.Height);

            //Increases shoot timer
            shootTimer += TIME_INC;

            //Checks if shoot timer is above its max time
            if (shootTimer > 2)
            {
                //checks if boss is ahead of the player in the x direction, if so add new bullet, and plays gun sound effect
                if (enRec.X > playerRec.X + playerRec.Width / 2)
                {
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle + 270));
                    gunShotSnd.CreateInstance().Play();
                }

                //Executes if above if statment is false, if so add new bullet, and plays gun sound effect
                else
                {
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle + 90));
                    gunShotSnd.CreateInstance().Play();
                }

                //reset shoot timer
                shootTimer = 0f;
            }

            //Incriments through each bullet
            for (int i = 0; i <bullets.Count; i++)
            {
                //Calls MoveBullet for current bullet
                bullets[i].MoveBullet();

                //Checks if bullet's life span is over, if so remove bullet and break out of loop
                if (bullets[i].MoveBullet())
                {
                    bullets.Remove(bullets[i]);
                    break;
                }
            }

            //Returns that enemy didnt collide with player
            return false;
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Description: Draws all the zombie and its bullets
        public override void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            base.Draw(spriteBatch, playerRec);
            
            foreach(Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        //Pre: playerRec is not null
        //Post: Returns if boss is within player's radius
        //Description: Checks if boss is within player's radius and returns the result
        private bool RadiusCal(Rectangle playerRec)
        {
            if (Math.Abs(Math.Sqrt(Math.Pow((enRec.Center.X - playerRec.Center.X), 2) + (Math.Pow((enRec.Center.Y - playerRec.Center.Y), 2)))) <= radius)
            {
                return true;
            }
            
            return false;
        }
    }
}
