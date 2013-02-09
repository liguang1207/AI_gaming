using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace The_Dungeon.BLL
{
    class AgentSensor: Sensor
    {
        private List<Vector2> EndPoints = new List<Vector2>();
        private const float MAX_RANGE = 64;
        double radian, radian90, degrees;


        Vector2 TempVector = new Vector2();
        Vector2 Vector90Degree = new Vector2();

        Vector2 VectorA = new Vector2();
        Vector2 VectorB = new Vector2();

        Vector2 VectorA90 = new Vector2();


        public AgentSensor(ref List<Actor> aWorldActors, Actor aHost, SpriteFont aDebugFont)
            : base(ref aWorldActors, aHost, aDebugFont)
        {

        }

        public override void Sense()
        {
            base.Sense();

            EndPoints = new List<Vector2>();
            foreach (Actor A in pWorldActors)
            {
                if (A is Pawn && A != pHost)
                {
                    //Get Distance
                    float Distance = Vector2.Distance(A.Position, pHost.Position);
                    if (Distance <= MAX_RANGE + A.CollisionRectangle.Width/2)
                    {
                        EndPoints.Add(A.Position);
                        //find a random point that the heading is point to
                        TempVector = new Vector2((float)Math.Cos(pHost.Rotation) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation) * 100 + pHost.Position.Y);

                        //Made a point that is 90 degrees of the heading - this is used for testing the angles for the two sides 
                        Vector90Degree = new Vector2((float)Math.Cos(pHost.Rotation + (Math.PI / 2)) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation + (Math.PI / 2)) * 100 + pHost.Position.Y);

                        //find the vector between the player and the point its heading to
                        VectorA = new Vector2(TempVector.X - pHost.Position.X, TempVector.Y - pHost.Position.Y);
                        VectorA90 = new Vector2(Vector90Degree.X - pHost.Position.X, Vector90Degree.Y - pHost.Position.Y);

                        //find the vector between the player and the enemy
                        VectorB = new Vector2(A.Position.X - pHost.Position.X, A.Position.Y - pHost.Position.Y);



                        //degree of the enemy to the head of the player.
                        //if enemy is 90 degrees to the head of the player then the angle will be pi/2
                        radian = DotProduct(VectorA, VectorB);
                        degrees = radian * 180 / Math.PI;

                        //this is the degree of the enemy to pi/2 degrees of the head. 
                        //so if the enemy is pi/2 degrees to the original head of the player, the new angle will be 0.
                        radian90 = DotProduct(VectorA90, VectorB);
                       
                        if (radian90 < Math.PI/2)
                        {
                            degrees  = 360 - radian;
                        }

                        DebugInformation += "Enemy(" + (EndPoints.Count).ToString() + ") - " + Distance + " , Heading: " + degrees +"\r\n";
                    }
                }
            }
        }
        public double DotProduct(Vector2 A, Vector2 B)
        {
            double radians, DotProdNum, DotProdDenom;
            //the dot product numerator 
            DotProdNum = A.X * B.X + A.Y * B.Y;
            //the dot product denominator
            DotProdDenom = Math.Sqrt(A.X * A.X + A.Y * A.Y) * (Math.Sqrt(B.X * B.X + B.Y * B.Y));

            radians = Math.Acos(DotProdNum / DotProdDenom);
            return radians;
        }
        public override void Draw(ref SpriteBatch SB)
        {
            base.Draw(ref SB);

            DrawingHelper.DrawCircle(pHost.Position, MAX_RANGE, Color.Blue, false);

            DrawingHelper.Begin(PrimitiveType.LineList);
            for (int i = 0; i < EndPoints.Count; i++)
            {
                DrawingHelper.DrawLine(pHost.Position, EndPoints[i], Color.Yellow);
            }
            DrawingHelper.End();
        }
    }
}

