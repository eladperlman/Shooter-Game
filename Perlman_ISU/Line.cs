//Author Name: Elad Perlman
//File Name: Line
//Project Name: Perlman_ISU
//Date Created: Dec 27th 2019
//Date Modified: Jan 17th 2020
//Description: This class stores the start and end points of a line
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class Line
    {
        //Stores the line's start and endpoint's position
        public readonly Vector2 startPoint;
        public readonly Vector2 endPoint;

        //Pre: startPoint and endPoint are both valid Vector2 and aren't null
        //Post: N/A
        //Description: Sets all global variables to their local ones
        public Line(Vector2 startPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}
