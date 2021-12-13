//Author Name: Elad Perlman
//File Name: Camera
//Project Name: Perlman_ISU
//Date Created: Dec 24th 2019
//Date Modified: Jan 17th 2020
//Description: This class moves the game's camera based on the player's position
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Camera
    {
        //Stores the cameras matrix, and center
        public Matrix transform;
        private Vector2 center;

        //Stores viewport variable
        private Viewport viewport;

        //Pre: viewport is not null
        //Post: N/A
        //Description: sets viewport equal to its local one
        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        //Pre: player is not null
        //Post: N/A
        //Description: transforms the camera based on player's location
        public void Update(Rectangle player)
        {                    
            //Sets center location equal to player's location
            center.X = player.X;
            center.Y = player.Y;

            //Adjusts the camera based on the player's position
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }
    }
}

