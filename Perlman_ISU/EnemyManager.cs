//Author Name: Elad Perlman
//File Name: EnemyManager
//Project Name: Perlman_ISU
//Date Created: Jan 1th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic of managing all the enemy types
using Animation2D;
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
    class EnemyManager
    {
        //Stores all the enemies images
        private Texture2D ghostImg;
        private Texture2D zombiePlayerImg;
        private Texture2D bulletImg;

        //Stores all the explosion sprite sheets
        private Texture2D enExImg;
        private Texture2D grdExImg;

        //Stores all the explosion animations
        private List<Animation> enExAni = new List<Animation>();
        private List<Animation> grdExAni = new List<Animation>();

        //Stores random variable
        private Random rnd = new Random();

        //Stores spawn timer of enemies
        private float spawnTimer = 0f;

        //stores list of all the enemies
        private List<Enemy> enemys = new List<Enemy>();

        //Stores max time, and the amount each timer will increase by each update
        private const int TIME_LIM = 2;
        private const float TIME_INC = 0.05f;

        //Stores the touch damage, and bullet damage
        private const int TOUCH_DAMG = -10;
        private const int BUL_DAMG = -2;

        //Stores the maximum damage that a grenade can deal
        private const int MAX_GRD_DAMG = 150;

        //Stores total amount of enemies that have been spawned
        private int enCount = 0;

        //Stores max amount of enemies that can spawn, and their health
        public int maxEnCount;
        private int enHealth;

        //Stores the distance amount that the enemy will spawn away from the player
        private const double SPAWN_MULT = 1000.0;

        //Stores amount of enemies that died
        private int enDied = 0;

        //Stores gun shot sound effect
        private SoundEffect gunShotSnd;

        //Pre: all Texture2D's variable are not null, and maxEnCount and enHealth are both positive integers
        //Post: None
        //Description: Sets each global variable to its local one
        public EnemyManager(Texture2D ghostImg, Texture2D zombiePlayerImg, Texture2D bulletImg, Texture2D enExImg, Texture2D grdExImg, int maxEnCount, int enHealth, SoundEffect gunShotSnd)
        {
            this.ghostImg = ghostImg;
            this.zombiePlayerImg = zombiePlayerImg;
            this.bulletImg = bulletImg;
            this.enExImg = enExImg;
            this.grdExImg = grdExImg;
            this.maxEnCount = maxEnCount;
            this.enHealth = enHealth;
            this.gunShotSnd = gunShotSnd;
        }

        //Pre: playerRec isn't null
        //Post: N/A
        //Description: Handles spawning all types of enemies
        public void SpawnEnemy(Rectangle playerRec)
        {
            //Sets a random number between 1 to 100
            int num = rnd.Next(1, 101);

            //Sets a random angle between 0 and 360
            int angle = rnd.Next(0, 361);

            //Stores the location of new enemy, and calcualtes its random location
            Vector2 rndLoc;
            rndLoc.X = (float)(playerRec.Center.X + Math.Cos(MathHelper.ToRadians(angle)) * SPAWN_MULT);
            rndLoc.Y = (float)(playerRec.Center.Y + Math.Sin(MathHelper.ToRadians(angle)) * SPAWN_MULT);

            //Increases spawnTimer
            spawnTimer += TIME_INC;

            //Checks if spawnTimer is over its max value, and less enemies spawned than the max amount
            if (spawnTimer >= TIME_LIM && enCount < maxEnCount)
            {
                //Checks which range the number falls in, then generates a new obstacle of the type that corresponds with that range
                if (num > 0 && num <= 80)
                {
                    enemys.Add(new Enemy(ghostImg, rndLoc, enHealth));
                }
                
                //Executes if above if statement failed, if so add a new zombie enemy
                else
                {
                    enemys.Add(new ZombieEn(zombiePlayerImg, bulletImg, rndLoc, enHealth, gunShotSnd));
                }

                //Incriment enCount
                enCount++;

                //Reset spawnTimer
                spawnTimer = 0f;
            }
        }

        //Pre: N/A
        //Post: returns if round is over
        //Description: checks if round is over and returns the result
        public bool IsRoundOver()
        {
            //Checks if round has ended and returns the result
            if (enCount == maxEnCount && enemys.Count == 0 && enExAni.Count == 0)
            {
                return true;
            }

            return false;
        }

        //Pre: All variables are not null
        //Post: Returns the amount of health that will be removed from the player
        //Description: Handles all of the enemies and their logic, as well as removing them when nessecary
        public int Update(Line[] playerLines, List<Bullet> playerBul, Rectangle playerRec, List<Grenade> playerGrd, GameTime gameTime)
        {
            //Stores the amount of health the player will lose
            int healthRemove = 0;

            //Calls SpawnEnemy and GrenadeDamage
            SpawnEnemy(playerRec);
            GrenadeDamage(playerGrd, gameTime);

            //Incriments through each enemy
            for (int i = 0; i < enemys.Count; i++)
            {
                //Checks if enemy died, if so add explosion animation and remove enemy, as well as exit loop
                if (enemys[i].health <= 0)
                {
                   enExAni.Add(new Animation(enExImg, 5, 5, 13, 0, 0, Animation.ANIMATE_ONCE, 3, enemys[i].angledRec.topLeft, 2f, true));
                   enemys.RemoveAt(i);
                   enDied++;
                   break;
                }
                
                //Checks if enemy touched player
                if (enemys[i].Update(playerLines, playerRec, playerBul, enemys))
                {
                    //Adds touch damage to the total damage
                    healthRemove += TOUCH_DAMG;
                    
                    //Adds explosion animation and removes enemy, as well as exits loop
                    enExAni.Add(new Animation(enExImg, 5, 5, 13, 0, 0, Animation.ANIMATE_ONCE, 3, enemys[i].angledRec.topLeft, 2f, true));
                    enemys.RemoveAt(i);
                    enDied++;
                    break;
                }

                //Incriments through each of enemy's bullets
                for (int j = 0; j < enemys[i].bullets.Count; j++)
                {
                    //Checks if bullet collided with player
                    if (enemys[i].enCol.AngledBoxCol(enemys[i].bullets[j].bulLines, playerLines))
                    {
                        //Add bullet damage to total damage, remove bullet, and exit loop
                        healthRemove += BUL_DAMG;
                        enemys[i].bullets.RemoveAt(j);
                        break;
                    }
                }
            }

            //Incriments through all enemies explosion animations
            for (int i = 0; i < enExAni.Count; i++)
            {
                //Calls Update function
                enExAni[i].Update(gameTime);

                //Checks if current explosion animation is not animating, if so remove animation and exit loop
                if (!enExAni[i].isAnimating)
                {
                    enExAni.RemoveAt(i);
                    break;
                }            
            }
            
            //returns total damage
            return healthRemove;
        }

        //Pre: playerGrd and gameTime are not null
        //Post: N/A
        //Pre: Handles all of the grenades functions 
        public void GrenadeDamage(List<Grenade> playerGrd, GameTime gameTime)
        {
            //Incriments through each player grenade
            for (int i = 0; i < playerGrd.Count; i++)
            {
                //Incriments through each enemy
                for (int j = 0; j < enemys.Count; j++)
                {
                    //Checks if grenade exploded, if so remove health from enemy based on its distance away from grenade
                    if (playerGrd[i].grdTimer >= playerGrd[i].maxTime)
                    {
                        enemys[j].health -= (int)(playerGrd[i].ObjectInRange(enemys[j].enRec.Center.ToVector2()) * MAX_GRD_DAMG);
                    }
                }

                //Checks if grenade exploded, if so add explosion animation, remove grenade and exit loop
                if (playerGrd[i].grdTimer >= playerGrd[i].maxTime)
                {
                    grdExAni.Add(new Animation(grdExImg, 9, 9, 81, 0, 0, Animation.ANIMATE_ONCE, 1, playerGrd[i].grdRec.Location.ToVector2(), 1f, true));
                    playerGrd.RemoveAt(i);
                    break;
                }
            }

            //Incriments through each grenade explosion animation
            for (int i = 0; i < grdExAni.Count; i++)
            {
                //Calls grenade's update function
                grdExAni[i].Update(gameTime);
                
                //Checks if grenade explosion is not animating, if so remove animation and exit loop
                if (!grdExAni[i].isAnimating)
                {
                    grdExAni.RemoveAt(i);
                    break;
                }
            }
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Desciption: Draws all the enemies and their explosion animations
        public void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            foreach (Enemy enemy in enemys)
            {
                enemy.Draw(spriteBatch, playerRec);
            }
            
            foreach (Animation ex in enExAni)
            {
                ex.Draw(spriteBatch, Color.White, SpriteEffects.None);
            }
            
            foreach ( Animation ex in grdExAni)
            {
                ex.Draw(spriteBatch, Color.White, SpriteEffects.None);
            }
        }

        //Pre: N/A
        //Post: Returns the amount of enemies left
        //Description: Gets the amount of enemies left
        public int GetEnLeft()
        {
            return maxEnCount - enDied;
        }
    }
}
