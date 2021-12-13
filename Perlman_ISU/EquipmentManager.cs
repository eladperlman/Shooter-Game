//Author Name: Elad Perlman
//File Name: EquipmentManager
//Project Name: Perlman_ISU
//Date Created: Jan 6th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic of managing all the equipment types
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class EquipmentManager
    {
        //Stores a random variable
        private Random rnd = new Random();
        
        //Stores the spawn timer of the equipment
        private float spawnTimer = 0f;

        //Stores the amount each timer will increase by every update
        private const float TIME_INC = 0.015f;

        //Stores timer's max time
        private const int MAX_TIME = 4;

        //Stores the amount of health gained from a medkit
        private const int HEALTH_GAIN = 25;

        //Stores the amount of ammo gained from a ammo pack
        private const int AMMO_GAIN = 30;

        //Stores all the equipments
        private List<Equipment> equipment = new List<Equipment>();

        //Stores all the different equipment images
        private Texture2D ammoImg;
        private Texture2D medkitImg;
        private Texture2D grdImg;

        //Pre: all Texture2D's are not null
        //Post: N/A
        //Description: Sets all global variables to their local ones
        public EquipmentManager(Texture2D ammoImg, Texture2D medkitImg, Texture2D grdImg)
        {
            this.ammoImg = ammoImg;
            this.medkitImg = medkitImg;
            this.grdImg = grdImg;
        }
        
        //Pre: N/A
        //Post: N/A
        //Description: Spawns all the different kind of equipment
        public void EqSpawner()
        {
            //Increases the spawnTimer
            spawnTimer += TIME_INC;

            //Checks if spawnTimer is above its max time
            if (spawnTimer >= MAX_TIME)
            {
                //Stores a random number from 0 to 100
                int rndNum = rnd.Next(0, 101);

                //if number is between 0 and 50, add a new equipment type ammo
                if (rndNum >= 0 && rndNum <= 50)
                {
                    equipment.Add(new Ammo(ammoImg));                   
                }

                //if number is between 50 and 80, add a new equipment type medkit
                if (rndNum > 50 && rndNum <= 80)
                {
                    equipment.Add(new Medkit(medkitImg));          
                }

                //if number is between 80 and 100, add a new equipment type grenade
                if (rndNum > 80 && rndNum <= 100)
                {
                    equipment.Add(new GrenadeEq(grdImg));
                }

                //Reset spawn timer
                spawnTimer = 0f;
            }

            //Inriments through every equipment
            for (int i = 0; i <equipment.Count; i++)
            {
                //Checks if their transparency is not 1, if so call TransSpawn
                if (equipment[i].transVal != 1)
                {
                    equipment[i].TransSpawn();
                }
            }
        }

        //Pre: playerLines is not null, and has 4 elements
        //Post: Returns the ammount of ammo picked up
        //Description: Checks if player picked up an ammo pack and increases its ammo count if so
        public int DeleteAmmo(Line[] playerLines)
        {
            //Stores the amount of ammo player will gain
            int ammoGain = 0;

            //Incriments through each equipment
            for (int i = 0; i < equipment.Count; i++)
            {                             
                //Checks if equipment collided with player and is type ammo
                if (equipment[i].ColCheck(playerLines) && equipment[i].GetType() == typeof(Ammo))
                {                    
                    //Increase ammoGain by 30, removes equipment and exits loop
                    ammoGain += AMMO_GAIN;                  
                    equipment.RemoveAt(i);
                    break;
                }
            }

            //Returns the amount of ammo player will gain
            return ammoGain;
        }

        //Pre: playerLines is not null, and has 4 elements
        //Post: Returns the ammount of health gained
        //Description: Checks if player picked up an medkit and increases its health if so
        public int DeleteHealth(Line[] playerLines)
        {
            //Stores amount of health player will gain
            int healthGain = 0;

            //Incriments through each equipment
            for (int i = 0; i < equipment.Count; i++)
            {
                //Checks if equipment collided with player and is type medkit
                if (equipment[i].ColCheck(playerLines) && equipment[i].GetType() == typeof(Medkit))
                {
                    //Increase health by 25, removes equipment and exits loop
                    healthGain += HEALTH_GAIN;
                    equipment.RemoveAt(i);
                    break;
                }
            }

            //Returns amount of health player will gain
            return healthGain;
        }

        //Pre: playerLines is not null, and has 4 elements
        //Post: Returns the ammount of grenades gained
        //Description: Checks if player picked up a grenade and increases its grenade count if so
        public int DeleteGrenade(Line[] playerLines)
        {
            //Stores the amount of grenades player will gain
            int grenadeGain = 0;

            //Incriments through each equipment
            for (int i = 0; i < equipment.Count; i++)
            {
                //Checks if equipment collided with player and is type grenadeEq
                if (equipment[i].ColCheck(playerLines) && equipment[i].GetType() == typeof(GrenadeEq))
                {
                    //Incriment grenadeCount, removes equipment and exits loop
                    grenadeGain++;
                    equipment.RemoveAt(i);
                    break;
                }
            }

            //Returns the amount of grenades player will gain
            return grenadeGain;
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the equipment
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Equipment eq in equipment)
            {
                eq.Draw(spriteBatch);
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets spawnTimer, and deletes all the equipment
        public void Reset()
        {
            spawnTimer = 0f;
            equipment.Clear();
        }
    }
}
