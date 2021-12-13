//Author Name: Elad Perlman
//File Name: Medkit
//Project Name: Perlman_ISU
//Date Created: Jan 3th 2019
//Date Modified: Jan 17th 2020
//Description: This class stores type medkit of GrenadeEq
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class GrenadeEq : Equipment
    {
        //Pre: grdImg is not null
        //Post: N/A
        //Description: Inherets its base class Equipment
        public GrenadeEq(Texture2D grdImg) : base(grdImg)
        {
        }
    }
}
