//Author Name: Elad Perlman
//File Name: PerPixelCol
//Project Name: Perlman_ISU
//Date Created: Jan 2th 2020
//Date Modified: Jan 17th 2020
//Description: This class is in charge of checking any collision type within the game
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perlman_ISU
{
    class PerPixelCol
    {
        //Pre: N/A
        //Post: N/A
        //Description: Used to create instances of PerPixelCol
        public PerPixelCol()
        {          
        }
        
        //Pre: both rec1 and rec2 are not null
        //Post: returns if the two rectangles collided
        //Description: Checks if the two given rectangles collided and returns the result
        public bool CollisionDec(Rectangle rec1, Rectangle rec2)
        {
            return !((rec1.X + rec1.Width) < rec2.X || rec1.X > (rec2.X + rec2.Width) || rec1.Y > (rec2.Y + rec2.Height) || (rec1.Y + rec1.Height) < rec2.Y);
        }       

        //Pre: line1 and line2 are both not null
        //Post: returns weather both lines collided or not
        //Description: checks if line1 and line2 collided and returns the result
        public bool LineToLine(Line line1, Line line2)
        {
            //stores if line1 and line2 are vertical lines
            bool isLine1Vert = line1.startPoint.X == line1.endPoint.X;
            bool isLine2Vert = line2.startPoint.X == line2.endPoint.X;

            //checks if both lines are vertical and are on the same x position
            if (isLine1Vert && isLine2Vert && line1.startPoint.X == line2.endPoint.X)
            {
                //checks if the y values of the two line segments are intersecting, if so returns true
                if (Math.Min(Math.Max(line1.startPoint.Y, line1.endPoint.Y), Math.Max(line2.startPoint.Y, line2.endPoint.Y)) >
                                Math.Max(Math.Min(line1.startPoint.Y, line1.endPoint.Y), Math.Min(line2.startPoint.Y, line2.endPoint.Y)))
                {
                    return true;
                }
            }

            //Checks if the first line is a vertical line and the second is not
            if (isLine1Vert && !isLine2Vert)
            {
                //Stores the slope and y-intercept of the second line
                float slope = CalcSlope(line2);
                float yInt = CalcYInt(line2, slope);
                
                //stores the point of intersection of both lines
                Vector2 poi = new Vector2(line1.startPoint.X, slope * line1.startPoint.X + yInt);

                //checks if the point of intersection is within the two line segments, if so return true
                if (IsPointInBound(line1, line2, poi))
                {
                    return true;
                }
            }

            //Checks if the second line is a vertical line and the first is not
            if (!isLine1Vert && isLine2Vert)
            {
                //Stores the slope and y-intercept of the first line
                float slope = CalcSlope(line1);
                float yInt = CalcYInt(line1, slope);

                //stores the point of intersection of both lines
                Vector2 poi = new Vector2(line2.startPoint.X, slope * line2.startPoint.X + yInt);

                //checks if the point of intersection is within the two line segments, if so return true
                if (IsPointInBound(line1, line2, poi))
                {
                    return true;
                }
            }

            //calculates the slope and y-intercept of both lines
            float slope1 = CalcSlope(line1);
            float yInt1 = CalcYInt(line1, slope1);
            float slope2 = CalcSlope(line2);
            float yInt2 = CalcYInt(line2, slope2);

            
            //checks if the two lines identical
            if (slope1 == slope2 && yInt1 == yInt2)
            {
                //checks if the two line segments overlap eachother, if so returns true
                if ((Math.Min(line1.startPoint.X, line1.endPoint.X) > Math.Min(line2.startPoint.X, line2.endPoint.X) &&
                    Math.Min(line1.startPoint.X, line1.endPoint.X) < Math.Max(line2.startPoint.X, line2.endPoint.X)) ||
                    (Math.Max(line1.startPoint.X, line1.endPoint.X) > Math.Min(line2.startPoint.X, line2.endPoint.X) &&
                    Math.Min(line1.startPoint.X, line1.endPoint.X) < Math.Max(line2.startPoint.X, line2.endPoint.X)))
                {
                    return true;
                }
            }

            //stores the point of intersection of both lines
            Vector2 poi2;
            poi2.X = (yInt2 - yInt1) / (slope1 - slope2);
            poi2.Y = slope1 * poi2.X + yInt1;

            //checks if the point of intersection is within the two line segments, if so return true
            if (IsPointInBound(line1, line2, poi2))
            {
                return true;
            }

            //returns that the two line segments did not intersect
            return false;

            //Pre: line is not null
            //Post: returns the slope of the line
            //Description: Calculates and returns the slope of the line
            float CalcSlope(Line line)
            {
                return (line.startPoint.Y - line.endPoint.Y) / (line.startPoint.X - line.endPoint.X);
            }

            //Pre: line is not null, and slope is a valid float that is not undefined
            //Post: returns the y-intercept of the line
            //Description: Calculates and returns the y-intercept of the line
            float CalcYInt(Line line, float slope)
            {
                return line.startPoint.Y - slope * line.startPoint.X;
            }

            //Pre: lines1 and lines2 are not null, and poi is a valid Vector2
            //Post: Returns if the point of intersection is located on both line segments
            //Description: Checks if point of intersection is located on both line segments and returns the result
            bool IsPointInBound(Line lines1, Line lines2, Vector2 poi)
            {
                //Checks if point of intersection is located on both line segments and returns the result
                if (poi.X >= Math.Min(lines1.startPoint.X, lines1.endPoint.X) &&
                           poi.X <= Math.Max(lines1.startPoint.X, lines1.endPoint.X) &&
                           poi.X >= Math.Min(lines2.startPoint.X, lines2.endPoint.X) &&
                           poi.X <= Math.Max(lines2.startPoint.X, lines2.endPoint.X) &&
                           poi.Y >= Math.Min(lines1.startPoint.Y, lines1.endPoint.Y) &&
                           poi.Y <= Math.Max(lines1.startPoint.Y, lines1.endPoint.Y) &&
                           poi.Y >= Math.Min(lines2.startPoint.Y, lines2.endPoint.Y) &&
                           poi.Y <= Math.Max(lines2.startPoint.Y, lines2.endPoint.Y))
                {
                    return true;
                }

                return false;
            }
        }

        //Pre: lines1 and lines2 contain four Line elements, and both are not null
        //Post: Returns if the two rectangles collided or not
        //Description: Calculates if any of the line segments of the two rectangles collide with eachother and returns the result
        public bool AngledBoxCol(Line[] lines1, Line[] lines2)
        {
            //incriments through each line in lines1
            for (int i = 0; i < lines1.Length; i++)
            {
                //incriments through each line in lines2
                for (int j = 0; j < lines2.Length; j++)
                {
                    //checks if the two line segments collide and if so, returns true
                    if (LineToLine(lines1[i], lines2[j]))
                    {
                        return true;
                    }
                }
            }

            //Returns that the two rectangles did not collide
            return false;
        }
    }
}
