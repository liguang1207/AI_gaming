using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dungeon.BLL
{
    class Ray2D
    {
            private Vector2 StartPosition;
            private Vector2 EndPosition;
            private readonly List<Point> result;

            public Ray2D(Vector2 aStartPosition, Vector2 aEndPosition)
            {
                StartPosition = aStartPosition;
                EndPosition = aEndPosition;
                result = new List<Point>();
            }

            public Vector2 Intersects(Rectangle rectangle)
            {
                //Initial Points
                Point p0 = new Point((int)StartPosition.X, (int)StartPosition.Y);
                Point p1 = new Point((int)EndPosition.X, (int)EndPosition.Y);

                foreach (Point testPoint in BresenhamLine(p0, p1))
                {
                    if (rectangle.Contains(testPoint)) //If the Rectangle has this point in it & it is in the line
                        return new Vector2((float)testPoint.X, (float)testPoint.Y);
                }
                return Vector2.Zero;
            }

            // Swap the values of A and B  

            private void Swap<T>(ref T a, ref T b)
            {
                T c = a;
                a = b;
                b = c;
            }

            // Returns the list of points from p0 to p1   

            private List<Point> BresenhamLine(Point p0, Point p1)
            {
                return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
            }

            // Returns the list of points from (x0, y0) to (x1, y1)  

            private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
            {
                // Optimization: it would be preferable to calculate in  
                // advance the size of "result" and to use a fixed-size array  
                // instead of a list.  

                result.Clear();

                bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
                if (steep)
                {
                    Swap(ref x0, ref y0);
                    Swap(ref x1, ref y1);
                }
                if (x0 > x1)
                {
                    Swap(ref x0, ref x1);
                    Swap(ref y0, ref y1);
                }

                int deltax = x1 - x0;
                int deltay = Math.Abs(y1 - y0);
                int error = 0;
                int ystep;
                int y = y0;
                if (y0 < y1) ystep = 1; else ystep = -1;
                for (int x = x0; x <= x1; x++)
                {
                    if (steep) result.Add(new Point(y, x));
                    else result.Add(new Point(x, y));
                    error += deltay;
                    if (2 * error >= deltax)
                    {
                        y += ystep;
                        error -= deltax;
                    }
                }

                return result;
            }
        
    }
}
