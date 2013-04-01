using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class turrets
    {
        public Vector2 spritePosition;
        public Vector2 spriteCenter;
        public Vector2 spriteVelocity;
        public Texture2D playerSprite;
        public float spriteSpeed = 5;
        public float seekSpeed = 2;
        const float rotationVelocity = 4f;
        public float Rotation = 0;
        public Rectangle turretRec;
        public float distance;
        public Vector2 TempVector;
        public Vector2 Vector90Degree;

        public Vector2 VectorA;
        public Vector2 VectorB;
        public Vector2 VectorA90;
        public double radian, radian90;
        public Vector2 seekCoord = new Vector2(-1, -1);

        public MouseState mouse;
        public MouseState preState;
        public Vector2 click = new Vector2(-1, -1);

        private const float MAX_RANGE = 150;

        public turrets(Vector2 Pos, Vector2 Center, Rectangle rec, Texture2D sprite)
        {
            spritePosition = Pos;
            spriteCenter = Center;
            turretRec = rec;
            playerSprite = sprite;
        }




        //calculating the distance between two vectors 
        public float calculateDistance(Vector2 A, Vector2 B)
        {
            float Y_diff, X_diff, distance;
            A = new Vector2(Math.Abs(A.X), Math.Abs(A.Y));
            B = new Vector2(Math.Abs(B.X), Math.Abs(B.Y));

            Y_diff = A.Y - B.Y;
            X_diff = A.X - B.X;

            distance = (float)Math.Sqrt(Math.Pow(X_diff, 2) + Math.Pow(Y_diff, 2));

            return Math.Abs(distance);

        }

        //set the turret head location to the location of the enemy
        public void ScanEnemies(List<player> enemy)
        {
            foreach (player P in enemy)
            {
                distance = Vector2.Distance(spritePosition, P.spritePosition);

                if (distance <= MAX_RANGE)
                {
                    //find a random point that the heading is point to
                    TempVector = new Vector2((float)Math.Cos(Rotation) * 50 + spritePosition.X, (float)Math.Sin(Rotation) * 50 + spritePosition.Y);

                    //Made a point that is 90 degrees of the heading - this is used for testing the angles for the two sides 
                    Vector90Degree = new Vector2((float)Math.Cos(Rotation + (Math.PI / 2)) * 100 + spritePosition.X, (float)Math.Sin(Rotation + (Math.PI / 2)) * 100 + spritePosition.Y);

                    //find the vector between the player and the point its heading to
                    VectorA = new Vector2(TempVector.X - spritePosition.X, TempVector.Y - spritePosition.Y);
                    VectorA90 = new Vector2(Vector90Degree.X - spritePosition.X, Vector90Degree.Y - spritePosition.Y);

                    //find the vector between the player and the enemy
                    VectorB = new Vector2(P.spritePosition.X - spritePosition.X, P.spritePosition.Y - spritePosition.Y);



                    //degree of the enemy to the head of the player.
                    //if enemy is 90 degrees to the head of the player then the angle will be pi/2
                    radian = DotProduct(VectorA, VectorB);


                    //this is the degree of the enemy to pi/2 degrees of the head. 
                    //so if the enemy is pi/2 degrees to the original head of the player, the new angle will be 0.
                    radian90 = DotProduct(VectorA90, VectorB);

                    if (radian90 < Math.PI / 2)
                    {
                        radian = 2 * Math.PI - radian;
                        
                    }
                    Rotation += -(float)radian;

                    break;
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


            radians = Math.Acos(Math.Round(DotProdNum / DotProdDenom, 6));
            return radians;
        }

    }
}
