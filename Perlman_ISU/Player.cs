//Author Name: Elad Perlman
//File Name: Player
//Project Name: Perlman_ISU
//Date Created: Dec 10th 2019
//Date Modified: Jan 17th 2020
//Description: This class takes care of all the logic and physics of a player such as moving it, shooting, and its collision, as well as drawing it 
using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Player
    {
        //Stores the player's image and destination rectangle
        public Texture2D playerImg;
        private Rectangle playerRec;

        //stores shield ability bar images and destination rectangles
        private Texture2D shieldAmountImg;
        private Texture2D shieldBarImg;
        private Rectangle shieldBarRec;
        private Rectangle shieldAmountRec;

        //Stores player's shield bubble image and destination rectangle
        private Texture2D shieldImg;
        private Rectangle shieldRec;

        //Stores player's bullet image
        private Texture2D bulletImg;

        //Stores gun shot sound effect
        private SoundEffect gunShotSnd;

        //Stores player's speed, and maximum speed and max sprint speed
        private Vector2 speed = new Vector2(0, 0);
        private const int MAX_SPEED = 15;
        private const int MAX_SPRINT = 25;

        //Stores the ar and burst weapon states
        private const int AR = 1;
        private const int BURST = 2;

        //Stores current gun state
        private int gunState = 1;

        //Stores the amount of bullets shot per burst
        private int burstCount = 0;
        
        //Stores the timer to shoot a burst, and its max time
        private float burstTimer = 0f;
        private const float BURST_MAX_TIME = 0.3f;

        //Stores weather gun is shooting a burst or not
        private bool burstActive = false;

        //Stores the times an ar and burst can shoot 
        private const int MAX_SHOOT_TIME_AR = 1; 
        private const int MAX_SHOOT_TIME_BURST = 2; 

        //Stores if player is spritning or not
        private bool sprint = false;
        
        //Stores the acceleration and friction values
        private const float ACC = 1.5f;
        private const float DECC = -1f;

        //Stores player's rotation angle and the speed an angle will increase at
        private float angle = 0;
        private float angleVel = 4;

        //Stores player's shoot timer
        private float shootTimer = 0f;

        //Stores player's bullet count
        public int bulCount = 30;

        //Stores player's health
        public int health = 100;

        //Stores keyboardstate variable
        private KeyboardState kb;

        //Stores the time incriment for the timers every update
        private const float TIME_INC = 0.1f;

        //Stores player's bullets
        private List<Bullet> bullets = new List<Bullet>();

        //Stores player's rotated rectangle vertacies
        public AngledRec angledRec;

        //Stores player's grenade image
        private Texture2D grdImg;

        //Stores all player's grenades
        public List<Grenade> grenades = new List<Grenade>();

        //Stores all player's destination rectangle lines
        public Line[] recLines = new Line[4];

        //Stores if grenade was thrown
        private bool grdPressed = false;

        //Stores player's grenade count
        public int grdCount = 5;

        //Stores shield spawn timer, and active timer
        private float shieldSpawnTimer = 0f;
        private float shieldActiveTimer = 0f;
        
        //Stores shield bubbles transeprancy
        private float shieldTrans = 0;

        //Stores shield bubbles transeprancy multiplyer
        private int shieldTransMult = -1;

        //Stores time it takes for shield to become avaliable, as well amount of time it will be active for
        private const int SHIELD_SPAWN_TIME = 15;
        private const int MAX_SHIELD_TIME = 6;
        private const int SHIELD_DIE_TIME = 3;

        //Stores the amount the shield timer will increase by each update
        private const float SHIELD_TIME_INC = 1f / Game1.FPS;

        //Stores the shield metre's width multipler
        private const float SHIELD_WIDTH_MULT = 16.5f;

        //Stores the x position shift for the shield bar
        private const int SHIELD_POS_SHIFT = 12;

        //Stores weather shield is active or not
        private bool shieldActive = false;

        //Stores player's minimum speed
        private const float MIN_SPEED = 0.4f;

        //Pre: All Texture2D's are not null
        //Post: N/A
        //Description: Sets all global variables to their corresponding local variables
        public Player(Texture2D playerImg, Texture2D bulletImg, Texture2D grdImg, Texture2D shieldImg, Texture2D shieldAmountImg, Texture2D shieldBarImg, SoundEffect gunShotSnd)
        {
            this.playerImg = playerImg;
            this.bulletImg = bulletImg;
            this.shieldImg = shieldImg;
            this.grdImg = grdImg;
            this.shieldAmountImg = shieldAmountImg;
            this.shieldBarImg = shieldBarImg;
            this.gunShotSnd = gunShotSnd;

            //Sets up the destination rectangle's for the shield bubble and the shield bar
            shieldRec = new Rectangle(0, 0, shieldImg.Width / 8, shieldImg.Height / 8);
            shieldBarRec = new Rectangle(0, 0, shieldBarImg.Width, shieldBarImg.Height);
            shieldAmountRec = new Rectangle(0, 0, shieldAmountImg.Width, shieldAmountImg.Height);

            //Sets up player's destination rectangle
            playerRec = new Rectangle(Game1.windowWidth / 2, Game1.windowHeight / 2, playerImg.Width / 2, playerImg.Height / 2);
        }

        //Pre: healthRemove is a negative integer or has a value of 0
        //Post: N/A
        //Description: Handles all the player's functionallity and physics
        public void Update(int healthRemove)
        {           
            //Sets up player's rotated rectangle
            angledRec = new AngledRec();
            angledRec.NewRec(playerRec, angle);
            
            //Sets up the 4 lines of the player's rotated rectangle
            recLines[0] = new Line(angledRec.topLeft, angledRec.topRight);
            recLines[1] = new Line(angledRec.topRight, angledRec.bottomRight);
            recLines[2] = new Line(angledRec.bottomRight, angledRec.bottomLeft);
            recLines[3] = new Line(angledRec.bottomLeft, angledRec.topLeft);

            //Calls PlayerHealth and Grenade functions
            PlayerHealth(healthRemove);
            Grenade();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles the physics of the player's movement
        public void MovePlayer()
        {
            //Sets up the keyboard state
            kb = Keyboard.GetState();

            //Checks if player clicked the D key and his x speed is below max value, if so increase x speed
            if (kb.IsKeyDown(Keys.D) && (speed.X <= MAX_SPEED || sprint))
            {
                speed.X += ACC;

            }

            //Checks if player clicked the A key and his x speed is below max value in negative direction, if so decrease x speed 
            if (kb.IsKeyDown(Keys.A) && (speed.X >= -MAX_SPEED || sprint))
            {
                speed.X -= ACC;

            }

            //Checks if player clicked the S key and his y speed is below max value in negative direction, if so decrease y speed 
            if (kb.IsKeyDown(Keys.W) && (speed.Y >= -MAX_SPEED || sprint))
            {
                speed.Y -= ACC;

            }

            //Checks if player clicked the W key and his y speed is below max value, if so increase y speed
            if (kb.IsKeyDown(Keys.S) && (speed.Y <= MAX_SPEED || sprint))
            {
                speed.Y += ACC;

            }
            
            //Checks if left key was clicked, if so decrease angle
            if (kb.IsKeyDown(Keys.Left))
            {
                angle -= angleVel;
            }

            //Checks if right key was clicked, if so increase angle
            if (kb.IsKeyDown(Keys.Right))
            {
                angle += angleVel;
            }

            //Check if left shift is held down, if so set sprint to true
            if (kb.IsKeyDown(Keys.LeftShift))
            {
                sprint = true;
            }

            //Check if left shift is not held down, or player's x or y speeds are above max sprint value, if so set sprint to false
            if (!kb.IsKeyDown(Keys.LeftShift) || Math.Abs(speed.X) >= MAX_SPRINT || Math.Abs(speed.Y) >= MAX_SPRINT)
            {
                sprint = false;
            }

            //Checks if player's x speed is above the minimum value
            if (Math.Abs(speed.X) > MIN_SPEED)
            {
                //Checks if x speed is in the left direction, if so add friction in the oppisite diretion 
                if (speed.X < 0)
                {
                    speed.X += -DECC;
                }
                //Executes if above if statement failed, if so add friction in the oppisite diretion the player is travelling in 
                else
                {
                    speed.X += DECC;
                }
            }
            
            //Execute if above if statement failed, if so set x speed to zero
            else
            {
                speed.X = 0;
            }

            //Checks if player's y speed is above the minimum value
            if (Math.Abs(speed.Y) > MIN_SPEED)
            {
                //Checks if y speed is in the left direction, if so add friction in the oppisite diretion 
                if (speed.Y < 0)
                {
                    speed.Y += -DECC;
                }

                //Executes if above if statement failed, if so add friction in the oppisite diretion the player is travelling in
                else
                {
                    speed.Y += DECC;
                }
            }

            //Execute if above if statement failed, if so set y speed to zero
            else
            {
                speed.Y = 0;
            }
        
            //Moves the player based on its speed
            playerRec.X += (int)(speed.X);
            playerRec.Y += (int)(speed.Y);
        }
         
        //Pre: N/A
        //Post: N/A
        //Description: Handles the all the physics and behaviour of the player's bullets
        public void Shoot()
        {          
            //Sets bullet's postion to player's location
            Rectangle bulletRec = new Rectangle(playerRec.X, playerRec.Y, bulletImg.Width, bulletImg.Height);

            //Incriments player shoot timer
            shootTimer += TIME_INC;

            //Checks if the shoot button is down
            if (kb.IsKeyDown(Keys.Space) && bulCount > 0)
            {
                //Checks if shoot timer is above 1 second
                if (shootTimer > MAX_SHOOT_TIME_AR && gunState == AR)
                {
                    //Adds a new bullet and resets shooting timer, and plays gun sound effect
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle));
                    gunShotSnd.CreateInstance().Play();
                    shootTimer = 0f;
                    
                    //removes a bullet from the total amount
                    bulCount--;
                }

                //Checks if gun can fire a burst
                if (shootTimer > MAX_SHOOT_TIME_BURST && gunState == BURST && !burstActive && bulCount > 2)
                {
                    //sets burstActive to true, and resets shootTimer
                    burstActive = true;
                    shootTimer = 0;
                }

            }
            
            //Checks if burstActive is true
            if (burstActive)
            {
                //Increases the burst timer
                burstTimer += TIME_INC;

                //Checks if gun fired a full burst, if so reset burstCount and burstTimer and set burstActive to false
                if (burstCount == 3)
                {
                    burstTimer = 0;
                    burstActive = false;
                    burstCount = 0;
                }

                //Checks if burstTimer is larger than its max time
                if (burstTimer >= BURST_MAX_TIME)
                {
                    //Adds a bullet to bullet list, and removes bullet from total bullet cound, and plays gun sound effect
                    bullets.Add(new Bullet(bulletRec, bulletImg, angle));
                    gunShotSnd.CreateInstance().Play();
                    bulCount--;

                    //Resets burstTimer and incriments burstCount
                    burstTimer = 0;
                    burstCount++;
                }
            }

            //Checks if key 1 was pressed, if so sets gunState to assult rifle
            if (kb.IsKeyDown(Keys.D1))
            {
                gunState = AR;
            }

            //Checks if key 2 was pressed, if so sets gunState to burst
            if (kb.IsKeyDown(Keys.D2))
            {
                gunState = BURST;
            }

            //Incriments through player's bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                //Moves current bullet
                bullets[i].MoveBullet();

                //Checks if bullets life spawn is over, if so bullet is deleted
                if (bullets[i].MoveBullet())
                {
                    bullets.Remove(bullets[i]);
                    break;
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles the all the physics and behaviour of the player's grenades
        public void Grenade()
        {          
            //Checks if e has not been pressed, if so set grdPressed to false
            if (!kb.IsKeyDown(Keys.E))
            {
                grdPressed = false;
            }

            //Checks if player has pressed e and grdPressed is false, as well as if there are more than 0 grenades 
            if (kb.IsKeyDown(Keys.E) && !grdPressed && grdCount > 0)
            {
                //Creates a new rectnagle for the grenade based on player's location, and adds a new grenade
                Rectangle grdRec = new Rectangle(playerRec.X, playerRec.Y, grdImg.Width / 4, grdImg.Height / 4);
                grenades.Add(new Grenade(grdImg, grdRec, angle));

                //Decrements grdCount
                grdCount--;

                //Sets grdPressed to true
                grdPressed = true;
            }

            //Incriments through each grenade
            for (int i = 0; i < grenades.Count; i++)
            {
                //Checks if e was pressed or if grenade exploded, if so call grenades update function
                if (kb.IsKeyUp(Keys.E) || !grenades[i].Update())
                {
                    grenades[i].Update();
                }
            }            
        }

        //Pre: 
        //Post: N/A
        //Description: Checks if user clicked the shield button, then activates the shield if pressed, 
        //as well as keeping a timer for when the user can activate the shield and a timer for how long the shield is active for
        public void ActivateShield()
        {
            //Sets the shield's rectangle location equal to the player's           
            shieldRec.X = playerRec.X - playerRec.Width;
            shieldRec.Y = (int)(playerRec.Y - playerRec.Height / 1.5f);

            //Sets the shieldbar's rectangle location 
            shieldBarRec.X = playerRec.X - playerRec.Width * 9; 
            shieldBarRec.Y = playerRec.Y - playerRec.Height * 3;

            //Sets the shieldbar amount's rectangle location 
            shieldAmountRec.X = shieldBarRec.X + SHIELD_POS_SHIFT;
            shieldAmountRec.Y = shieldBarRec.Y;

            //Checks if shieldTimer is below its max value, if so set the width of the shield amount to the timer times a constant
            if (shieldSpawnTimer <= SHIELD_SPAWN_TIME)
            {
                shieldAmountRec.Width = (int)(shieldSpawnTimer * SHIELD_WIDTH_MULT);
            }          

            //Adds one to the shield timer every second
            shieldSpawnTimer += SHIELD_TIME_INC;

            //Checks if shield button was pressed, and if shield is above its maximum point
            if (kb.IsKeyDown(Keys.F) && shieldSpawnTimer >= SHIELD_SPAWN_TIME)
            {
                //Sets transperancy of shield equal to 1 and activates shield
                shieldTrans = 1f;
                shieldActive = true;
            }

            //Checks if shield is active
            if (shieldActive)
            {
                //Adds 1 to the shield active timer every second
                shieldActiveTimer += SHIELD_TIME_INC;

                //Checks if shield was active for more than its dying time
                if (shieldActiveTimer >= SHIELD_DIE_TIME)
                {
                    //Incriments shield transparency 
                    shieldTrans += SHIELD_TIME_INC * shieldTransMult;

                    //Checks if shield transparency is less than or equal to 0 or greater than or equal to 1
                    if (shieldTrans <= 0 || shieldTrans >= 1)
                    {
                        //Multiplies shield transperancy multiplier by 1
                        shieldTransMult *= -1;
                    }
                }

                //Checks if shield has been active for more than its max time
                if (shieldActiveTimer >= MAX_SHIELD_TIME)
                {
                    //Deactivates shield
                    shieldActive = false;

                    //Resets the all timers and shield transparency to 0
                    shieldActiveTimer = 0f;
                    shieldSpawnTimer = 0f;
                    shieldTrans = 0f;
                }
            }
        }
        
        //Pre: healthRemove is a negative integer or 0
        //Post: N/A
        //Description: Descreases player's health based on the value of healthRemove
        public void PlayerHealth(int healthRemove)
        {
            //Checks if shield isn't active, if so decrease player'e health based on the value of healthRemove
            if (!shieldActive)
            {
                health += healthRemove;
            }
        }

        //Pre: spriteBatch isnt null
        //Post: N/A
        //Description: Draws all the player's features 
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draws the player
            spriteBatch.Draw(playerImg, playerRec, null, Color.White, MathHelper.ToRadians(angle), new Vector2(playerImg.Width / 2, playerImg.Height / 2), SpriteEffects.None, 0);
            
            //Draws all of the player's bullets
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            //Draws all the player's grenades
            foreach (Grenade grd in grenades)
            {
                grd.Draw(spriteBatch);
            }

            //Draws the shield bar and its amount of shield
            spriteBatch.Draw(shieldAmountImg, shieldAmountRec, Color.White);
            spriteBatch.Draw(shieldBarImg, shieldBarRec, Color.White);

            //Draws the player's health grenade and ammo count
            spriteBatch.DrawString(Game1.font, "Health: " + health, new Vector2(playerRec.X - playerRec.Width * 9, playerRec.Y - playerRec.Height * 2.5f), Color.White);
            spriteBatch.DrawString(Game1.font, "Grenades: " + grdCount, new Vector2(playerRec.X + playerRec.Width * 5, playerRec.Y - playerRec.Height * 2.5f), Color.White);
            spriteBatch.DrawString(Game1.font, "Ammo: " + bulCount, new Vector2(playerRec.X + playerRec.Width * 5, playerRec.Y - playerRec.Height * 2f), Color.White);

            //Checks if gunState is Ar, if so draw weapon is assult rifle
            if (gunState == AR)
            {
                spriteBatch.DrawString(Game1.font, "Weapon: Assult Rifle", new Vector2(playerRec.X - playerRec.Width * 9, playerRec.Y - playerRec.Height * 1.5f), Color.White);
            }

            //Executes if gunState is burst, if so draw weapon is burst
            else
            {
                spriteBatch.DrawString(Game1.font, "Weapon: Burst", new Vector2(playerRec.X - playerRec.Width * 9, playerRec.Y - playerRec.Height * 1.5f), Color.White);
            }

            //Draws the shield bubble 
            spriteBatch.Draw(shieldImg, shieldRec, Color.Multiply(Color.White, shieldTrans));
        }

        //Pre: N/A 
        //Post: Returns player's rectangle
        //Description: Returns player's rectangle
        public Rectangle GetRec()
        {
            return playerRec;
        }

        //Pre: N/A 
        //Post: Returns player's angle
        //Description: Returns player's angle
        public float GetAngle()
        {
            return angle;
        }

        //Pre: N/A 
        //Post: Returns player's bullets
        //Description: Returns player's bullets
        public List<Bullet> GetBullets()
        {
            return bullets;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Sets all the modified variables back to their orginial values
        public void Reset()
        {
            bullets.Clear();
            grenades.Clear();
            speed = Vector2.Zero;
            sprint = false;
            health = 100;
            bulCount = 30;
            gunState = 1;
            grdCount = 5;
            playerRec.X = Game1.windowWidth / 2;
            playerRec.Y = Game1.windowHeight / 2;
            angle = 0;
            burstCount = 0;
            burstTimer = 0f;
            burstActive = false;
            shootTimer = 0f;
            grdPressed = false;
            shieldSpawnTimer = 0f;
            shieldActiveTimer = 0f;
        }
    }
}

