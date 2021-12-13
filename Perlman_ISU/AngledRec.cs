//Author Name: Elad Perlman
//File Name: AngledRec
//Project Name: Perlman_ISU
//Date Created: Jan 1th 2019
//Date Modified: Jan 17th 2020
//Description: This class is in charge of creating new rotated vertacies of a rectangle based on a given angle
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class AngledRec
    {
        //Stores the position of all the rectangle's vertacies
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        //Stores the position of the center of the rectangle
        public Vector2 center;

        //stores the distance from the center to any vertex
        public float radius;

        //Pre: N/A
        //Post: N/A
        //Description: Used to create instances of AngledRec
        public AngledRec()
        {            
        }

        //Pre: box is not null, newAngle is a valid float
        //Post: N/A
        //Description: Creates the new vertacies of the rectangle after a rotation
        public void NewRec(Rectangle box, float newAngle)
        {
            //Shifts the rectangle back to its orginal position
            box.X -= box.Width / 2;
            box.Y -= box.Height / 2;

            //sets center to the box's center point
            center = box.Center.ToVector2();

            //calculates the distance from the center to any of the rectangle's vertecies
            radius = (float)Math.Sqrt(Math.Pow(box.Width / 2.0, 2) + Math.Pow(box.Height / 2.0, 2));

            //Flips the new angle's value
            newAngle *= -1;

            //Calculates the orginial angle any vertex makes with the center of the rectangle
            double ogAngle = Math.Atan2(box.Height / 2.0, box.Width / 2.0);

            //sets all the vertacies to the new rotated vertacies
            topLeft = VertCalc(MathHelper.ToRadians(180) - ogAngle, newAngle, center, radius);
            topRight = VertCalc(ogAngle, newAngle, center, radius);
            bottomLeft = VertCalc(MathHelper.ToRadians(180) + ogAngle, newAngle, center, radius);
            bottomRight = VertCalc(-ogAngle, newAngle, center, radius);
        }

        //Pre: ogAngle, newAngle, and center are not null, radius is a positive float
        //Post: returns the new rotated vertex based on its orginal vertex
        //Post: Calculates the position of the rotated vertex based on the new angle given
        public Vector2 VertCalc(double ogAngle, float newAngle, Vector2 center, float radius)
        {
            Vector2 newVert = new Vector2((float)Math.Cos(ogAngle + MathHelper.ToRadians(newAngle)), (float)(-Math.Sin(ogAngle + MathHelper.ToRadians(newAngle))));
            newVert *= radius;

            newVert += center;

            return newVert;
        }
    }
}
