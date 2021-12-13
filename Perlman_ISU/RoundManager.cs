//Author Name: Elad Perlman
//File Name: RoundManager
//Project Name: Perlman_ISU
//Date Created: Jan 12th 2020
//Date Modified: Jan 17th 2020
//Description: This class is in charge of handling all the types of rounds and their functionallity
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
    class RoundManager
    {
        //Stores all the rounds
        private List<Round> round = new List<Round>();

        //Stores the current round number
        public int roundCount = 1;

        //Stores the maximum amount of enemies that can spawn
        private int maxEnCount = 15;

        //Stores the maximum amount of bosses that can spawn
        private int maxBsCount = 5;

        //Stores the enemies intial health
        private int enHealth = 10;

        //Stores the round's start timer
        private float roundStartCount = 0f;

        //Stores the amount of time that passes every update
        private float timeInc = 1f / Game1.FPS;

        //Stores the player
        private Player player;

        //Stores the round number that will be a boss round 
        private const int BOSS_ROUND = 5;
       
        //Stores the amount of enemies and their health that will increase at the end of each round
        private const int EN_COUNT_INC = 4;
        private const int EN_HEALTH_INC = 3;
        
        //Stores the minimum amount of enemies that will spawn
        private const int MIN_EN_COUNT = 15;

        //Stores the amount of bosses and their health that will increase at the end of each boss round
        private const int MAX_BS_COUNT = 12;
        private const int MAX_EN_HEALTH = 60;

        //Stores all the enemies images
        private Texture2D ghostImg;
        private Texture2D zombiePlayerImg;
        
        //Stores the bullet's image 
        private Texture2D bulletImg;
        
        //Stores the explosion spritesheets for the enemy and grenade
        private Texture2D enExImg;
        private Texture2D grdExImg;
        
        //Stores the boss's image
        private Texture2D bossImg;

        //Stores the fight and gunshot sound effects 
        private SoundEffect fightSnd;
        private SoundEffect gunShotSnd;

        //Pre: all variables are not null 
        //Post: N/A
        //Description: Sets all global variables to their local ones
        public RoundManager(Player player, Texture2D ghostImg, Texture2D zombiePlayerImg,
                     Texture2D bulletImg, Texture2D enExImg, Texture2D grdExImg,
                     Texture2D bossImg, EnemyManager enManager, BossManager bsManager, SoundEffect fightSnd, SoundEffect gunShotSnd)
        {
            this.player = player;
            this.ghostImg = ghostImg;
            this.zombiePlayerImg = zombiePlayerImg;
            this.bulletImg = bulletImg;
            this.enExImg = enExImg;
            this.grdExImg = grdExImg;
            this.bossImg = bossImg;
            this.fightSnd = fightSnd;
            this.gunShotSnd = gunShotSnd;

            //Adds a new Round
            round.Add(new Round(enManager, bsManager));
        }

        //Pre: N/A
        //Post: Returns if it is time for the round to start
        public bool RoundStart()
        {
            //Checks if round did not start yet
            if (round.Count < 1)
            {
                //Creates a new enemy and boss manager
                EnemyManager enManager = new EnemyManager(ghostImg, zombiePlayerImg, bulletImg, enExImg, grdExImg, maxEnCount, enHealth, gunShotSnd);
                BossManager bsManager = new BossManager(bossImg, bulletImg, grdExImg, maxBsCount);

                //Adds a new Round and creates an instance of fight sound effect
                round.Add(new Round(enManager, bsManager));
                fightSnd.CreateInstance().Play();
            }
            
            //Increases the start round timer
            roundStartCount += timeInc;

            //Checks if its time to start round and returns true
            if (roundStartCount >= 1)
            {             
                return true;
            }

            //Returns that round did not start yet
            return false;
        }

        //Pre: gameTime is not null
        //Post: N/A
        //Description: Handles all the different round types and their stats and functionallity
        public void Round(GameTime gameTime)
        {
            //Checks if current round is a boss round
            if (roundCount % BOSS_ROUND == 0)
            {                          
                //Checks if round started
                if (RoundStart())
                {
                    //Checks if round ended
                    if (round[0].BossRound(gameTime, player))
                    {
                        //Incriments roundCount and resets round start timer
                        roundCount++;
                        roundStartCount = 0;

                        //Resets max enemy count to its orignal value
                        maxEnCount = MIN_EN_COUNT;

                        //Checks if there are less bosses that can spawn than the maximum amount, if so incriments maxBsCount
                        if (maxBsCount <= MAX_BS_COUNT)
                        {
                            maxBsCount++;
                        }

                        //Checks if current enemy health is below the maximum amount, if so add health to each enemy
                        if (enHealth <= MAX_EN_HEALTH)
                        {
                            enHealth += EN_HEALTH_INC;
                        }

                        //Removes round
                        round.RemoveAt(0);
                    }
                }                
            }

            //execute if above if statement has failed
            else
            {                
                //Checks if round has started
                if (RoundStart())
                {
                    //Check if the round ended
                    if (round[0].EnemyRound(gameTime, player))
                    {
                        //Incriments roundCount and resets round start timer
                        roundCount++;
                        roundStartCount = 0;

                        //Adds to the amount of enemies that will spawn next round
                        maxEnCount += EN_COUNT_INC;

                        //Checks if current enemy health is below the maximum amount, if so add health to each enemy
                        if (enHealth <= MAX_EN_HEALTH)
                        {
                            enHealth += EN_HEALTH_INC;
                        }

                        //Removes round
                        round.RemoveAt(0);
                    }
                }
            }
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Description: Draws all the rounds elements
        public void Draw(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            //Checks if round has started, if so call the round's draw function
            if (round.Count > 0)
            {
                round[0].DrawEnemies(spriteBatch, playerRec);
                round[0].DrawBoss(spriteBatch, playerRec);

                spriteBatch.DrawString(Game1.font, "Round: " + roundCount, new Vector2(playerRec.X - playerRec.Width * 9, playerRec.Y - playerRec.Height), Color.White);
            }         
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets all the modified variables to their orginial values
        public void Reset()
        {
            round.Clear();
            roundCount = 1;
            maxBsCount = 5;
            maxEnCount = 15;
            roundStartCount = 0f;
            enHealth = 10;
        }
    }
}
