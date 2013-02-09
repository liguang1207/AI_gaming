using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace The_Dungeon.BLL
{
    class PieSensor : Sensor
    {
        private List<Vector2> EndPoints = new List<Vector2>();
        private const float MAX_RANGE = 150;
        private List<Vector2> EnemyQuad1 = new List<Vector2>();
        private List<Vector2> EnemyQuad2 = new List<Vector2>();
        private List<Vector2> EnemyQuad3 = new List<Vector2>();
        private List<Vector2> EnemyQuad4 = new List<Vector2>();
        private Color Quad1Color;
        private Color Quad2Color;
        private Color Quad3Color;
        private Color Quad4Color;
        private Vector2 LastEnemyLocation;
       
        



        double radian, radian90;
        
       
        double test;
        double test2;
        Vector2 TempVector = new Vector2();
        Vector2 Vector90Degree = new Vector2();

        Vector2 VectorA = new Vector2();
        Vector2 VectorB = new Vector2();

        Vector2 VectorA90 = new Vector2();

        public PieSensor(ref List<Actor> aWorldActors, Actor aHost, SpriteFont aDebugFont)
            : base(ref aWorldActors, aHost, aDebugFont)
        {
            
        }

        public override void Sense()
        {
            base.Sense();


            
            EndPoints.Clear();
            EnemyQuad1.Clear();
            EnemyQuad2.Clear();
            EnemyQuad3.Clear();
            EnemyQuad4.Clear();
            
           
           //dividing the quadrants of the pie sensor
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + Math.PI / 4) * MAX_RANGE + pHost.Position.X, (float)Math.Sin(pHost.Rotation + Math.PI / 4) * MAX_RANGE + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 3 * Math.PI / 4) * MAX_RANGE + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 3 * Math.PI / 4) * MAX_RANGE + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 5 * Math.PI / 4) * MAX_RANGE + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 5 * Math.PI / 4) * MAX_RANGE + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 7 * Math.PI / 4) * MAX_RANGE + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 7 * Math.PI / 4) * MAX_RANGE + pHost.Position.Y));

            
               

            foreach (Actor A in pWorldActors)
            {

                if (A is Pawn && A != pHost)
                {

                    
                    //Get Distance
                    float Distance = Vector2.Distance(A.Position, pHost.Position);
                    if (Distance <= MAX_RANGE + A.CollisionRectangle.Width/2)
                    {

                        
                        
                        //find a random point that the heading is point to
                        TempVector = new Vector2((float)Math.Cos(pHost.Rotation) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation) * 100 + pHost.Position.Y);
                        
                        //Made a point that is 90 degrees of the heading - this is used for testing the angles for the two sides 
                        Vector90Degree = new Vector2((float)Math.Cos(pHost.Rotation+(Math.PI/2)) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation+(Math.PI/2)) * 100 + pHost.Position.Y);
                        
                        //find the vector between the player and the point its heading to
                        VectorA = new Vector2(TempVector.X - pHost.Position.X, TempVector.Y - pHost.Position.Y);
                        VectorA90 = new Vector2(Vector90Degree.X - pHost.Position.X, Vector90Degree.Y - pHost.Position.Y);
                        
                        //find the vector between the player and the enemy
                        VectorB = new Vector2(A.Position.X - pHost.Position.X, A.Position.Y - pHost.Position.Y);
                        
                       
                        
                        //degree of the enemy to the head of the player.
                        //if enemy is 90 degrees to the head of the player then the angle will be pi/2
                        radian = DotProduct(VectorA, VectorB); 
                           

                        //this is the degree of the enemy to pi/2 degrees of the head. 
                        //so if the enemy is pi/2 degrees to the original head of the player, the new angle will be 0.
                        radian90 = DotProduct(VectorA90, VectorB);
                            
                        
                        //the head of the player
                        if(radian<Math.PI / 4)
                        {
                            EnemyQuad1.Add(A.Position);
                            if (EnemyQuad1.Count == 1)
                            {
                                Quad1Color = Color.Green;
                            }
                            else if (EnemyQuad1.Count == 2)
                            {
                                Quad1Color = Color.Orange;
                            }
                            else
                            {
                                Quad1Color = Color.Red;
                            }
                        }
                        
                        //the tail of the player 
                        else if(radian>(3*Math.PI/4))
                        {
                            EnemyQuad2.Add(A.Position);
                            if (EnemyQuad2.Count == 1)
                            {
                                Quad2Color = Color.Green;
                            }
                            else if (EnemyQuad2.Count == 2)
                            {
                                Quad2Color = Color.Orange;
                            }
                            else
                            {
                                Quad2Color = Color.Red;
                            }
                        }
              
                        else if(radian90<Math.PI/4)
                        {
                            LastEnemyLocation = A.Position;
                            EnemyQuad3.Add(A.Position);
                            
                            if (EnemyQuad3.Count == 1)
                            {
                                Quad3Color = Color.Green;
                            }
                            else if (EnemyQuad3.Count == 2)
                            {
                                Quad3Color = Color.Orange;
                            }
                            else
                            {
                                Quad3Color = Color.Red;
                            }
                        }
                   
                        else
                        {
                            EnemyQuad4.Add(A.Position);
                            if (EnemyQuad4.Count == 1)
                            {
                                Quad4Color = Color.Green;
                            }
                            else if (EnemyQuad4.Count == 2)
                            {
                                Quad4Color = Color.Orange;
                            }
                            else
                            {
                                Quad4Color = Color.Red;
                            }
                        }
                        
                    }
                }
            }

            DebugInformation = "[Q1 - " + EnemyQuad1.Count.ToString() + ", Q2 - " + EnemyQuad2.Count.ToString() + ", Q3 - " + EnemyQuad3.Count().ToString() + ", Q4 - " + EnemyQuad4.Count.ToString() + "]";
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
            int j;
            base.Draw(ref SB);

            DrawingHelper.DrawCircle(pHost.Position, MAX_RANGE, Color.Blue, false);

            DrawingHelper.Begin(PrimitiveType.LineList);
            for (int i = 0; i < EndPoints.Count; i++)
            {
                DrawingHelper.DrawLine(pHost.Position, EndPoints[i], Color.Yellow);
            }
            for (j = 0; j < EnemyQuad1.Count; j++)
            {
                DrawingHelper.DrawLine(pHost.Position, EnemyQuad1[j], Quad1Color);

            }
            for (j = 0; j < EnemyQuad2.Count; j++)
            {
                DrawingHelper.DrawLine(pHost.Position, EnemyQuad2[j], Quad2Color);

            }
            for (j = 0; j < EnemyQuad3.Count; j++)
            {
                DrawingHelper.DrawLine(pHost.Position, EnemyQuad3[j], Quad3Color);

            }
            for (j = 0; j < EnemyQuad4.Count; j++)
            {
                DrawingHelper.DrawLine(pHost.Position, EnemyQuad4[j], Quad4Color);

            }
            //SB.DrawString(DebugFont, (pHost.Rotation).ToString() +" "+ (((3*Math.PI / 4) - pHost.Rotation)% (2 * Math.PI)).ToString() + " "+ EnemyPos.ToString(), new Vector2(100f, 300f), Color.Red);
            //SB.DrawString(DebugFont, LastEnemyLocation.ToString(), new Vector2(100f, 350f), Color.Red);

            DrawingHelper.End();
        }
    }
}
