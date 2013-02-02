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
        private const float MAX_RANGE = 200;
        private List<Vector2> EnemyQuad1 = new List<Vector2>();
        private List<Vector2> EnemyQuad2 = new List<Vector2>();
        private List<Vector2> EnemyQuad3 = new List<Vector2>();
        private List<Vector2> EnemyQuad4 = new List<Vector2>();
        private Color Quad1Color;
        private Color Quad2Color;
        private Color Quad3Color;
        private Color Quad4Color;
        private Vector2 LastEnemyLocation;
       
        SpriteFont DebugFont;

        

        double degrees, DotProdDenom, DotprodNum;
        
       
        double test;
        double test2;
        Vector2 TempVector = new Vector2();
        Vector2 VectorA = new Vector2();
        Vector2 VectorB = new Vector2();
        
        public PieSensor(ref List<Actor> aWorldActors, Actor aHost, SpriteFont aDebugFont)
            : base(ref aWorldActors, aHost)
        {
            DebugFont = aDebugFont;
            
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
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + Math.PI / 4) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation + Math.PI / 4) * 100 + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 3*Math.PI / 4) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 3*Math.PI / 4) * 100 + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 5*Math.PI / 4) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 5* Math.PI / 4) * 100 + pHost.Position.Y));
            EndPoints.Add(new Vector2((float)Math.Cos(pHost.Rotation + 7 * Math.PI / 4) * 100 + pHost.Position.X, (float)Math.Sin(pHost.Rotation + 7 * Math.PI / 4) * 100 + pHost.Position.Y));

            
               

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
                        
                        //find the vector between the player and the point its heading to
                        VectorA = new Vector2(TempVector.X - pHost.Position.X, TempVector.Y - pHost.Position.Y);
                        
                        //find the vector between the player and the enemy
                        VectorB = new Vector2(A.Position.X - pHost.Position.X, A.Position.Y - pHost.Position.Y);
                        
                        //the dot product numerator 
                        DotprodNum = VectorA.X * VectorB.X + VectorA.Y * VectorB.Y;
                       
                        //the dot product denominator
                        DotProdDenom = Math.Sqrt(VectorA.X * VectorA.X + VectorA.Y * VectorA.Y) * (Math.Sqrt(VectorB.X * VectorB.X + VectorB.Y * VectorB.Y));
                        degrees = Math.Acos(DotprodNum / DotProdDenom);
                        
                        
                        
                        //the head of the player
                        if(degrees<Math.PI / 4)
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
                        else if(degrees>(3*Math.PI/4))
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
              //*****Still havent really figured out this part yet, still trying
                        else if(degrees > Math.PI/4&&(Vector2.Distance(A.Position,LastEnemyLocation)<Vector2.Distance(A.Position,pHost.Position)))
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
                  /////////*******   
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
