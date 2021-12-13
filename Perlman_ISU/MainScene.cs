//Author Name: Elad Perlman
//File Name: MainScene
//Project Name: Perlman_ISU
//Date Created: Dec 10th 2019
//Date Modified: Jan 19th 2020
//Description: This is a top down shooter game full of action, boss rounds, and vicious enemies that are prone to kill the player
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace Perlman_ISU
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Stores the images of all the different players
        private Texture2D playerImg;
        private Texture2D redPlayerImg;
        private Texture2D bluePlayerImg;

        //Stores the image of the player screen selection
        private Texture2D playerSelScr;

        //Stores all the player option buttons
        private Button regPlayerBtn;
        private Button redPlayerBtn;
        private Button bluePlayerBtn;

        //Stores the image of the in game's background
        private Texture2D bgImg;

        //Stores all background music
        private Song menuBgMusic;
        private Song inGameMusic;

        //Stores the fight sound effect
        private SoundEffect fightSnd;
        private SoundEffect gunShotSnd;

        //Stores the image and destination rectangle of the lose screen
        private Texture2D loseScrnImg;
        private Rectangle loseScrnRec;

        //Stores the player
        private Player player;

        //Stores the game's camera
        private Camera camera;

        //Stores the background generator instance
        private BackgroundGen bg;

        //Stores the images of all the enemies
        private Texture2D ghostImg;
        private Texture2D zombieImg;

        //Stores the screen's width and height
        public static int windowWidth;
        public static int windowHeight;
        
        //Stores the images of all the bullets in the game
        private Texture2D playerBulImg;
        private Texture2D enBulletImg;

        //Stores an instance of enemy manager
        private EnemyManager enManager;

        //Stores the images of all the equipments in the game
        private Texture2D ammoImg;
        private Texture2D medkitImg;
        private Texture2D grdImg;

        //Stores the image and destination rectangle of the player's base
        private Texture2D baseImg;
        private Rectangle baseRec;

        //Stores the sprite sheets of the explosion animations
        private Texture2D enExImg;
        private Texture2D grdExImg;

        //Stores the image of the player's shield bubble
        private Texture2D shieldImg;

        //Stores an instance of equipment manager
        private EquipmentManager eqManager;

        //Stores the image of the boss
        private Texture2D bossImg;

        //Stores an instance of boss manager
        private BossManager bsManager;

        //Stores an instance of round manager
        private RoundManager rndManager;

        //Stores the images of the shield bar 
        private Texture2D shieldBarImg;
        private Texture2D shieldAmountImg;

        //Stores the image and destination rectangle of the menu
        private Texture2D menuImg;
        private Rectangle menuRec;

        //Stores the images of all the buttons in the game
        private Texture2D playBtnImg;
        private Texture2D instBtnImg;
        private Texture2D exitBtnImg;
        private Texture2D backBtnImg;

        //Stores all the buttons in the game
        private Button playBtn;
        private Button instBtn;
        private Button exitBtn;
        private Button backBtn;
        private Button menuBtn;

        //Stores the output message's font
        public static SpriteFont font;

        //Stores the image and destination rectangle of the instruction screen
        private Texture2D instrImg;
        private Rectangle instrRec;

        //Stores the round the player died at and the highest round the player reached
        private int curRoundCount;
        private int highRoundCount;

        //Stores the FPS of the game
        public const float FPS = 60f;

        //Stores the current game state
        private int gameState = 3;
        
        //Stores all the different game states 
        private const int PLAY = 0;
        private const int EXIT = 1;
        private const int INSTR = 2;
        private const int MENU = 3;
        private const int LOSE = 4;
        private const int SKIN_SEL = 5;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Sets the width and height of the screen
            this.graphics.PreferredBackBufferWidth = 1500;
            this.graphics.PreferredBackBufferHeight = 800;
            
            //Allows for mouse to be visible
            IsMouseVisible = true;
            
            //Sets up the game's FPS
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / (int)FPS);
            
            this.graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {          
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Sets the width and height of the screen
            windowWidth = graphics.GraphicsDevice.Viewport.Width;
            windowHeight = graphics.GraphicsDevice.Viewport.Height;
            
            //Sets the grenades image
            grdImg = Content.Load<Texture2D>("Images/Sprites/grenade");

            //Sets up the fight and gunshot sound effects
            fightSnd = Content.Load<SoundEffect>("Sounds/Effects/Fight sound effect");
            gunShotSnd = Content.Load<SoundEffect>("Sounds/Effects/enemy gunshot");

            //Sets all the different player images
            playerImg = Content.Load<Texture2D>("Images/Sprites/player");
            redPlayerImg = Content.Load<Texture2D>("Images/Sprites/player red");
            bluePlayerImg = Content.Load<Texture2D>("Images/Sprites/player blue");
            
            //Sets the player's bullet image
            playerBulImg = Content.Load<Texture2D>("Images/Sprites/bullet player");
            
            //Sets the images of the shield bars and the shield bubble
            shieldImg = Content.Load<Texture2D>("Images/Sprites/shield");
            shieldBarImg = Content.Load<Texture2D>("Images/Sprites/shield bar");
            shieldAmountImg = Content.Load<Texture2D>("Images/Sprites/shield Amount");

            //Sets the image of the player selection screen
            playerSelScr = Content.Load<Texture2D>("Images/Sprites/player selection");
            
            //Sets up all the player buttons
            regPlayerBtn = new Button(playerImg, new Vector2(400, 400));
            redPlayerBtn = new Button(redPlayerImg, new Vector2(700, 400));
            bluePlayerBtn = new Button(bluePlayerImg, new Vector2(1000, 400));

            //Creates a new instance of the player class
            player = new Player(playerImg, playerBulImg, grdImg, shieldImg, shieldAmountImg, shieldBarImg, gunShotSnd);

            //Creates a new instance of the camera class
            camera = new Camera(GraphicsDevice.Viewport);

            //Sets the in game background image, and creates an instance of the background generator class
            bgImg = Content.Load<Texture2D>("Images/Sprites/brick bg");
            bg = new BackgroundGen(bgImg);

            //Sets up the menu's image and rectangle
            menuImg = Content.Load<Texture2D>("Images/Sprites/menu screen");
            menuRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Sets up the instruction screen's image and rectangle 
            instrImg = Content.Load<Texture2D>("Images/Sprites/instructions");
            instrRec = new Rectangle(0, 0, instrImg.Width, instrImg.Height);

            //Sets up the losing screen's image and rectangle 
            loseScrnImg = Content.Load<Texture2D>("Images/Sprites/lose screen");
            loseScrnRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Sets up all the button's images
            playBtnImg = Content.Load<Texture2D>("Images/Sprites/play button");
            instBtnImg = Content.Load<Texture2D>("Images/Sprites/instructions button");
            exitBtnImg = Content.Load<Texture2D>("Images/Sprites/exit button");
            backBtnImg = Content.Load<Texture2D>("Images/Sprites/back button");

            //Creates all the buttons in the game, and their location
            playBtn = new Button(playBtnImg, new Vector2(580, 250));
            instBtn = new Button(instBtnImg, new Vector2(580, 400));
            exitBtn = new Button(exitBtnImg, new Vector2(580, 550));
            backBtn = new Button(backBtnImg, new Vector2(100, 100));
            menuBtn = new Button(playBtnImg, new Vector2(580, 450));

            //Sets up the sprite sheet for the explosion animations
            enExImg = Content.Load<Texture2D>("Images/Animations/explode2");
            grdExImg = Content.Load<Texture2D>("Images/Animations/grenade explosion");

            //Sets up the images for all enemy types, and their bullet image
            ghostImg = Content.Load<Texture2D>("Images/Sprites/ghost");
            zombieImg = Content.Load<Texture2D>("Images/Sprites/enemy shoot");
            enBulletImg = Content.Load<Texture2D>("Images/Sprites/bullet enemy");

            //Creates a new instance of the enemy manager class
            enManager = new EnemyManager(ghostImg, zombieImg, enBulletImg, enExImg, grdExImg, 15, 10, gunShotSnd);

            //Sets up the images of all the equipment
            medkitImg = Content.Load<Texture2D>("Images/Sprites/medkit");
            ammoImg = Content.Load<Texture2D>("Images/Sprites/ammo box");

            //Sets up the player's base's image and rectangle
            baseImg = Content.Load<Texture2D>("Images/Sprites/base ");
            baseRec = new Rectangle(0, 0, windowWidth, windowHeight);                      

            //Creates a new instance of the equipment manager class
            eqManager = new EquipmentManager(ammoImg, medkitImg, grdImg);

            //Sets up the boss's image, and creates a new instance of the boss manager class
            bossImg = Content.Load<Texture2D>("Images/Sprites/boss isu");
            bsManager = new BossManager(bossImg, enBulletImg, grdExImg, 5);

            //Creates a new instance of the round manager class
            rndManager = new RoundManager(player, ghostImg, zombieImg, enBulletImg, enExImg, grdExImg, bossImg, enManager, bsManager, fightSnd, gunShotSnd);

            //Sets up the game font
            font = Content.Load<SpriteFont>("Fonts/Text");

            //Sets up the background music
            menuBgMusic = Content.Load<Song>("Sounds/Background/bg music isu");
            inGameMusic = Content.Load<Song>("Sounds/Background/duel of fates");
        }
             
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Checks the game's current state
            switch (gameState)
            {
                //Checks if the game state is the start screen and if it is it calls the menu's update function
                case MENU:
                    MenuUpdate();
                    break;

                //Checks if the game state is in game and if it is it calls the in game's update function
                case PLAY:
                    InGameUpdate(gameTime);
                    break;

                //Checks if the game state is the instructions and if it is it calls the instruction's update function
                case INSTR:
                    InstrUpdate();
                    break;

                //Checks if the game state is the lose screen and if it is it calls the lose screen's update function
                case LOSE:
                    LoseScreenUpdate();
                    break;

                //Checks if the game state is exit and if it is exit the game
                case EXIT:
                    Exit();
                    break;

                //Checks if the game state is skin slection and if it is call the screen selection update function
                case SKIN_SEL:
                    PlayerMenuUpdate();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);           
            
            //Checks the game's current state
            switch (gameState)
            {
                //Checks if the game state is the start screen and if it is it calls the menu's draw function
                case MENU:
                    MenuDraw();
                    break;

                //Checks if the game state is in game and if it is it calls the in game's draw function
                case PLAY:
                    InGameDraw();
                    break;

                //Checks if the game state is the instructions and if it is it calls the instruction's draw function
                case INSTR:
                    MenuDraw();
                    InstrDraw();
                    break;

                //Checks if the game state is the lose screen and if it is it calls the lose screen's draw function
                case LOSE:
                    LoseScreenDraw();
                    break;

                //Checks if the game state is skin slection and if it is call the screen selection draw function
                case SKIN_SEL:
                    PlayerMenuDraw();
                    break;
            }
       
            base.Draw(gameTime);
        }

       //Pre: gameTime is not null
       //Post: N/A
       //Description: Handles all the logic and physics of the playable game state
        private void InGameUpdate(GameTime gameTime)
        {
            //Checks if any song is playing 
            if (MediaPlayer.State == MediaState.Stopped)
            {
                //Plays in game music 
                MediaPlayer.Play(inGameMusic);
            }

            //Checks if player died
            if (player.health <= 0)
            {
                //Sets current round to the round that the player died at
                curRoundCount = rndManager.roundCount;

                //Checks if current round is higher than the highscore, if so sets the current round to highscore
                if (curRoundCount > highRoundCount)
                {
                    highRoundCount = curRoundCount;
                }

                //Sets game state to lose screen, and stops music
                gameState = LOSE;
                MediaPlayer.Stop();
            }
            
            //Calls the background generator function
            bg.Generate(player.GetRec());

            //Calls the equipment spawner function
            eqManager.EqSpawner();

            //Calls the player's move, shoot and shield functions
            player.MovePlayer();
            player.Shoot();
            player.ActivateShield();

            //Adds health grenades and bullets to the player based on the called functions
            player.health += eqManager.DeleteHealth(player.recLines);
            player.grdCount += eqManager.DeleteGrenade(player.recLines);
            player.bulCount += eqManager.DeleteAmmo(player.recLines);

            //Calls the round function of round manager
            rndManager.Round(gameTime);

            //Calls the camera following function
            camera.Update(player.GetRec());
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the in game content
        private void InGameDraw()
        {
            //begins the spriteBatch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            //Draws all the backgrounds
            bg.Draw(spriteBatch);

            //Draws the player's base
            spriteBatch.Draw(baseImg, baseRec, Color.White);

            //Draws all the equipment
            eqManager.Draw(spriteBatch);

            //Draws all the player's components
            player.Draw(spriteBatch);

            //Draws all the round manager's components
            rndManager.Draw(spriteBatch, player.GetRec());

            //Ends spriteBatch
            spriteBatch.End();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Checks which button was pressed and switches the game state occordingly 
        private void MenuUpdate()
        {
            //Checks if any song is playing 
            if (MediaPlayer.State == MediaState.Stopped)
            {
                //Plays start screen music
                MediaPlayer.Play(menuBgMusic);
            }

            //Checks if play button was pressed, if so set gameState to skin selection, and stops music
            if (playBtn.ButtonPressed())
            {
                gameState = SKIN_SEL;
            }

            //Checks if instructions button was pressed, if so set gameState to instructions 
            else if (instBtn.ButtonPressed())
            {
                gameState = INSTR;
            }

            //Checks if the exit button was pressed if so sets gamState to exit
            else if (exitBtn.ButtonPressed())
            {
                gameState = EXIT;
            }
        }
        
        //Pre: N/A
        //Post: N/A
        //Description: Draws all of the menu's components
        private void MenuDraw()
        {
            //Begins spriteBatch
            spriteBatch.Begin();

            //Draws the menu screen
            spriteBatch.Draw(menuImg, menuRec, Color.White);
            
            //Draws all the menu button's
            playBtn.Draw(spriteBatch);
            instBtn.Draw(spriteBatch);
            exitBtn.Draw(spriteBatch);

            //Ends spriteBatch
            spriteBatch.End();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Checks if back button was pressed, if so sets gameState to menu
        private void InstrUpdate()
        {
            if (backBtn.ButtonPressed())
            {
                gameState = MENU;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the instuction's components
        private void InstrDraw()
        {
            //Begins spriteBatch
            spriteBatch.Begin();
            
            //Draws the menu instructions screen and the back button 
            spriteBatch.Draw(instrImg, instrRec, Color.White);
            backBtn.Draw(spriteBatch);

            //Ends spriteBatch
            spriteBatch.End();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic in the skin selection game state
        private void PlayerMenuUpdate()
        {
            //Checks if player picked the regular player, if so set player's image to regular and start game
            if (regPlayerBtn.ButtonPressed())
            {
                player.playerImg = playerImg;
                gameState = PLAY;
                MediaPlayer.Stop();
            }

            //Checks if player picked the red player, if so set player's image to red player and start game
            else if (redPlayerBtn.ButtonPressed())
            {
                player.playerImg = redPlayerImg;
                gameState = PLAY;
                MediaPlayer.Stop();
            }

            //Checks if player picked the blue player, if so set player's image to blue player and start game
            else if (bluePlayerBtn.ButtonPressed())
            {
                player.playerImg = bluePlayerImg;
                gameState = PLAY;
                MediaPlayer.Stop();
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the skin selection screen's components
        private void PlayerMenuDraw()
        {
            //Begins spriteBatch
            spriteBatch.Begin();

            //Draws the screen selection image
            spriteBatch.Draw(playerSelScr, menuRec, Color.White);
            
            //Draws all the player options
            regPlayerBtn.Draw(spriteBatch);
            redPlayerBtn.Draw(spriteBatch);
            bluePlayerBtn.Draw(spriteBatch);
            
            //Ends spriteBatch
            spriteBatch.End();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets all of the games components
        private void Reset()
        {
            player.Reset();
            rndManager.Reset();
            eqManager.Reset();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all of the lose screen's functionallity
        private void LoseScreenUpdate()
        {            
            //Checks if menu button was pressed, if so restart game and switch game state to menu
            if (menuBtn.ButtonPressed())
            {
                Reset();
                gameState = MENU;
            }

            //Checks if exit button was clicked, if so exit game
            if (exitBtn.ButtonPressed())
            {
                gameState = EXIT;
            }
        }

        //Pre: N/A 
        //Post: N/A
        //Description: Draws all the lose screen's components
        private void LoseScreenDraw()
        {
            //Begins spriteBatch
            spriteBatch.Begin();

            //Draws the lose screen
            spriteBatch.Draw(loseScrnImg, loseScrnRec, Color.White);
            spriteBatch.DrawString(font, Convert.ToString(curRoundCount), new Vector2(645, 300), Color.White);
            spriteBatch.DrawString(font, Convert.ToString(highRoundCount), new Vector2(845, 375), Color.White);
            
            //Draws the menu and exit buttons
            menuBtn.Draw(spriteBatch);
            exitBtn.Draw(spriteBatch);

            //Ends spriteBatch
            spriteBatch.End();
        }
    }
}
