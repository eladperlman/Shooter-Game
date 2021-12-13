//Author Name: Elad Perlman
//File Name: Boss
//Project Name: Perlman_ISU
//Date Created: Jan 2th 2020
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of a boss such as moving it, shooting, and its collision, as well as drawing it 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Boss
    {
        //Stores the boss's image and destination rectangle
        private Texture2D bossImg;
        public Rectangle bossRec;

        //Stores the boss's rotated rectangle and the rectangle four lines
        public AngledRec angledRec;
        private Line[] bossLines = new Line[4];

        //Stores the boss's speed and the friction value 
        private Vector2 bossSpeed = new Vector2(20, 0);
        private const float FRICTION = -0.8f;

        //Stores the direction the boss will spawn in
        private int moveMult;
        
        //Stores the boss's health, and its orginal health
        public int health;
        public int ogHealth;

        //Stores all the boss's bullets
        public List<Bullet> bullets = new List<Bullet>();

        //Stores the distance away from the player that the boss will stop at
        private double radius = 300;
        
        //Stores the boss's shooting timer
        private float shootTimer = 0f;

        //Stores the boss's angle
        private float angle;

        //Stores the amount each timer will increase by each update
        private const float TIME_INC = 0.05f;
        
        //Stores the boss's bullet's image
        private Texture2D bulletImg;

        //Store the boss's distance away from the player
        private Vector2 disp;

        //Stores if boss if moving
        private bool moveBoss = false;

        //Stores the boss's collision class
        public PerPixelCol col;

        //Stores the player's bullet damage 
        private const int BUL_DAMG = -10;

        //Stores the timer's max time
        private const int MAX_TIME = 2;

        //Stores the boss's minimum speed
        private const float MIN_SPEED = 0.4f;

        //Stores the factor the boss's speed will be decreased by
        private const float SPEED_MULT = 50;

        //Pre: BossImg, bulletImg, and bossRec are not null, move mult has a value of either 1 or -1, and health is a positive integer
        //Post: N/A
        //Description: Sets all global variables to their local ones
        public Boss(Texture2D bossImg, Rectangle bossRec, int moveMult, int health, Texture2D bulletImg)
        {
            this.bossImg = bossImg;
            this.bossRec = bossRec;
            this.moveMult = moveMult;
            this.health = health;
            this.bulletImg = bulletImg;
            ogHealth = health;

            //Creates a new collision instance
            col = new PerPixelCol();

            //Creates a new angledRec instance, and calls its NewRec function
            angledRec = new AngledRec();
            angledRec.NewRec(bossRec, angle);
        }

        //Pre: all variables are not null
        //Post: N/A
        //Description: Calls all of the boss's playerable functions
        public void Update(Rectangle playerRec, List<Bullet> playerBul, List<Boss> bosses)
        {
            //Checks if boss has just spawned, if so call SpawnBoss
            if (!moveBoss)
            {
                SpawnBoss();
            }
            
            //Executes if above if statment is false, if so calls MoveBoss
            else
            {
                MoveBoss(playerRec, playerBul, bosses);
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Spawns the boss, and blows him away in its given direction
        public void SpawnBoss()
        {
            //Moves the boss horizontally by its speed and direction
            bossRec.X += (int)(bossSpeed.X * moveMult);

            //Checks if boss's speed is above the minimum amount, if so add friction to the speed
            if (bossSpeed.X > MIN_SPEED)
            {
                bossSpeed.X += FRICTION;
            }
            
            //Execute if above if statment was false
            else
            {
                //Sets boss's speed to zero and sets moveBoss to true
                bossSpeed.X = 0;
                moveBoss = true;
            }
        }

        //Pre: all variables are not null
        //Post: N/A
        //Decription: Handle's all of the boss's playable functions
        public void MoveBoss(Rectangle playerRec, List<Bullet> playerBul, List<Boss> bosses)
        {
            //Creates a new angledRec instance, and calls its NewRec function
            angledRec = new AngledRec();
            angledRec.NewRec(bossRec, angle);

            //Sets each line to its corresponding location
            bossLines[0] = new Line(angledRec.topLeft, angledRec.topRight);
            bossLines[1] = new Line(angledRec.topRight, angledRec.bottomRight);
            bossLines[2] = new Line(angledRec.bottomRight, angledRec.bottomLeft);
            bossLines[3] = new Line(angledRec.bottomLeft, angledRec.topLeft);

            //Calls BossDamage
            BossDamage(playerBul);

            //Calculates the angle of the boss if its x position is less than the player's center x position
            angle = bossRec.X < playerRec.X + playerRec.Width / 2
                ? MathHelper.ToDegrees((float)Math.Atan2(playerRec.Center.Y - bossRec.Center.Y, playerRec.Center.X - bossRec.Center.X))
                : MathHelper.ToDegrees((float)Math.Atan2(bossRec.Center.Y - playerRec.Center.Y, bossRec.Center.X - playerRec.Center.X));

            //calculates the x and y distances away from the player 
            disp.X = playerRec.X - bossRec.X;
            disp.Y = playerRec.Y - bossRec.Y;

            //Calculates the boss's speed
            bossSpeed = disp / SPEED_MULT;

            //Checks if boss is not within the player's radius
            if (!RadiusCal(playerRec))
            {
                //stores if enemy has collided with another enemy
                bool enemyCol = false;

                //Incriments through every boss
                for (int i = 0; i < bosses.Count; i++)
                {
                    //Checks if boss has collided with another boss and is furthor from player than the other boss it collided with
                    if (bossLines != bosses[i].bossLines && bosses[i].bossLines[0] != null && col.AngledBoxCol(bosses[i].bossLines, bossLines) &&
                        Distance(playerRec.Center.ToVector2(), bossRec.Center.ToVector2()) >
                        Distance(playerRec.Center.ToVector2(), bosses[i].bossRec.Center.ToVector2()))
                    {
                        //set enemyCol to true and break out of loop
                        enemyCol = true;
                        break;
                    }
                }

                //Checks if boss didnt collide with other bosses, if so move boss
                if (!enemyCol)
                {
                    bossRec.X += (int)bossSpeed.X;
                    bossRec.Y += (int)bossSpeed.Y;
                }
            }

            //creates a new destination rectangle for bullet
            Rectangle bulletRec = new Rectangle(bossRec.X, bossRec.Y, bulletImg.Width, bulletImg.Height);

            //Increases shoot timer
            shootTimer += TIME_INC;

            //Checks if shoot timer is above its max time
            if (shootTimer > MAX_TIME)
            {
                //checks if boss is ahead of the player in the x direction, if so add new bullet 
                if (bossRec.X > playerRec.X + playerRec.Width / 2)
                {
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle + 270));
                }
                
                //Executes if above if statment is false, if so add new bullet
                else
                {
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle + 90));
                }
                
                //reset shoot timer
                shootTimer = 0f;
            }

            //Incriments through each bullet
            for (int i = 0; i < bullets.Count; i++)
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
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: None
        //Description: Draws the boss and its bullets
        public void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            //Draws the boss flipped if it is ahead of the player in the x direction
            if (bossRec.X > playerRec.X + playerRec.Width / 2)
            {
                spriteBatch.Draw(bossImg, bossRec, null, Color.White, MathHelper.ToRadians(angle), new Vector2(bossImg.Width / 2, bossImg.Height / 2), SpriteEffects.FlipHorizontally, 0);
            }

            //Draws the boss regularly if it is not ahead of the player in the x direction
            else
            {
                spriteBatch.Draw(bossImg, bossRec, null, Color.White, MathHelper.ToRadians(angle), new Vector2(bossImg.Width / 2, bossImg.Height / 2), SpriteEffects.None, 0);
            }

            //Draws the boss's bullets
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        //Pre: playerRec is not null
        //Post: Returns if boss is within player's radius
        //Description: Checks if boss is within player's radius and returns the result
        private bool RadiusCal(Rectangle playerRec)
        {
            if (Math.Abs(Math.Sqrt(Math.Pow((bossRec.Center.X - playerRec.Center.X), 2) + (Math.Pow((bossRec.Center.Y - playerRec.Center.Y), 2)))) <= radius)
            {
                return true;
            }

            return false;
        }

        //Pre: playerBul is not null
        //Post: N/A
        //Description: Checks if boss collided with the player's bullets and removes health from boss if so
        public void BossDamage(List<Bullet> playerBul)
        {
            //Incriments through each player bullet
            for (int i = 0; i < playerBul.Count; i++)
            {
                //Checks if bullet and player collided
                if (col.AngledBoxCol(playerBul[i].bulLines, bossLines))
                {
                    //Removes health from boss, and removes player bullet as well as break out of the loop
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
    }
}
