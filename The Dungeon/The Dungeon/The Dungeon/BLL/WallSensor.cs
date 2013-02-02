using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace The_Dungeon.BLL
{
    class WallSensor : Sensor
    {
        private List<Vector2> EndPoints = new List<Vector2>();

        private List<float> intersectDistance = new List<float>();
        private const float MAX_RANGE = 100f;


       
        public WallSensor(ref List<Actor> aWorldActors, Actor aHost)
            : base(ref aWorldActors, aHost)
        {

        }

        public override void Sense()
        {
            base.Sense();
            EndPoints = new List<Vector2>();
                
                for (int i = 0; i < 3; i++) //3 Feelers
                {
                    float RotationOffset = 0f;
                    if (i == 0) { RotationOffset = -0.4f; }
                    else if (i == 2) { RotationOffset = 0.4f; }

                    //Get Next Position
                    Vector2 Velocity = pHost.Velocity;
                    Velocity.X = (float)Math.Cos(pHost.Rotation + RotationOffset) * (MAX_RANGE + pHost.RotationalVelocity);
                    Velocity.Y = (float)Math.Sin(pHost.Rotation + RotationOffset) * (MAX_RANGE + pHost.RotationalVelocity);
                    Vector2 NextPosition = pHost.Position + Velocity;

                    //Make Ray
                    Ray2D R = new Ray2D(pHost.Position, NextPosition);
                    Boolean HitWall = false;

                    foreach (Actor A in pWorldActors)
                    {
                        if (A is BlockingActor) //If its a wall
                        {
                                //Get Distance
                                float Distance = Vector2.Distance(A.Position, pHost.Position);
                                if (Distance <= MAX_RANGE + pHost.RotationalVelocity) //In Range
                                {
                                    Vector2 HitAt = R.Intersects(A.CollisionRectangle); //Check if it intersects
                                    if (HitAt != Vector2.Zero)
                                    {
                                        EndPoints.Add(HitAt);
                                        HitWall = true;
                                    }
                                }
                            }


                        }

                        if (!HitWall)
                        {
                            EndPoints.Add(NextPosition);
                        }
                    }
                
        }

        public override void Draw(ref SpriteBatch SB)
        {
            base.Draw(ref SB);
            
            DrawingHelper.Begin(PrimitiveType.LineList);
            
            for (int i = 0; i < EndPoints.Count; i++)
            {
                DrawingHelper.DrawLine(pHost.Position, EndPoints[i], Color.Yellow);
            }
            DrawingHelper.End();
        }
    }
}
