//Author Name: Elad Perlman
//File Name: Medkit
//Project Name: Perlman_ISU
//Date Created: Jan 3th 2020
//Date Modified: Jan 17th 2020
//Description: This class stores type medkit of Equipment
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Medkit : Equipment
    {
        //Pre: medkitImg is not null
        //Post: N/A
        //Description: Inherets its base class Equipment
        public Medkit(Texture2D medkitImg) : base(medkitImg)
        {
        }
    }
}
