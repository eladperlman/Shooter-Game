//Author Name: Elad Perlman
//File Name: Round
//Project Name: Perlman_ISU
//Date Created: Jan 13th 2020
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics and graphics of an enemy round and a boss round
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Round
    {
        //Stores an instance of enemy manager
        private EnemyManager enManager;

        //Stores an instance of boss manager
        private BossManager bsManager;
        
        //Pre: enManager and bsManager are not null
        //Post: N/A
        //Description: Sets enManager and bsManager to their local ones
        public Round(EnemyManager enManager, BossManager bsManager)
        {
            this.enManager = enManager;
            this.bsManager = bsManager;
        }

        //Pre: gameTime and player are not null
        //Post: returns if enemy round is over or not
        //Description: Handles all the logic of an enemy round
        public bool EnemyRound(GameTime gameTime, Player player)
        {
            //Calls player update using enManager's update as a parameter
            player.Update(enManager.Update(player.recLines, player.GetBullets(), player.GetRec(), player.grenades, gameTime));

            //Returns if round is over
            return enManager.IsRoundOver();
        }

        //Pre: gameTime and player are not null
        //Post: returns if boss round is over or not
        //Description: Handles all the logic of a boss round
        public bool BossRound(GameTime gameTime, Player player)
        {
            //Calls player update using bsManager's update as a parameter
            player.Update(bsManager.BossDamage(player.recLines));

            //Returns if round is over
            return bsManager.Update(player.GetRec(), player.GetBullets(), player.grenades, gameTime);
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Description: Draws all components of a boss round
        public void DrawBoss(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            bsManager.Draw(spriteBatch, playerRec);
        }

        //Pre: spriteBatch and playerRec are not null
        //Post: N/A
        //Description: Draws all components of an enemy round
        public void DrawEnemies(SpriteBatch spriteBatch, Rectangle playerRec)
        {
            enManager.Draw(spriteBatch, playerRec);
            spriteBatch.DrawString(Game1.font, "Enemies left: " + enManager.GetEnLeft(), new Vector2(playerRec.X - playerRec.Width * 9, playerRec.Y - playerRec.Height * 2), Color.White);
        }
    }
}
