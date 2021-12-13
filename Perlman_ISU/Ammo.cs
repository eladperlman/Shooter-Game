//Author Name: Elad Perlman
//File Name: Ammo
//Project Name: Perlman_ISU
//Date Created: Jan 1th 2019
//Date Modified: Jan 17th 2020
//Description: This class is a type of Equipment 
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{  
    class Ammo : Equipment
    {
        //Pre: N/A
        //Post: N/A
        //Description: Inherets base class Equipment
        public Ammo(Texture2D ammoImg) : base(ammoImg)
        {
        }        
    }
}
