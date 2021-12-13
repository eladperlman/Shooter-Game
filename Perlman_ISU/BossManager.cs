//Author Name: Elad Perlman
//File Name: BossManager
//Project Name: Perlman_ISU
//Date Created: Jan 5th 2020
//Date Modified: Jan 17th 2020
//Description: This class is in charge of handling all the bosses and their functionallity
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
    class BossManager
    {
        //Stores all the current bosses
        private List<Boss> boss = new List<Boss>();
        
        //Stores the boss's and its bullet's image
        private Texture2D bossImg;
        private Texture2D bulletImg;

        //Stores the explosion spritesheet 
        private Texture2D exImg;

        //Stores the boss's destination rectangle
        private Rectangle bossRec;

        //Stores the boss's width and height
        private int bossWidth;
        private int bossHeight;

        //Stores the boss's orginal health
        private int ogBossHealth = 100;

        //Stores the amount of boss's that died
        private int dieCount = 0;
        
        //Stores the maximum amount of boss's that can die
        private int maxBoss;

        //Stores the maximum damage a boss can take from a grenade
        private const int MAX_GRD_DAMG = 150;

        //Stores the damage the boss's bullet will deal
        private const int BUL_DAMG = -3;

        //Stores the animations of the boss's and grenade's explosions
        private List<Animation> bossExAni = new List<Animation>();
        private List<Animation> grdExAni = new List<Animation>();

        //Pre: all Texture2D's are not null, and maxBoss is a positive integer
        //Post: N/A
        //Description: Sets all global varible's to their local ones
        public BossManager(Texture2D bossImg, Texture2D bulletImg, Texture2D exImg, int maxBoss)
        {
            this.bossImg = bossImg;
            this.bulletImg = bulletImg;
            this.exImg = exImg;
            this.maxBoss = maxBoss;

            //Sets the boss's width and height to its image's width and height
            bossWidth = bossImg.Width;
            bossHeight = bossImg.Height;
        }

        //Pre: playerRec, playerBul, and playerGrd are not null
        //Post: returns if the boss has been defeated
        //Description: Handles all of the boss's behavior and returns a boolean based on if the boss has been beaten
        public bool Update(Rectangle playerRec, List<Bullet> playerBul, List<Grenade> playerGrd, GameTime gameTime)
        {            
            //Calls BossDie using the local variables
            BossDie(playerRec, playerBul, gameTime);

            //Calls GrenadeDamage using the local variables
            GrenadeDamage(playerGrd, gameTime);
            
            //Checks if boss has been defeated, if so reset all the boss's stats and return true
            if (dieCount >= maxBoss && boss.Count < 1)
            {
                ResetBoss();

                return true;
            }

            //If boss has not spawned in, call SpawnBoss 
            if (boss.Count < 1)
            {
                SpawnBoss(playerRec);
            }
            
            //Returns that the boss has not been defeated yet
            return false;
        }

        //Pre: playerLines is not null and contains four elements
        //Post: returns the damage the boss's bullets have dealted
        //Description: Checks if any of the boss's bullets collided with the player, and if so it returns the amount of damage the player has taken
        public int BossDamage(Line[] playerLines)
        {
            //Stores the bullet's total damage
            int healthRemove = 0;

            //Incriments through all the bosses
            for (int i = 0; i < boss.Count; i++)
            {
                //Incriments through all the boss's bullets
                for (int j = 0; j < boss[i].bullets.Count; j++)
                {
                    //Checks if current bullet is colliding with the player
                    if (boss[i].col.AngledBoxCol(boss[i].bullets[j].bulLines, playerLines))
                    {
                        //Adds the damage taken from the bullet and deletes current bullet and exits the loop
                        healthRemove += BUL_DAMG;
                        boss[i].bullets.RemoveAt(j);
                        break;
                    }
                }
            }

            //Returns the total bullet's damage
            return healthRemove;
        }

        //Pre: playerRec, gameTime, and playerBul are not null
        //Post: N/A
        //Description: Handles all of the boss's stats and checks if any of the bosses died
        public void BossDie(Rectangle playerRec, List<Bullet> playerBul, GameTime gameTime)
        {
            //Incrimetns through all the bosses
            for (int i = 0; i < boss.Count; i++)
            {
                //Calls the update sub-program of the current boss
                boss[i].Update(playerRec, playerBul, boss);

                //Checks if boss has died and the total amount that died is less than the maximum amount of bosses that can die
                if (boss[i].health <= 0 && dieCount < maxBoss)
                {
                    //Creates a new boss based on the current's boss location
                    Rectangle newBoss = new Rectangle((int)boss[i].angledRec.topLeft.X,
                                                      (int)boss[i].angledRec.topLeft.Y,
                                                      (int)(boss[i].bossRec.Width / 1.3f),
                                                      (int)(boss[i].bossRec.Height / 1.3f));

                    //Creates a new explosion animation
                    bossExAni.Add(new Animation(exImg, 9, 9, 81, 0, 0, Animation.ANIMATE_ONCE, 1, boss[i].angledRec.topLeft, 1f, true));

                    //Adds two new bosses to the boss list
                    boss.Add(new Boss(bossImg, newBoss, 1, (int)(boss[i].ogHealth / 1.5f), bulletImg));
                    boss.Add(new Boss(bossImg, newBoss, -1, (int)(boss[i].ogHealth / 1.5f), bulletImg));
                }

                //checks if boss died
                if (boss[i].health <= 0) 
                {
                    //Incrimetns dieCount
                    dieCount++;
                    
                    //Adds a new explosion animation and removes current boss and exits loop
                    bossExAni.Add(new Animation(exImg, 9, 9, 81, 0, 0, Animation.ANIMATE_ONCE, 1, boss[i].angledRec.topLeft, 1f, true));
                    boss.RemoveAt(i);
                    break;
                }
            }

            //Incriments through all boss explosion animations
            for (int i = 0; i < bossExAni.Count; i++)
            {
                //Calls the update sub-program of current animation
                bossExAni[i].Update(gameTime);

                //Checks if current animation is not animating, if so removes the animation and exits loop
                if (!bossExAni[i].isAnimating)
                {
                    bossExAni.RemoveAt(i);
                    break;
                }
            }
        }

        //Pre: playerGrd and gameTime are not null
        //Post: N/A
        //Description: Handles spawning and deleting the explosion animations
        public void GrenadeDamage(List<Grenade> playerGrd, GameTime gameTime)
        {
            //Incriments through all of the player's grenades
            for (int i = 0; i < playerGrd.Count; i++)
            {
                //Incriments through all the bosses
                for (int j = 0; j < boss.Count; j++)
                {
                    //Checks if grenade has exploded, if so adds a new explosion animation 
                    if (playerGrd[i].grdTimer >= playerGrd[i].maxTime)
                    {
                        bossExAni.Add(new Animation(exImg, 9, 9, 81, 0, 0, Animation.ANIMATE_ONCE, 1, playerGrd[i].grdRec.Location.ToVector2(), 1f, true));

                        //removes health from boss based on its distance away from the grenade
                        boss[j].health -= (int)(playerGrd[i].ObjectInRange(boss[j].bossRec.Center.ToVector2()) * MAX_GRD_DAMG);
                    }
                }

                //Checks if grenade has exploded, if so removes current player grenade and breaks out of loop 
                if (playerGrd[i].grdTimer >= playerGrd[i].maxTime)
                {
                    playerGrd.RemoveAt(i);
                    break;
                }
            }

            //Incriments through all the grenades explosion animations
            for (int i = 0; i < grdExAni.Count; i++)
            {
                //Calls current grenades update function
                grdExAni[i].Update(gameTime);

                //Checks if grenade is not animating, if so removes grenade and exits the loop
                if (!grdExAni[i].isAnimating)
                {
                    grdExAni.RemoveAt(i);
                    break;
                }
            }
        }

        //Pre: playerRec is not null
        //Post: N/A
        //Description: Handles spawning all the bosses
        public void SpawnBoss(Rectangle playerRec)
        {
            //Creates a new destination rectangle for the boss 300 pixels away from the player 
            bossRec = new Rectangle(playerRec.X, playerRec.Y - 300, bossWidth, bossHeight);

            //Adds a new explosion animation
            bossExAni.Add(new Animation(exImg, 9, 9, 81, 0, 0, Animation.ANIMATE_ONCE, 1, bossRec.Location.ToVector2(), 1f, true));
            
            //Adds two new bosses
            boss.Add(new Boss(bossImg, bossRec, 1, ogBossHealth, bulletImg));
            boss.Add(new Boss(bossImg, bossRec, -1, ogBossHealth, bulletImg));
        }

        //Pre: spriteBatch and playerRec are both not null
        //Post: N/A
        //Description: Draws all the boss's elements
        public void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            //Draws each boss
            foreach (Boss bs in boss)
            {
                bs.Draw(spriteBatch, playerRec);
            }

            //Draws each boss explosion animation
            foreach (Animation ex in bossExAni)
            {
                ex.Draw(spriteBatch, Color.White, SpriteEffects.None);
            }

            //Draws each grenade explosion animation
            foreach (Animation ex in grdExAni)
            {
                ex.Draw(spriteBatch, Color.White, SpriteEffects.None);
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets the boss's die count and removes all current bosses
        public void ResetBoss()
        {
            dieCount = 0;
            boss.Clear();
        }
    }
}
